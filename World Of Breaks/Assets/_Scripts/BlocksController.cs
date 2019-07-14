﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlocksController : MonoBehaviour {

	[System.Serializable]
	public class TilePresets {
		public int blocksWidth = 10;
		public int blocksHeight = 8;
		public float cameraSize;

		public TilePresets (int blocksWidth, int blocksHeight, float cameraSize)
		{
			this.blocksWidth = blocksWidth;
			this.blocksHeight = blocksHeight;
			this.cameraSize = cameraSize;
		}

		public TilePresets ()
		{
		}
		

	}

	public int presetIndex;
	public TilePresets[] presets;

	public TilePresets curPreset {
		
		get {
			if (!Game.isChallenge)
				return presets [presetIndex];
			else
				return challPreset;
		}
	}

	public TilePresets challPreset = new TilePresets (10, 8, 4.95f);

	public Block[][] blockMap;
	public BlockForSpawn[] blocksForSpawn;
	public Vector2 offsetBetweenBlocks;
	public int blocksLife;
	GameObject blockHolder;

	public event System.Action OnChangeTopLine;

	public Vector3 centerOfScreen;

	public static BlocksController Instance;
	public bool canSaveBlockMap;
	public Texture2D challengesMapTexture;

	private void Awake ()
	{
		Instance = this;
	}

	void Start ()
	{      
		blockHolder = new GameObject ("BlockHolder");
		//centerOfScreen = Camera.main.ScreenToWorldPoint (new Vector3 (Screen.width / 2f, Screen.height - Screen.height * .08f, 10));
		blockHolder.transform.position = Vector3.zero;// centerOfScreen;

		if (!Game.isChallenge)
			presetIndex = (int)TileSizeScreen.tileSize;
		else
			presetIndex = 0;

		Camera.main.orthographicSize = curPreset.cameraSize;

		InitializeBlockMap ();

		if (!Game.isChallenge) {
			if (!UIScreen.newGame && !BlocksSaver.LoadBlocksMap (this)) {
			}

			ShiftBlockMapDown ();
			canSaveBlockMap = true;
		} else {
			blockMap = textureToBlockMap (challengesMapTexture, Game.curChallengeIndex, curPreset.blocksWidth, curPreset.blocksHeight);
		}
	}

	public void CalculateBlockLife ()
	{
		blocksLife = 0;

		for (int i = 0; i < blockMap.Length; i++) {
			for (int j = 0; j < blockMap [i].Length; j++) {
				if (blockMap [i] [j].blockComp != null && blockMap [i] [j].blockComp.canLooseBeforeDown)
					blocksLife += blockMap [i] [j].blockLife;
			}
		}
        

		if (blocksLife <= 0) {
			if (!Game.isChallenge)
				UIScreen.Ins.ShowClearText ();
			else
				UIScreen.Ins.ChallengeCompleted ();
		}
	}

	void InitializeBlockMap ()
	{
		blockMap = new Block[curPreset.blocksWidth][];
		for (int i = 0; i < blockMap.Length; i++) {
			blockMap [i] = new Block[curPreset.blocksHeight];
			for (int j = 0; j < blockMap [i].Length; j++) {
				blockMap [i] [j] = new Block (0);
			}
		}
	}

	public void ShiftBlockMapDown ()
	{
		for (int i = blockMap.Length - 2; i >= 0; i--) {
			System.Array.Copy (blockMap [i], blockMap [i + 1], blockMap [0].Length);
		}

		for (int i = 0; i < blockMap.Length; i++) {
			for (int j = 0; j < blockMap [0].Length; j++) {
				
				if (blockMap [i] [j].blockComp == null)
					continue;
				blockMap [i] [j].blockComp.coordsY = i;

				Vector3 localPos = blockMap [i] [j].blockComp.transform.localPosition;
				localPos.y = ConvertCoordToPos (i).y;

				Vector3 pos = blockHolder.transform.TransformPoint (localPos);

				iTween.MoveTo (blockMap [i] [j].blockComp.gameObject, pos, .5f);
			}
		}

        
		if (OnChangeTopLine != null) {
			OnChangeTopLine ();
		}


           


		CheckForLose ();
		UIScreen.Ins.UpdateScore (++UIScreen.Ins.score);

		if (UIScreen.Ins.score >= 100) {
			if (TileSizeScreen.tileSize == TileSizeScreen.TileSize.Small && TileSizeScreen.tileSizeLocked.tileSizeLocked [0]) {
				TileSizeScreen.UnlockTile (0);
				UIScreen.Ins.ShowPopUpText (LocalizationManager.GetLocalizedText ("medium_unlocked"));
			}
		}

		if (UIScreen.Ins.score >= 150)
			if (TileSizeScreen.tileSize == TileSizeScreen.TileSize.Medium && TileSizeScreen.tileSizeLocked.tileSizeLocked [1]) {
				TileSizeScreen.UnlockTile (1);
				UIScreen.Ins.ShowPopUpText (LocalizationManager.GetLocalizedText ("big_unlocked"));
			}


		Utility.Invoke (this, .2f, ChangeTopLine);
        
        
		//Invoke ("ChangeTopLine", .2f);
	}

	void CheckForLose ()
	{
		for (int i = 0; i < blockMap [0].Length; i++) {

			if (blockMap [blockMap.Length - 1] [i].blockLife > 0) {
				UIScreen.OnLoseEventCall ();
				break;
			}
		}

		if (!UIScreen.Ins.playerLose) {
			for (int i = 0; i < blockMap [0].Length; i++) {
			
				if (blockMap [blockMap.Length - 1] [i].blockComp != null) {
					Destroy (blockMap [blockMap.Length - 1] [i].blockComp.gameObject);
				}
			}
		}
	}

	public void ChallengeProgress ()
	{
		UIScreen.Ins.UpdateScore (--UIScreen.Ins.score);

		if (OnChangeTopLine != null) {
			OnChangeTopLine ();
		}

		if (UIScreen.Ins.playerWin)
			UIScreen.OnWinEventCall ();
		else if (UIScreen.Ins.playerLose || UIScreen.Ins.score <= 0)
				UIScreen.OnLoseEventCall ();
	}

	System.Random rand = new System.Random ();

	void ChangeTopLine ()
	{
		double randNumPlusBlock = Random.Range (0, blockMap [0].Length - 1);

		for (int i = 0; i < blockMap [0].Length; i++) {

			BlockWithText spawnedBlock = null;

			if (randNumPlusBlock == i) {

				spawnedBlock = SpawnBlock (i, 0, 0);

				blockMap [0] [i] = new Block (0, 0, spawnedBlock);
				continue;
			}


			double randNum = rand.NextDouble ();


			if (randNum < .2f) {

			} else {

				int curLevelCur = (UIScreen.Ins.score % 10 == 0) ? UIScreen.Ins.score * Random.Range (1, 3) : UIScreen.Ins.score;

				int indexBlock = GetRandomBlockIndex ();

				spawnedBlock = SpawnBlock (i, 0, indexBlock);

				blockMap [0] [i] = new Block ((spawnedBlock.canLooseBeforeDown) ? curLevelCur : 0, indexBlock, spawnedBlock);

				//blockMap [0] [i] = new Block ((index == 1) ? 0 : (BallController.Instance.ballCount > CurLevel + CurLevel / 2 && Random.Range (0f, 1f) < .5f) ? (CurLevel + 1) * 2 : CurLevel + 1, SpawnBlock (i, 0, index));

			}

			if (spawnedBlock != null)
				iTween.FadeFrom (spawnedBlock.gameObject, 0, .5f);

		}

        

	}


	public Block[][] textureToBlockMap (Texture2D texture, int index, int blocksWidth = 10, int blocksHeight = 8)
	{
		Block[][] blockMap = new Block[blocksWidth][];
		for (int i = 0; i < blockMap.Length; i++) {
			blockMap [i] = new Block[blocksHeight];

			for (int j = 0; j < blockMap [i].Length; j++) {
				Color col = texture.GetPixel (j + index * blocksHeight, blocksWidth * 2 - 1 - i);
				float h, s, v;
				Color.RGBToHSV (col, out h, out s, out v);
                
				int blockIndex = Mathf.FloorToInt (h * 360 / 20);

				if (blockIndex == 0) {
					blockIndex = -1;
				}

				// print(i + "   " +j+ "  " + col + "  " + blockIndex);

				if (blockIndex == -1)
					blockMap [i] [j] = new Block (0);
				else {
					col = texture.GetPixel (j + index * blocksHeight, blocksWidth - 1 - i);
					int lifeCount = Mathf.FloorToInt (col.r * 255);
					BlockWithText go = SpawnBlock (j, i, blockIndex);
					go.isLoadingBlock = true;
					blockMap [i] [j] = new Block (lifeCount, blockIndex, go);
				}
			}
		}

		return blockMap;
	}


	int GetRandomBlockIndex ()
	{
		int RandomNum = rand.Next (0, GetSummChance ());

		int i = 0;
		int sum = 0;

		while (sum <= RandomNum) {
			
			if (!blocksForSpawn [i].isRequired) {

				sum += blocksForSpawn [i].chanceForSpawn;
			}

			i++;

		}

		//	print ("Обект с индексом" + i + "Номер " + RandomNum + "   Шанс" + GetSummChance ());

		return i - 1;
	}


	int GetSummChance ()
	{

		int summ = 0;

		for (int i = 0; i < blocksForSpawn.Length; i++) {
			if (!blocksForSpawn [i].isRequired)
				summ += blocksForSpawn [i].chanceForSpawn;
		}

		return summ;

	}

	void ShowMapInConsole ()
	{
		for (int i = 0; i < blockMap.Length; i++) {
			string str = "";

			for (int j = 0; j < blockMap [i].Length; j++) {
				str += blockMap [i] [j].blockLife + "    ";
			}

//			print (str);
		}
	}

	public BlockWithText SpawnBlock (int coordsX, int coordsY, int blockIndex)
	{

		BlockWithText tmp = Instantiate<GameObject> (blocksForSpawn [blockIndex].blockPrefab.gameObject).GetComponent<BlockWithText> ();

		tmp.transform.SetParent (blockHolder.transform, false);
		tmp.transform.localScale *= .8f;

		//float offset = (Screen.width - EdgeScreenCollisions.screenResolution.x) / 2f;

		//float scrrenOffset = Screen.width / blockMap [0].Length;

		//print (scrrenOffset);

		//Vector3 screenPos = Camera.main.ScreenToWorldPoint (new Vector3 (scrrenOffset * coordsX, Screen.height - Screen.height * .08f - (scrrenOffset * (coordsY + 1)), 0));

		tmp.transform.localPosition = ConvertCoordToPos (coordsY, coordsX);


		tmp.coordsX = coordsX;
		tmp.coordsY = coordsY;

		return tmp;
	}

	public BlockWithText SpawnBlock (Vector3 pos)
	{
		BlockWithText tmp = Instantiate<GameObject> (blocksForSpawn [1].blockPrefab.gameObject).GetComponent<BlockWithText> ();

		tmp.transform.position = pos - (Vector3.up - Vector3.left) * .5f;

		return tmp;
	}

	public Vector3 ConvertCoordToPos (int coordsY, int coordsX = 0)
	{

		//float offset = (Screen.width - EdgeScreenCollisions.screenResolution.x) / 2f;

		//float screenOffset = Screen.width / blockMap [0].Length;

		//Vector3 screenPos = Camera.main.ScreenToWorldPoint (new Vector3 (screenOffset, Screen.height - Screen.height * .08f - (screenOffset * (coordsY + 1)), 0));

		Vector3 posBlock = new Vector3 (offsetBetweenBlocks.x * coordsX - (blockMap [0].Length / 2f * offsetBetweenBlocks.x) + .015f, -offsetBetweenBlocks.y * (coordsY + 1) + ((blockMap.Length - 1) / 2f * offsetBetweenBlocks.y), 0);

		return posBlock;
	}

	public Vector2 GetBlockHolerSize ()
	{
		return new Vector2 (offsetBetweenBlocks.x * blockMap [0].Length / 2f, (blockMap.Length + 1) / 2f * offsetBetweenBlocks.y);
	}

	public IEnumerator DestroyBlockWhenLevelEnd (GameObject obj, int destroyLevelCount)
	{

		int curLevel = UIScreen.Ins.score;

		while (UIScreen.Ins.score < curLevel + destroyLevelCount) {
			yield return null;
		}

		Destroy (obj);
	}

	public void DestroyLastLineBtn ()
	{
		if (Game.isChallenge)
			return;

		int coinAmount = 20;

		if (User.HaveCoin (coinAmount) && DestroyLastLine (true)) {
			User.BuyWithCoin (coinAmount);
		}
	}

	public bool DestroyLastLine (bool checkBlockLife = true)
	{
        
		for (int i = blockMap.Length - 1; i >= 0; i--) {
			for (int j = 0; j < blockMap [i].Length; j++) {
				if (blockMap [i] [j].blockLife > 0) {
					DestroyLine (i, checkBlockLife);
					return true;
				}
			}
		}
		return false;
	}

	public void DestroyLine (int line, bool checkBlockLife = true)
	{
		for (int j = 0; j < blockMap [line].Length; j++) {
			if (!checkBlockLife || blockMap [line] [j].blockLife > 0) {
				if (blockMap [line] [j].blockComp != null) {
					blockMap [line] [j].blockComp.Die ();
				}

			}

		}
	}

	public void DestroyAllBlocks ()
	{
		canSaveBlockMap = false;
		for (int i = 0; i < blockMap.Length; i++) {
			for (int j = 0; j < blockMap [i].Length; j++) {
				if (blockMap [i] [j].blockComp != null) {
					blockMap [i] [j].blockComp.Die ();
				}
			}
		}
	}


	void OnValidate ()
	{
		foreach (var block in blocksForSpawn) {
			if (block.blockPrefab != null && string.IsNullOrEmpty (block.name)) {
				block.name = block.blockPrefab.transform.name;
			}
		}

	}

	void SaveOrDeleteBlocksMap ()
	{
		if (ScreenController.curActiveScreen == ScreenController.GameScreen.Continue) {
			BlocksSaver.DeleteBlockMapKeys ();
			PlayerPrefs.Save ();
		} else if (blockMap != null && canSaveBlockMap) {
				BlocksSaver.SaveBlocksMap (blockMap, BallController.Instance);
				PlayerPrefs.Save ();
			}
	}

	private void OnDestroy ()
	{
		SaveOrDeleteBlocksMap ();
	}

	#if UNITY_EDITOR
	void OnApplicationQuit ()
	{

		SaveOrDeleteBlocksMap ();

	}
	#else
	
	void OnApplicationPause ()
	{

	SaveOrDeleteBlocksMap ();

	}
#endif


}

public class Block {
	public int blockLife;
	public int blockIndex;
	public BlockWithText blockComp;

	public Block (int life, int blockIndex = -1, BlockWithText block = null)
	{
		this.blockLife = life;
		this.blockIndex = blockIndex;
		blockComp = block;
	}
}

[System.Serializable]
public class BlockForSpawn {
	public string name;
	[Range (0, 200)]
	public int chanceForSpawn;
	public bool isRequired;
	public BlockWithText blockPrefab;
}

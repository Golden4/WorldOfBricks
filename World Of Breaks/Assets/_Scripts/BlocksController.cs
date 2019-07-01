﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlocksController : SingletonGeneric<BlocksController> {
	
	public Block[][] blockMap = new Block[10] [];
	public BlockForSpawn[] blocksForSpawn;
	public Vector2 offsetBetweenBlocks;

	GameObject blockHolder;

	public Vector3 centerOfScreen;


	void Start ()
	{
		CurLevel = 1;

		blockHolder = new GameObject ("BlockHolder");
		centerOfScreen = Camera.main.ScreenToWorldPoint (new Vector3 (Screen.width / 2f, Screen.height - Screen.height * .08f, 10));
		blockHolder.transform.position = centerOfScreen;

		Screen.SetResolution (Mathf.RoundToInt (Screen.height / 16f * 10f), Screen.height, false);

		InitializeBlockMap ();
		//ShiftBlockMapDown ();
		ChangeTopLine ();
		ShowMapInConsole ();

	}


	void InitializeBlockMap ()
	{
		for (int i = 0; i < blockMap.Length; i++) {
			blockMap [i] = new Block[7];
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

		CurLevel++;

		Invoke ("ChangeTopLine", .2f);
		Invoke ("ShowMapInConsole", .2f);
		Invoke ("CheckForLose", .2f);
	}

	void CheckForLose ()
	{
		for (int i = 0; i < blockMap [0].Length; i++) {

			if (blockMap [blockMap.Length - 1] [i].blockLife > 0) {
				EventManager.OnLose ();
				break;
			}
		}

		if (!GameManager.Instance.playerLose) {
			for (int i = 0; i < blockMap [0].Length; i++) {
			
				if (blockMap [blockMap.Length - 1] [i].blockComp != null) {
					Destroy (blockMap [blockMap.Length - 1] [i].blockComp.gameObject);
				}
			}
		}

	}

	void ChangeTopLine ()
	{
		bool plusBallSpawned = false;

		double randNumPlusBlock = Random.Range (0, blockMap [0].Length - 1);

		for (int i = 0; i < blockMap [0].Length; i++) {

			BlockWithText spawnedBlock = null;

			if (randNumPlusBlock == i) {

				spawnedBlock = SpawnBlock (i, 0, 0);

				blockMap [0] [i] = new Block (0, spawnedBlock);
				continue;
			}


			double randNum = Random.Range (0f, 1f);

			if (randNum < .5f) {

			} else {

				int curLevelCur = (CurLevel % 10 == 0) ? CurLevel * Random.Range (1, 3) : CurLevel;

				//int indexBlock = Random.Range (1, blockPrefabs.Length);

				spawnedBlock = SpawnBlock (i, 0, GetRandomBlockIndex ());

				blockMap [0] [i] = new Block ((spawnedBlock.canLooseBeforeDown) ? curLevelCur : 0, spawnedBlock);

				//blockMap [0] [i] = new Block ((index == 1) ? 0 : (BallController.Instance.ballCount > CurLevel + CurLevel / 2 && Random.Range (0f, 1f) < .5f) ? (CurLevel + 1) * 2 : CurLevel + 1, SpawnBlock (i, 0, index));

			}

			if (spawnedBlock != null)
				iTween.FadeFrom (spawnedBlock.gameObject, 0, .5f);

		}
	}


	int GetRandomBlockIndex ()
	{
		float RandomNum = Random.Range (0f, GetSummChance ());

		int i = 0;
		float sum = 0;

		while (sum <= RandomNum) {
			
			if (!blocksForSpawn [i].isRequired) {

				sum += blocksForSpawn [i].chanceForSpawn / 100f;
			}

			i++;

		}

		//	print ("Обект с индексом" + i + "Номер " + RandomNum + "   Шанс" + GetSummChance ());

		return i - 1;
	}


	float GetSummChance ()
	{

		float summ = 0;

		for (int i = 0; i < blocksForSpawn.Length; i++) {
			if (!blocksForSpawn [i].isRequired)
				summ += blocksForSpawn [i].chanceForSpawn;
		}

		return summ / 100f;

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

	BlockWithText SpawnBlock (int coordsX, int coordsY, int blockIndex)
	{

		BlockWithText tmp = Instantiate<GameObject> (blocksForSpawn [blockIndex].blockPrefab.gameObject).GetComponent<BlockWithText> ();

		tmp.transform.SetParent (blockHolder.transform);

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

		Vector3 posBlock = new Vector3 (offsetBetweenBlocks.x * coordsX - (blockMap [0].Length / 2f * offsetBetweenBlocks.x - .03f), -offsetBetweenBlocks.y * (coordsY + 1), 0);

		return posBlock;
	}

	public IEnumerator DestroyBlockWhenLevelEnd (GameObject obj, int destroyLevelCount)
	{

		int curLevel = GameManager.Instance.curScore;

		while (GameManager.Instance.curScore < curLevel + destroyLevelCount) {
			yield return null;
		}

		Destroy (obj);

	}


	public int CurLevel {
		get {
			return GameManager.Instance.curScore;
		}

		set {
			GameManager.Instance.curScore = value;
			GameManager.Instance.UpdateCurScoreText ();
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

}

public class Block {
	public int blockLife;
	public BlockWithText blockComp;

	public Block (int life, BlockWithText block = null)
	{
		this.blockLife = life;
		blockComp = block;
	}
}

[System.Serializable]
public class BlockForSpawn {
	public string name;
	[Range (0, 100)]
	public int chanceForSpawn;
	public bool isRequired;
	public BlockWithText blockPrefab;
}

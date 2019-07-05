using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlocksController : MonoBehaviour {
	
	public Block[][] blockMap = new Block[10] [];
	public BlockForSpawn[] blocksForSpawn;
	public Vector2 offsetBetweenBlocks;
    public int blocksLife;
	GameObject blockHolder;

	public event System.Action OnChangeTopLine;

	public Vector3 centerOfScreen;

    public static BlocksController Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Start ()
	{
        UIScreen.Ins.UpdateScore(1);

        if (UIScreen.newGame)
        {
            BlocksSaver.DeleteBlockMapKeys();
            UIScreen.Ins.UpdateScore((UIScreen.Ins.checkpoint==0)? 1 : UIScreen.Ins.checkpoint);
            UIScreen.Ins.SetCheckpoint(0);
        }

		blockHolder = new GameObject ("BlockHolder");
		//centerOfScreen = Camera.main.ScreenToWorldPoint (new Vector3 (Screen.width / 2f, Screen.height - Screen.height * .08f, 10));
		blockHolder.transform.position = Vector3.zero;// centerOfScreen;
        

		InitializeBlockMap ();

        if(!BlocksSaver.LoadBlocksMap(this))
        {
            ChangeTopLine();
        }

        ShowMapInConsole();
    }

    public void CalculateBlockLife()
    {
        blocksLife = 0;

        for (int i = 0; i < blockMap.Length; i++)
        {
            for (int j = 0; j < blockMap[i].Length; j++)
            {
                blocksLife += blockMap[i][j].blockLife;
            }
        }
        

        if (blocksLife <= 0)
        {
            UIScreen.Ins.ShowClearText();
        }

    }

	void InitializeBlockMap ()
	{
		for (int i = 0; i < blockMap.Length; i++) {
			blockMap [i] = new Block[8];
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

        UIScreen.Ins.UpdateScore(++UIScreen.Ins.score);
        Utility.Invoke(this, .2f, ChangeTopLine);
		//Invoke ("ChangeTopLine", .2f);
	}

	void CheckForLose ()
	{
		for (int i = 0; i < blockMap [0].Length; i++) {

			if (blockMap [blockMap.Length - 1] [i].blockLife > 0) {
                UIScreen.OnLoseEventCall();
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


			double randNum = Random.Range (0f, 1f);

			if (randNum < .3f) {

			} else {

				int curLevelCur = (UIScreen.Ins.score % 10 == 0) ? UIScreen.Ins.score * Random.Range (1, 3) : UIScreen.Ins.score;

                int indexBlock = GetRandomBlockIndex();

                spawnedBlock = SpawnBlock (i, 0, indexBlock);

				blockMap [0] [i] = new Block ((spawnedBlock.canLooseBeforeDown) ? curLevelCur : 0, indexBlock, spawnedBlock);

				//blockMap [0] [i] = new Block ((index == 1) ? 0 : (BallController.Instance.ballCount > CurLevel + CurLevel / 2 && Random.Range (0f, 1f) < .5f) ? (CurLevel + 1) * 2 : CurLevel + 1, SpawnBlock (i, 0, index));

			}

			if (spawnedBlock != null)
				iTween.FadeFrom (spawnedBlock.gameObject, 0, .5f);

		}

        CheckForLose();

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

    public void DestroyLastLineBtn()
    {
        if (User.BuyWithCoin(10))
        {
            for (int i = blockMap.Length - 1; i >= 0; i--)
            {
                for (int j = 0; j < blockMap[i].Length; j++)
                {
                    if (blockMap[i][j].blockLife > 0)
                    {
                        DestroyLine(i);
                        return;
                    }
                }
            }
        }
    }

    public void DestroyLine(int line)
    {
        for (int j = 0; j < blockMap[line].Length; j++)
        {
            if (blockMap[line][j].blockLife > 0)
            {
                if (blockMap[line][j].blockComp != null)
                {
                    blockMap[line][j].blockComp.Die();
                }

            }

        }
    }

    public void DestroyAllBlocks()
    {
        for (int i = 0; i < blockMap.Length; i++)
        {
            for (int j = 0; j < blockMap[i].Length; j++)
            {
                if(blockMap[i][j].blockComp != null)
                {
                    blockMap[i][j].blockComp.Die();
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

#if UNITY_EDITOR
    void OnApplicationQuit()
    {
        if (blockMap != null)
        {
            BlocksSaver.SaveBlocksMap(blockMap, BallController.Instance);
            PlayerPrefs.Save();
        }

    }
#else
	
	void OnApplicationPause ()
	{
		BlocksSaver.SaveBlocksMap(blockMap, BallController.Instance);
		PlayerPrefs.Save ();
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

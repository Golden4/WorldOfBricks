using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BlocksController : MonoBehaviour
{
    [System.Serializable]
    public class TilePresets
    {
        public int blocksWidth = 10;
        public int blocksHeight = 8;
        public float cameraSize;

        public TilePresets(int blocksWidth, int blocksHeight, float cameraSize)
        {
            this.blocksWidth = blocksWidth;
            this.blocksHeight = blocksHeight;
            this.cameraSize = cameraSize;
        }

        public TilePresets() { }
    }

    public int presetIndex;
    public TilePresets[] presets;

    public TilePresets curPreset
    {

        get
        {
            if (!Game.isChallenge)
                return presets[presetIndex];
            else
                return challPreset;
        }
    }

    public TilePresets challPreset = new TilePresets(10, 8, 4.95f);

    public Block[][] blockMap;
    public Block[][] blockMapOld;
    public BlockForSpawn[] blocksForSpawn;
    public Vector2 offsetBetweenBlocks;
    [HideInInspector]
    public int blocksLife;
    GameObject blockHolder;

    public Gradient boxGradient;
    public Gradient boxGradient2;

    public event System.Action OnChangeTopLine;

    public static BlocksController Instance;
    [HideInInspector]
    public bool canSaveBlockMap;

    [HideInInspector]
    public int blockDestroyCount = 0;
    [HideInInspector]
    public int maxScore;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        blockHolder = new GameObject("BlockHolder");
        //centerOfScreen = Camera.main.ScreenToWorldPoint (new Vector3 (Screen.width / 2f, Screen.height - Screen.height * .08f, 10));
        blockHolder.transform.position = Vector3.zero; // centerOfScreen;

        if (!Game.isChallenge)
            presetIndex = (int)TileSizeScreen.tileSize;
        else
            presetIndex = 0;

        ResetCamera();

        InitializeBlockMap();

        if (!Game.isChallenge)
        {
            if (!UIScreen.newGame && !BlocksSaver.LoadBlocksMap(this)) { }

            ShiftBlockMapDown();
            canSaveBlockMap = true;
        }
        else
        {
            blockMap = textureToBlockMap(ChallengesInfo.GetChall.challengesGroups[Game.curChallengeGroupIndex].mapTexture, Game.curChallengeIndex, curPreset.blocksWidth, curPreset.blocksHeight);
        }

        if (Game.isChallenge)
            maxScore = GetMaxScore();
    }

    public void ResetCamera()
    {
        Camera.main.orthographicSize = curPreset.cameraSize;
    }

    public int CalculateBlockLife()
    {
        blocksLife = 0;

        for (int i = 0; i < blockMap.Length; i++)
        {
            for (int j = 0; j < blockMap[i].Length; j++)
            {
                if (blockMap[i][j].blockComp != null && blockMap[i][j].blockComp.canLooseDown)
                    blocksLife += blockMap[i][j].blockLife;
            }
        }

        if (blocksLife <= 0)
        {
            if (!Game.isChallenge)
                UIScreen.Ins.ShowClearText();
            else
                UIScreen.Ins.ChallengeCompleted();
        }

        return blocksLife;

    }

    void InitializeBlockMap()
    {
        blockMap = new Block[curPreset.blocksWidth][];
        for (int i = 0; i < blockMap.Length; i++)
        {
            blockMap[i] = new Block[curPreset.blocksHeight];
            for (int j = 0; j < blockMap[i].Length; j++)
            {
                blockMap[i][j] = new Block(0);
            }
        }
    }

    public void ShiftBlockMapDown()
    {

        if (!UIScreen.Ins.playerLose)
        {
            for (int i = 0; i < blockMap[0].Length; i++)
            {

                if (blockMap[blockMap.Length - 1][i].blockComp != null)
                {
                    blockMap[blockMap.Length - 1][i].blockComp.justDestroy = true;
                    blockMap[blockMap.Length - 1][i].blockComp.Die();
                }
            }
        }

        for (int i = blockMap.Length - 2; i >= 0; i--)
        {
            System.Array.Copy(blockMap[i], blockMap[i + 1], blockMap[0].Length);
        }

        if (OnChangeTopLine != null)
        {
            OnChangeTopLine();
        }

        for (int i = 0; i < blockMap.Length; i++)
        {
            for (int j = 0; j < blockMap[0].Length; j++)
            {

                if (blockMap[i][j].blockComp == null)
                    continue;

                if (blockMap[i][j].blockComp.isDead)
                    continue;

                blockMap[i][j].blockComp.coordsY = i;

                Vector3 localPos = blockMap[i][j].blockComp.transform.localPosition;
                localPos.y = ConvertCoordToPos(i).y;

                Vector3 pos = blockHolder.transform.TransformPoint(localPos);
                blockMap[i][j].blockComp.transform.DOMove(pos, .5f);
            }
        }

        CheckForLose();
        UIScreen.Ins.UpdateScore(++UIScreen.Ins.level);

        if (UIScreen.Ins.level >= 100)
        {

            if (TileSizeScreen.tileSize == TileSizeScreen.TileSize.Small && TileSizeScreen.tileSizeLocked.tileSizeLocked[0])
            {
                TileSizeScreen.UnlockTile(0);
                UIScreen.Ins.ShowPopUpText(LocalizationManager.GetLocalizedText("medium_unlocked"));
            }
            else if (TileSizeScreen.tileSize == TileSizeScreen.TileSize.Medium && TileSizeScreen.tileSizeLocked.tileSizeLocked[1])
            {
                TileSizeScreen.UnlockTile(1);
                UIScreen.Ins.ShowPopUpText(LocalizationManager.GetLocalizedText("big_unlocked"));
            }
        }

        AudioManager.PlaySoundFromLibrary("ChangeBlockLine");

        changeTopLineTween = DOVirtual.DelayedCall(.2f, () => ChangeTopLine());
    }

    Tween changeTopLineTween;

    void CheckForLose()
    {
        for (int i = 0; i < blockMap[0].Length; i++)
        {

            if (blockMap[blockMap.Length - 1][i].blockLife > 0)
            {
                UIScreen.OnLoseEventCall();
                break;
            }
        }
    }

    public void ChallengeProgress()
    {
        UIScreen.Ins.UpdateScore(--UIScreen.Ins.level);

        if (OnChangeTopLine != null)
        {
            OnChangeTopLine();
        }

        if (UIScreen.Ins.playerWin)
            UIScreen.OnWinEventCall();
        else if (UIScreen.Ins.playerLose || UIScreen.Ins.level <= 0)
            UIScreen.OnLoseEventCall();
    }

    System.Random rand = new System.Random();

    void ChangeTopLine()
    {

        double randNumPlusBlock = Random.Range(0, blockMap[0].Length - 1);

        for (int i = 0; i < blockMap[0].Length; i++)
        {

            BlockWithText spawnedBlock = null;

            if (randNumPlusBlock == i)
            {

                spawnedBlock = SpawnBlock(i, 0, 0);

                blockMap[0][i] = new Block(0, 0, spawnedBlock);
                continue;
            }

            double randNum = rand.NextDouble();

            if (randNum < .05f)
            {

            }
            else
            {
                int curLevelCur = (UIScreen.Ins.level % 10 == 0) ? UIScreen.Ins.level * 2 : UIScreen.Ins.level;

                int indexBlock = GetRandomBlockIndex();

                spawnedBlock = SpawnBlock(i, 0, indexBlock);

                blockMap[0][i] = new Block((spawnedBlock.canLooseDown) ? curLevelCur : 0, indexBlock, spawnedBlock);

                //blockMap [0] [i] = new Block ((index == 1) ? 0 : (BallController.Instance.ballCount > CurLevel + CurLevel / 2 && Random.Range (0f, 1f) < .5f) ? (CurLevel + 1) * 2 : CurLevel + 1, SpawnBlock (i, 0, index));

            }


            if (spawnedBlock != null)
                Utility.FadeSpritesFrom(spawnedBlock.transform, 0, .5f);


        }
    }

    public int GetMaxScore()
    {
        int blockDestroyCount = 0;
        int sum = 0;
        for (int i = 0; i < blockMap.Length; i++)
        {
            for (int j = 0; j < blockMap[i].Length; j++)
            {
                if (blockMap[i][j].blockComp != null && blockMap[i][j].blockComp.canLooseDown)
                {
                    blockDestroyCount += 10;
                    blockDestroyCount = blockDestroyCount % 200;
                    sum += blockDestroyCount;
                }
            }
        }

        return sum;
    }

    public Block[][] textureToBlockMap(Texture2D texture, int index, int blocksWidth = 10, int blocksHeight = 8)
    {
        Block[][] blockMap = new Block[blocksWidth][];
        for (int i = 0; i < blockMap.Length; i++)
        {
            blockMap[i] = new Block[blocksHeight];

            for (int j = 0; j < blockMap[i].Length; j++)
            {
                Color col = texture.GetPixel(j + index * blocksHeight, blocksWidth * 2 - 1 - i);
                float h, s, v;
                Color.RGBToHSV(col, out h, out s, out v);

                int blockIndex = Mathf.FloorToInt(h * 360 / 20);

                if (blockIndex == 0)
                {
                    blockIndex = -1;
                }

                // print(i + "   " +j+ "  " + col + "  " + blockIndex);

                if (blockIndex == -1)
                    blockMap[i][j] = new Block(0);
                else
                {
                    col = texture.GetPixel(j + index * blocksHeight, blocksWidth - 1 - i);
                    int lifeCount = Mathf.FloorToInt(col.r * 255);
                    BlockWithText go = SpawnBlock(j, i, blockIndex);
                    go.isLoadingBlock = true;
                    blockMap[i][j] = new Block(lifeCount, blockIndex, go);
                }
            }
        }

        return blockMap;
    }

    int GetRandomBlockIndex()
    {
        int RandomNum = rand.Next(0, GetSummChance());

        int i = 0;
        int sum = 0;

        while (sum <= RandomNum)
        {

            if (!blocksForSpawn[i].isRequired)
            {

                sum += blocksForSpawn[i].chanceForSpawn;
            }

            i++;

        }

        //	print ("Обект с индексом" + i + "Номер " + RandomNum + "   Шанс" + GetSummChance ());

        return i - 1;
    }

    int GetSummChance()
    {

        int summ = 0;

        for (int i = 0; i < blocksForSpawn.Length; i++)
        {
            if (!blocksForSpawn[i].isRequired)
                summ += blocksForSpawn[i].chanceForSpawn;
        }

        return summ;

    }

    void ShowMapInConsole(Block[][] blockMap)
    {
        if (blockMap != null)
            for (int i = 0; i < blockMap.Length; i++)
            {
                string str = "";

                for (int j = 0; j < blockMap[i].Length; j++)
                {
                    str += blockMap[i][j].blockIndex + "    ";
                }

                print(str);
            }
    }

    public BlockWithText SpawnBlock(int coordsX, int coordsY, int blockIndex)
    {

        BlockWithText tmp = Instantiate<GameObject>(blocksForSpawn[blockIndex].blockPrefab.gameObject).GetComponent<BlockWithText>();

        tmp.transform.SetParent(blockHolder.transform, false);
        tmp.transform.localScale *= .8f;

        //float offset = (Screen.width - EdgeScreenCollisions.screenResolution.x) / 2f;

        //float scrrenOffset = Screen.width / blockMap [0].Length;

        //print (scrrenOffset);

        //Vector3 screenPos = Camera.main.ScreenToWorldPoint (new Vector3 (scrrenOffset * coordsX, Screen.height - Screen.height * .08f - (scrrenOffset * (coordsY + 1)), 0));

        tmp.transform.localPosition = ConvertCoordToPos(coordsY, coordsX);

        tmp.coordsX = coordsX;
        tmp.coordsY = coordsY;

        return tmp;
    }

    public BlockWithText SpawnBlock(Vector3 pos)
    {
        BlockWithText tmp = Instantiate<GameObject>(blocksForSpawn[1].blockPrefab.gameObject).GetComponent<BlockWithText>();

        tmp.transform.position = pos - (Vector3.up - Vector3.left) * .5f;

        return tmp;
    }

    public Vector3 ConvertCoordToPos(int coordsY, int coordsX = 0)
    {

        //float offset = (Screen.width - EdgeScreenCollisions.screenResolution.x) / 2f;

        //float screenOffset = Screen.width / blockMap [0].Length;

        //Vector3 screenPos = Camera.main.ScreenToWorldPoint (new Vector3 (screenOffset, Screen.height - Screen.height * .08f - (screenOffset * (coordsY + 1)), 0));

        Vector3 posBlock = new Vector3(offsetBetweenBlocks.x * coordsX - (blockMap[0].Length / 2f * offsetBetweenBlocks.x) + .015f, -offsetBetweenBlocks.y * (coordsY + 1) + ((blockMap.Length - 1) / 2f * offsetBetweenBlocks.y), 0);

        return posBlock;
    }

    public Vector2 GetBlockHolerSize()
    {
        return new Vector2(offsetBetweenBlocks.x * blockMap[0].Length / 2f, (blockMap.Length + 1) / 2f * offsetBetweenBlocks.y);
    }

    public IEnumerator DestroyBlockWhenLevelEnd(GameObject obj, int destroyLevelCount)
    {

        int curLevel = UIScreen.Ins.level;

        while (UIScreen.Ins.level < curLevel + destroyLevelCount)
        {
            yield return null;
        }

        Destroy(obj);
    }

    public void DestroyLastLineBtn()
    {
        if (Game.isChallenge)
            return;

        int coinAmount = 20;

        if (User.HaveCoin(coinAmount) && DestroyLastLine(true))
        {
            User.BuyWithCoin(coinAmount);
        }

        UIScreen.Ins.EnableDestroyLastLineBtn(false);
    }

    public bool DestroyLastLine(bool checkBlockLife = true)
    {

        for (int i = blockMap.Length - 1; i >= 0; i--)
        {
            for (int j = 0; j < blockMap[i].Length; j++)
            {
                if (blockMap[i][j].blockLife > 0)
                {
                    DestroyLine(i, checkBlockLife);
                    return true;
                }
            }
        }
        return false;
    }

    public void DestroyLine(int line, bool checkBlockLife = true)
    {
        for (int j = 0; j < blockMap[line].Length; j++)
        {
            if (!checkBlockLife || blockMap[line][j].blockLife > 0)
            {
                if (blockMap[line][j].blockComp != null)
                {
                    blockMap[line][j].blockComp.Die();
                }

            }

        }
    }

    public void DestroyAllBlocks(bool needEffects = true)
    {
        canSaveBlockMap = false;
        for (int i = 0; i < blockMap.Length; i++)
        {
            for (int j = 0; j < blockMap[i].Length; j++)
            {
                if (blockMap[i][j].blockComp != null)
                {
                    blockMap[i][j].blockComp.needEffects = needEffects;
                    blockMap[i][j].blockComp.justDestroy = true;
                    blockMap[i][j].blockComp.Die();
                }
            }
        }
    }

    [HideInInspector]
    public int retryCount;
    [HideInInspector]
    public bool retryThrow;
    int oldPlayerScore;
    int oldBallCount;
    int oldLevel;
    Vector2 oldBallStartPos;
    bool saveAvaible;

    public void SaveOldState()
    {
        retryThrow = false;
        SaveBlockMapOld();
        oldPlayerScore = UIScreen.Ins.playerScore;
        oldLevel = UIScreen.Ins.level;
        oldBallCount = BallController.Instance.ballCount;
        oldBallStartPos = BallController.Instance.startThrowPos;
        saveAvaible = true;
    }

    public bool CanRetry()
    {
        return saveAvaible && !retryThrow;
    }

    void RetryThrowAction()
    {
        retryThrow = true;

        if (changeTopLineTween != null && changeTopLineTween.IsPlaying())
            changeTopLineTween.Kill();

        UIScreen.Ins.EnableRetryThrowBtn(false);

        DestroyAllBlocks(false);
        blockMap = blockMapOld;
        SpawnBlocksFromBlocksMap(blockMap);
        UIScreen.Ins.SetPlayerScore(oldPlayerScore);
        UIScreen.Ins.UpdateScore(oldLevel);
        UIScreen.Ins.HideTimeAcceleratorBtn();
        BallController.Instance.ballCount = oldBallCount;
        BallController.Instance.startThrowPos = oldBallStartPos;
        BallController.Instance.UpdateBallCount();
        BallController.Instance.ReturnAllBalls();
        retryCount++;
        Time.timeScale = 1;
        MessageBox.HideAll();
    }

    public void RetryThrow()
    {
        //if (!BallController.Instance.isThrowing)
        //	return;

        Time.timeScale = 0;
        int coinToRetry = 25 * (retryCount + 1);

        MessageBox.ShowStatic("Undo throw?", MessageBox.BoxType.Retry, delegate
        {
            Time.timeScale = 1;
        }).SetImageTextBtn(
            coinToRetry.ToString(),
            User.HaveCoin(coinToRetry),
            delegate
            {
                if (User.BuyWithCoin(coinToRetry))
                {
                    RetryThrowAction();
                }

            }, MessageBox.BtnSprites.Coin).SetImageBtn(true, delegate
            {
                if (AdManager.Ins != null)
                {
                    AdManager.Ins.showRewardedVideo();
                    AdManager.onRewardedVideoFinishedEvent -= RetryThrowAction;
                    AdManager.onRewardedVideoFinishedEvent += RetryThrowAction;
                }
            }, MessageBox.BtnSprites.Video, default, false);

        // MessageBox.ShowStatic("Undo throw?", delegate
        // {

        //     if (AdManager.Ins != null)
        //     {
        //         AdManager.Ins.showRewardedVideo();
        //         AdManager.onRewardedVideoFinishedEvent -= RetryThrowAction;
        //         AdManager.onRewardedVideoFinishedEvent += RetryThrowAction;
        //     }

        // }, delegate
        // {
        //     if (User.BuyWithCoin(coinToRetry))
        //     {
        //         RetryThrowAction();
        //     }
        // }, delegate
        // {
        //     Time.timeScale = 1;
        // }, true, User.HaveCoin(coinToRetry), coinToRetry.ToString());
    }

    public void SpawnBlocksFromBlocksMap(Block[][] blocksMap)
    {
        for (int i = 0; i < blockMap.Length; i++)
        {
            for (int j = 0; j < blockMap[0].Length; j++)
            {
                if (blockMap[i][j].blockIndex > -1)
                {
                    BlockWithText spawnedBlock = SpawnBlock(j, i, blockMap[i][j].blockIndex);
                    blockMap[i][j].blockComp = spawnedBlock;
                    blockMap[i][j].blockComp.isLoadingBlock = true;
                }
            }
        }
    }

    public void SaveBlockMapOld()
    {
        blockMapOld = new Block[curPreset.blocksWidth][];
        for (int i = 0; i < blockMap.Length; i++)
        {
            blockMapOld[i] = new Block[curPreset.blocksHeight];
            for (int j = 0; j < blockMap[i].Length; j++)
            {
                blockMapOld[i][j] = new Block(blockMap[i][j].blockLife, blockMap[i][j].blockIndex, null);
            }
        }
    }

    void OnValidate()
    {
        foreach (var block in blocksForSpawn)
        {
            if (block.blockPrefab != null && string.IsNullOrEmpty(block.name))
            {
                block.name = block.blockPrefab.transform.name;
            }
        }

    }

    void SaveOrDeleteBlocksMap()
    {
        if (ScreenController.curActiveScreen == ScreenController.GameScreen.Continue)
        {
            BlocksSaver.DeleteBlockMapKeys();
            PlayerPrefs.Save();
        }
        else if (blockMap != null && canSaveBlockMap)
        {
            BlocksSaver.SaveBlocksMap(blockMap, BallController.Instance);
            PlayerPrefs.Save();
        }
    }

    private void OnDestroy()
    {
        AdManager.onRewardedVideoFinishedEvent -= RetryThrowAction;
        SaveOrDeleteBlocksMap();
    }

#if UNITY_EDITOR
    void OnApplicationQuit()
    {

        SaveOrDeleteBlocksMap();

    }
#else

    void OnApplicationPause () {

        SaveOrDeleteBlocksMap ();

    }
#endif

}

public class Block
{
    public int blockLife;
    public int blockIndex;
    public BlockWithText blockComp;

    public Block(int life, int blockIndex = -1, BlockWithText block = null)
    {
        this.blockLife = life;
        this.blockIndex = blockIndex;
        blockComp = block;
    }
}

[System.Serializable]
public class BlockForSpawn
{
    public string name;
    [Range(0, 300)]
    public int chanceForSpawn;
    public bool isRequired;
    public BlockWithText blockPrefab;
}
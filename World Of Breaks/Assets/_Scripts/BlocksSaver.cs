using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BlocksSaver
{
    static string ConvertToString(Block[][] blockMap)
    {

        string blocksInfo = "";

        for (int i = 0; i < blockMap.Length; i++)
        {
            for (int j = 0; j < blockMap[i].Length; j++)
            {
                blocksInfo += blockMap[i][j].blockLife + "|" + blockMap[i][j].blockIndex + ((j == blockMap[i].Length - 1) ? "" : "+");
            }

            if (i != blockMap.Length - 1)
                blocksInfo += "/";

        }

        //MonoBehaviour.print (blocksInfo);

        return blocksInfo;

    }

    static void LoadBlockMapFromString(string blockInfo, BlocksController c)
    {
        string[] blocksLines = blockInfo.Split(('/'));

        for (int i = 0; i < blocksLines.Length; i++)
        {
            string[] blocks = blocksLines[i].Split('+');

            for (int j = 0; j < blocks.Length; j++)
            {

                string[] infoStr = blocks[j].Split('|');

                int[] info = new int[2];

                info[0] = int.Parse(infoStr[0]);
                info[1] = int.Parse(infoStr[1]);
                
                if (info[1] == -1)
                    c.blockMap[i][j] = new Block(0);
                else
                {
                    BlockWithText go = c.SpawnBlock(j, i, info[1]);
                    go.isLoadingBlock = true;
                    c.blockMap[i][j] = new Block(info[0], info[1], go);
                }
            }
        }
    }

    public static void SaveBlocksMap(Block[][] blockMap, BallController bc)
    {
            Debug.Log("Saved!!!");
            PlayerPrefs.SetString("BlocksMap", ConvertToString(blockMap));
            PlayerPrefs.SetInt("BallCount", bc.ballCount);
            PlayerPrefs.SetString("StartPos", bc.startThrowPos.x + "|" + bc.startThrowPos.y);
            PlayerPrefs.SetInt("CurLevel", UIScreen.Ins.score);
        
    }

    public static bool LoadBlocksMap(BlocksController c)
    {

        if (PlayerPrefs.HasKey("BlocksMap"))
        {
            LoadBlockMapFromString(PlayerPrefs.GetString("BlocksMap"), c);

            if (PlayerPrefs.HasKey("CurLevel"))
            {
                UIScreen.Ins.UpdateScore(PlayerPrefs.GetInt("CurLevel"));
            }

            if (PlayerPrefs.HasKey("BallCount"))
            {
                BallController.Instance.ballCount = PlayerPrefs.GetInt("BallCount");
            }
            if (PlayerPrefs.HasKey("StartPos"))
            {

                string[] pos = PlayerPrefs.GetString("StartPos").Split('|');
                BallController.Instance.startThrowPos = new Vector2(float.Parse(pos[0]), float.Parse(pos[1]));

            }

        }

        return PlayerPrefs.HasKey("BlocksMap");

    }

    public static void DeleteBlockMapKeys()
    {
        PlayerPrefs.DeleteKey("BlocksMap");
        PlayerPrefs.DeleteKey("CurLevel");
        PlayerPrefs.DeleteKey("BallCount");
        PlayerPrefs.DeleteKey("StartPos");
        PlayerPrefs.Save();
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChallengesScreen : ScreenBase
{

    public Transform challengesHolder;
    public GameObject challengePrefab;

    public ChallengeInfo[] challengeInfos;

    [System.Serializable]
    public class ChallengeInfo {

    }

    public static int curChallengeIndex;
    

    private void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject go = Instantiate(challengePrefab);
            go.transform.SetParent(challengesHolder, false);
            go.gameObject.SetActive(true);
            go.GetComponentInChildren<Text>().text = (i + 1).ToString();
            int index = i;
            go.GetComponent<Button>().onClick.AddListener( delegate { StartChallenges(index); });
        }
        
        //for (int i = 0; i < maps.GetLength(0); i++)
        //{
        //    for (int j = 0; j < maps.GetLength(1); j++)
        //    {
        //        string[] infoStr = maps[i,j].Split('|');

        //        int[] info = new int[2];

        //        info[0] = int.Parse(infoStr[0]);
        //        info[1] = int.Parse(infoStr[1]);

        //        if (info[1] == -1)
        //            c.blockMap[i][j] = new Block(0);
        //        else
        //        {
        //            BlockWithText go = c.SpawnBlock(j, i, info[1]);
        //            go.isLoadingBlock = true;
        //            c.blockMap[i][j] = new Block(info[0], info[1], go);
        //        }
        //    }
        //}

    }

    void StartChallenges(int indexChallenge)
    {
        Game.isChallenge = true;
        curChallengeIndex = indexChallenge;
        MenuScreen.Ins.StartGame();
    }



}

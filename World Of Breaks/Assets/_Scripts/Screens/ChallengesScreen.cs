﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChallengesScreen : ScreenBase
{
    public Transform challengesHolder;
    public GameObject challengePrefab;
    public List<Button> challList = new List<Button>();
    public Sprite lockedSprite;
    public Sprite complitedSprite;
    
    private void Start()
    {
        for (int i = 0; i < Database.GetChall.challengesData.Length; i++)
        {
            GameObject go = Instantiate(challengePrefab);
            go.transform.SetParent(challengesHolder, false);
            go.gameObject.SetActive(true);
            go.GetComponentInChildren<Text>().text = (i + 1).ToString();
            int index = i;
            go.GetComponent<Button>().onClick.AddListener( delegate { StartChallenges(index, Database.GetChall.challengesData[index]); });
            challList.Add(go.GetComponent<Button>());
            UpdateButtonState(i);
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
    int canPlayChallenegesCount = 0;

    void UpdateButtonState(int index)
    {
        Image spriteState = challList[index].transform.Find("StateIcon").GetComponent<Image>();
        
        if (User.GetChallengesData.challData[index])
        {
            spriteState.gameObject.SetActive(true);
            spriteState.sprite = complitedSprite;
            spriteState.color = Color.green;
            challList[index].GetComponent<ButtonIcon>().EnableBtn(true);
        } else if (/*index > 0 && User.GetChallengesData.challData[index - 1] || */canPlayChallenegesCount < 3 && !User.GetChallengesData.challData[index] || index == 0 && !User.GetChallengesData.challData[index])
        {
            canPlayChallenegesCount++;
            spriteState.gameObject.SetActive(false);
            challList[index].GetComponent<ButtonIcon>().EnableBtn(true);
        } else
        {
            spriteState.gameObject.SetActive(true);
            spriteState.sprite = lockedSprite;
            spriteState.color = Color.black;
            challList[index].GetComponent<ButtonIcon>().EnableBtn(false);
        }
    }

    void StartChallenges(int indexChallenge, ChallengesInfo.ChallengeInfo info)
    {
        Game.curChallengeIndex = indexChallenge;
        Game.curChallengeInfo = info;
        MenuScreen.Ins.StartGame(true, true);
    }



}
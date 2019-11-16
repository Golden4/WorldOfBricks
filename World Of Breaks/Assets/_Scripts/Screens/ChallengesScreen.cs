using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ChallengesScreen : ScreenBase<ChallengesScreen>
{
    public RectTransform challengesHolder;
    public GameObject challengePrefab;
    public Image lvlInfoImage;

    List<ChallengeButton> challList = new List<ChallengeButton>();

    public ScrollRect sr;

    public void ShowChallengesForGroup(int indexGroup)
    {
        //  challengesHolder.sizeDelta = new Vector2(challengesHolder.sizeDelta.x, 150 * ((ChallengesInfo.GetChall.challengesGroups[indexGroup].challengesData.Count / 3) + 1) + 20);

        for (int i = 0; i < ChallengesInfo.GetChall.challengesGroups[indexGroup].challengesData.Count; i++)
        {
            GameObject go = Instantiate(challengePrefab);
            go.transform.SetParent(challengesHolder, false);
            go.gameObject.SetActive(true);
            int index = i;
            int indexG = indexGroup;

            Button button = go.GetComponent<Button>();
            button.onClick.AddListener(delegate
            {
                StartChallenges(index, indexG, ChallengesInfo.GetChall.challengesGroups[indexG].challengesData[index]);
            });
            
            ChallengeButton challengeButton = go.GetComponent<ChallengeButton>();
            challengeButton.SetButton(User.GetChallengesData.challData[indexG].IsLevelLocked(index),
                (i + 1),
                ChallengesGroupScreen.Ins.progressColorGradient.Evaluate(indexGroup / (float)ChallengesInfo.GetChall.challengesGroups.Count),
                ChallengesGroupScreen.Ins.mainColorGradient.Evaluate(indexGroup / (float)ChallengesInfo.GetChall.challengesGroups.Count),
                User.GetChallengesData.GetValue(indexGroup, i)
                );
            challList.Add(challengeButton);
       
        }

        lvlInfoImage.GetComponent<ChallengeLvlBtnUI>().ChangeChallengeBtnInfo(indexGroup);
    }

    public override void OnActivate()
    {
        base.OnActivate();

        for (int i = 0; i < challList.Count; i++)
        {
            challList[i].transform.DOKill();
            challList[i].transform.DOScale(1, .5f).ChangeStartValue(Vector3.zero).ChangeEndValue(Vector3.one).SetDelay(.07f * i).SetEase(Ease.OutCubic);
        }

        lvlInfoImage.transform.DOKill();
        lvlInfoImage.transform.DOScaleY(1, .5f).ChangeStartValue(Vector3.zero).ChangeEndValue(Vector3.one).SetEase(Ease.OutCubic);

        // sr.GraphicUpdateComplete();
        // challengesHolder.anchoredPosition = new Vector2(challengesHolder.anchoredPosition.x, 0);
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        for (int i = 0; i < challList.Count; i++)
        {
            Destroy(challList[i].gameObject);
        }
        challList.Clear();
    }

    void StartChallenges(int indexChallenge, int indexGroup, ChallengesInfo.ChallengeInfo info)
    {
        Game.curChallengeIndex = indexChallenge;
        Game.curChallengeGroupIndex = indexGroup;
        Game.curChallengeInfo = info;
        TileSizeScreen.Ins.StartGame(true, true);
    }

    public void Back()
    {
        ScreenController.Ins.ActivateScreen(ScreenController.GameScreen.ChallegesGroup);
    }
}
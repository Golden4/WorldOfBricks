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

    List<Button> challList = new List<Button>();

    public ScrollRect sr;

    public void ShowChallengesForGroup(int indexGroup)
    {
        //  challengesHolder.sizeDelta = new Vector2(challengesHolder.sizeDelta.x, 150 * ((ChallengesInfo.GetChall.challengesGroups[indexGroup].challengesData.Count / 3) + 1) + 20);

        for (int i = 0; i < ChallengesInfo.GetChall.challengesGroups[indexGroup].challengesData.Count; i++)
        {
            GameObject go = Instantiate(challengePrefab);
            go.transform.SetParent(challengesHolder, false);
            go.gameObject.SetActive(true);
            go.GetComponentInChildren<Text>().text = (i + 1).ToString();
            int index = i;
            int indexG = indexGroup;

            Button button = go.GetComponent<Button>();

            button.onClick.AddListener(delegate
            {
                StartChallenges(index, indexG, ChallengesInfo.GetChall.challengesGroups[indexG].challengesData[index]);
            });

            challList.Add(button);

            Image image = go.GetComponent<Image>();
            image.color = ChallengesGroupScreen.Ins.mainColorGradient.Evaluate(indexGroup / (float)ChallengesInfo.GetChall.challengesGroups.Count); //ChallengesInfo.GetChall.challengesGroups[indexGroup].mainColor;

            int prevStarCount = 0;

            if (i - 1 >= 0)
            {
                prevStarCount = User.GetChallengesData.GetValue(indexGroup, i - 1);
            }

            UpdateButtonState(i, indexGroup, User.GetChallengesData.GetValue(indexGroup, i), prevStarCount);
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

    void UpdateButtonState(int index, int indexGroup, int starCount, int prevStarCount)
    {
        Image spriteState = challList[index].transform.GetChild(0).Find("StateIcon").GetComponent<Image>();

        if (starCount > 0)
        {
            ShowStars(true, index, starCount);
            spriteState.gameObject.SetActive(false);
            challList[index].GetComponent<ButtonIcon>().EnableBtn(true);
        }
        else if ((index == 0 && starCount == 0) || (starCount == 0 && prevStarCount > 0))
        {
            ShowStars(true, index, starCount);
            spriteState.gameObject.SetActive(false);
            challList[index].GetComponent<ButtonIcon>().EnableBtn(true);
        }
        else
        {
            ShowStars(false, index);
            spriteState.gameObject.SetActive(true);
            challList[index].GetComponent<ButtonIcon>().EnableBtn(false);
        }
    }

    void ShowStars(bool show, int index, int count = 0)
    {
        if (show)
        {
            Image[] stars = new Image[3];

            for (int i = 0; i < 3; i++)
            {
                stars[i] = challList[index].transform.GetChild(0).GetChild(0).GetChild(i).GetChild(0).GetComponent<Image>();

                stars[i].gameObject.SetActive(i < count);
            }
        }
        else
        {
            challList[index].transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        }
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
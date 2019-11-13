using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ChallengesGroupScreen : ScreenBase<ChallengesGroupScreen>
{
    public RectTransform challengeGroupHolder;
    public GameObject challengeGroupPrefab;

    List<Button> challGroupList = new List<Button>();

    public Gradient mainColorGradient;
    public Gradient progressColorGradient;
    public Gradient lockedColorGradient;

    public override void OnActivate()
    {
        base.OnActivate();

        for (int i = 0; i < challGroupList.Count; i++)
        {
            challGroupList[i].transform.DOKill();
            challGroupList[i].transform.DOScale(1, .8f).ChangeStartValue(Vector3.zero).ChangeEndValue(Vector3.one).SetDelay(.07f * i).SetEase(Ease.OutCubic);
        }
    }

    public override void OnInit()
    {
        base.OnInit();

        for (int i = 0; i < ChallengesInfo.GetChall.challengesGroups.Count; i++)
        {
            GameObject go = Instantiate(challengeGroupPrefab);
            go.transform.SetParent(challengeGroupHolder, false);
            go.gameObject.SetActive(true);

            int index = i;

            go.GetComponent<Button>().onClick.AddListener(delegate
            {
                if (!User.GetChallengesData.IsGroupLocked(index))
                {
                    ChallengesScreen.Ins.ShowChallengesForGroup(index);
                    ScreenController.Ins.ActivateScreen(ScreenController.GameScreen.Challeges);
                }
            });

            challGroupList.Add(go.GetComponent<Button>());

            ChallengeLvlBtnUI challengeLvlBtnUI = go.GetComponent<ChallengeLvlBtnUI>();

            if (i + 1 < ChallengesInfo.GetChall.challengesGroups.Count)
                if (!User.GetChallengesData.IsGroupLocked(i) && User.GetChallengesData.IsGroupLocked(i + 1))
                {
                    challengeLvlBtnUI.curActive = true;
                }
                else
                {
                    challengeLvlBtnUI.curActive = false;
                }

            challengeLvlBtnUI.ChangeChallengeBtnInfo(i);
        }


    }

}

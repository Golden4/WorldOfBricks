using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ChallengeLvlBtnUI : MonoBehaviour
{
    public Text nameText;
    public Text lvlText;
    public Text progressText;
    public Image stars;
    public Text starsText;
    public Image progressImage;
    public Image lockedImage;
    public Image circleOutline;
    public bool curActive;

    void Start()
    {
        if (circleOutline != null)
        {
            circleOutline.gameObject.SetActive(curActive);

            if (curActive)
            {
                circleOutline.transform.DOScale(1.3f, .5f).ChangeStartValue(Vector3.one * 1.1f).SetLoops(-1, LoopType.Yoyo);
            }
        }
    }

    public void ChangeChallengeBtnInfo(int groupIndex)
    {
        if (!User.GetChallengesData.IsGroupLocked(groupIndex))
            ChangeChallengeBtnInfo(User.GetChallengesData.IsGroupLocked(groupIndex),
                    ChallengesInfo.GetChall.challengesGroups[groupIndex].name,
                    User.GetChallengesData.GetCountData(groupIndex) + "/" + ChallengesInfo.GetChall.challengesGroups[groupIndex].challengesData.Count,
                    (groupIndex + 1).ToString(),
                    User.GetChallengesData.GetSummData(groupIndex) + "/" + ChallengesInfo.GetChall.challengesGroups[groupIndex].challengesData.Count * 3,
                    User.GetChallengesData.GetCountData(groupIndex) / (float)ChallengesInfo.GetChall.challengesGroups[groupIndex].challengesData.Count,
                    ChallengesGroupScreen.Ins.mainColorGradient.Evaluate(groupIndex / (float)ChallengesInfo.GetChall.challengesGroups.Count),
                    ChallengesGroupScreen.Ins.progressColorGradient.Evaluate(groupIndex / (float)ChallengesInfo.GetChall.challengesGroups.Count)
                    );
        else
            ChangeChallengeBtnInfo(User.GetChallengesData.IsGroupLocked(groupIndex),
            ChallengesInfo.GetChall.challengesGroups[groupIndex].name,
            User.GetChallengesData.GetCountData(groupIndex) + "/" + ChallengesInfo.GetChall.challengesGroups[groupIndex].challengesData.Count,
            (groupIndex + 1).ToString(),
            User.GetChallengesData.GetSummData(groupIndex) + "/" + ChallengesInfo.GetChall.challengesGroups[groupIndex].challengesData.Count * 3,
            User.GetChallengesData.GetCountData(groupIndex) / (float)ChallengesInfo.GetChall.challengesGroups[groupIndex].challengesData.Count,
            ChallengesGroupScreen.Ins.lockedColorGradient.Evaluate(groupIndex / (float)ChallengesInfo.GetChall.challengesGroups.Count)
            );
    }

    public void ChangeChallengeBtnInfo(bool locked, string nameText, string progressText, string lvlText, string starsText, float progessAmount, Color main, Color progressColor = default)
    {
        this.stars.gameObject.SetActive(!locked);
        this.lvlText.gameObject.SetActive(!locked);
        this.lockedImage.gameObject.SetActive(locked);
        this.nameText.text = nameText;
        this.lvlText.text = lvlText;
        this.lvlText.color = main;
        this.progressText.text = progressText;
        this.starsText.text = starsText;
        this.progressImage.gameObject.SetActive(!locked);
        this.progressImage.fillAmount = progessAmount;
        this.progressImage.color = progressColor;
        lockedImage.color = main;
        this.GetComponent<Image>().color = main;
    }

    public void ChangeChallengeBtnInfo(bool locked, string nameText, string secondText, string lvlText, Color main)
    {
        this.lvlText.gameObject.SetActive(!locked);
        this.lockedImage.gameObject.SetActive(locked);
        this.lvlText.text = lvlText;
        this.lvlText.color = main;
        this.nameText.text = nameText;
        this.progressText.text = secondText;
        lockedImage.color = main;
        this.GetComponent<Image>().color = main;
    }
}

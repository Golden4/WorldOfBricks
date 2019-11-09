using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ChallengeResultScreen : ScreenBase<ChallengeResultScreen>
{
    public Text resultText;
    public Button retryBtn;
    public Button nextBtn;

    public Transform starsParent;
    public Image[] stars;

    public PanelsController pc;

    public static float[] starPersents = new float[] {
        0.2f,
        0.6f,
        1f
    };

    public int[] coinsForStars = new int[] {
        5, 6, 7
    };

    public static float progressPersent
    {
        get
        {
            return (float)UIScreen.Ins.playerScore / BlocksController.Instance.maxScore;
        }
    }

    public override void OnInit()
    {
        pc = GetComponentInChildren<PanelsController>();
    }

    public override void OnActivate()
    {
        base.OnActivate();
        Game.gamesPlayed++;

        BallController.Instance.ReturnAllBalls();

        if (UIScreen.Ins.playerWin)
        {
            OnPlayerWin();

        }
        else if (UIScreen.Ins.playerLose)
        {
            OnPlayerLose();
        }
        else
        {
            OnPlayerLose();
        }

        Game.ballTryingIndex = -1;

    }

    public static int GetCurrentStarCount(float persent)
    {
        int starCount = 0;

        for (int i = 0; i < starPersents.Length; i++)
        {
            if (persent > starPersents[i])
            {
                starCount++;
            }
        }
        return starCount;
    }

    void OnPlayerWin()
    {
        pc.ShowGiftPanel(false);
        pc.ShowRewardPanel(true);

        if (Game.ballTryingIndex > -1)
        {
            pc.ShowBuyBallPanel(true);
            pc.ShowTryBallsPanel(false);
        }
        else if (Game.gamesPlayed % 3 == 1 && PanelsController.CanTakeBall())
        {
            pc.ShowBuyBallPanel(false);
            pc.ShowTryBallsPanel(true);
        }
        else
        {
            pc.ShowGiftPanel(true);
            pc.ShowBuyBallPanel(false);
            pc.ShowTryBallsPanel(false);
        }

        if (Game.curChallengeIndex + 1 < ChallengesInfo.GetChall.GetCurrentChallengesData().Length)
            nextBtn.gameObject.SetActive(true);
        else
            nextBtn.gameObject.SetActive(false);

        retryBtn.gameObject.SetActive(false);

        starsParent.gameObject.SetActive(true);

        int starCount = GetCurrentStarCount(progressPersent);

        int completedStars = User.GetChallengesData.GetCurrentValue();


        ShowStars(starCount, completedStars);

        if (completedStars < starCount)
        {
            int reward = 0;

            for (int i = completedStars; i < starCount; i++)
            {
                reward += coinsForStars[i];
            }

            User.AddCoin(reward);

            pc.rewardText.text = "+" + reward.ToString();

            resultText.text = string.Format(LocalizationManager.GetLocalizedText("challenge_complete"), ((Game.curChallengeGroupIndex + 1) + "-" + (Game.curChallengeIndex + 1)));

            User.GetChallengesData.SetCurrentValue(starCount);
        }
        else
        {
            pc.GiveReward(false);
            resultText.text = string.Format(LocalizationManager.GetLocalizedText("challenge_complete_again"), ((Game.curChallengeGroupIndex + 1) + "-" + (Game.curChallengeIndex + 1)));
        }

        //unlock new challenge group if last challenge cmplte
        if (User.GetChallengesData.GetCountData(Game.curChallengeGroupIndex) == ChallengesInfo.GetChall.challengesGroups[Game.curChallengeGroupIndex].challengesData.Count)
        {
            if (Game.curChallengeGroupIndex + 1 < User.GetChallengesData.challData.Length)
                User.GetChallengesData.challData[Game.curChallengeGroupIndex + 1].locked = false;
        }

        User.SaveChallengesData();
        AudioManager.PlaySoundFromLibrary("Success");
    }

    void OnPlayerLose()
    {
        pc.ShowGiftPanel(true);
        pc.ShowRewardPanel(false);

        if (Game.ballTryingIndex > -1)
        {
            pc.ShowBuyBallPanel(true);
            pc.ShowTryBallsPanel(false);
        }
        else if (Game.gamesPlayed % 3 == 1 && PanelsController.CanTakeBall())
        {
            pc.ShowBuyBallPanel(false);
            pc.ShowTryBallsPanel(true);
        }
        else
        {
            pc.ShowGiftPanel(true);
            pc.ShowBuyBallPanel(false);
            pc.ShowTryBallsPanel(false);
        }
        starsParent.gameObject.SetActive(false);
        nextBtn.gameObject.SetActive(false);
        retryBtn.gameObject.SetActive(true);
        resultText.text = string.Format(LocalizationManager.GetLocalizedText("challenge_failed"), ((Game.curChallengeGroupIndex + 1) + "-" + (Game.curChallengeIndex + 1)));
        AudioManager.PlaySoundFromLibrary("Failed");
    }

    void ShowStars(int starCount, int completedStars)
    {
        for (int i = 0; i < stars.Length; i++)
        {

            if (i < starCount)
            {
                stars[i].gameObject.SetActive(true);

                Text plusCoinText = stars[i].transform.GetChild(0).GetComponent<Text>();
                stars[i].transform.GetChild(0).gameObject.SetActive(false);

                GameObject go = Instantiate<GameObject>(Resources.Load<GameObject>("Particles/UI/StarParticle"));
                go.transform.SetParent(transform, false);
                go.transform.localScale = Vector3.one;
                ParticleSystem ps = go.GetComponent<ParticleSystem>();
                Destroy(go, 2);

                int index = i;
                stars[i].transform.DOScale(Vector3.one, .5f).ChangeStartValue(Vector3.zero).SetDelay(i * .5f).SetEase(Ease.OutElastic).OnComplete(delegate
                {
                    if (index >= completedStars)
                    {
                        plusCoinText.gameObject.SetActive(true);
                        plusCoinText.transform.DOScale(Vector3.one, .5f).ChangeStartValue(Vector3.zero).SetEase(Ease.OutBounce);
                        plusCoinText.text = "+" + coinsForStars[index];
                        pc.GiveReward(true, coinsForStars[index], stars[index].transform.position.x, stars[index].transform.position.y, false);
                    }
                }).OnStart(delegate
                {
                    // Vector3 pos = Camera.main.ScreenToWorldPoint(stars[index].transform.position);
                    ps.transform.position = stars[index].transform.position;
                    ps.Emit(15);
                });
                // }
            }
            else
            {
                stars[i].gameObject.SetActive(false);
            }
        }
    }

    public void LoadNextChallenge()
    {
        Game.curChallengeIndex++;
        Game.curChallengeInfo = ChallengesInfo.GetChall.GetCurrentChallengesData()[Game.curChallengeIndex];
        SceneController.RestartLevel();
    }
}

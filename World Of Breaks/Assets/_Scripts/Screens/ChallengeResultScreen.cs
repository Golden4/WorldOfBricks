using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeResultScreen : ScreenBase<ChallengeResultScreen> {
    [SerializeField]
    Text resultText;

    [SerializeField]
    Button retryBtnTextButton;

    [SerializeField]
    Button retryBtnSmallButton;

    [SerializeField]
    Button nextBtnTextButton;

    public Transform starsParent;
    public Image[] stars;

    [SerializeField]
    Text rewardText;

    // public PanelsController pc;

    public static float[] starPersents = new float[] {
        0.2f,
        0.6f,
        1f
    };

    public int[] coinsForStars = new int[] {
        5,
        6,
        7
    };

    public static float progressPersent {
        get {
            return (float) UIScreen.Ins.playerScore / BlocksController.Instance.maxScore;
        }
    }

    public override void OnInit () {
        // pc = GetComponentInChildren<PanelsController>();
    }

    public override void OnActivate () {
        base.OnActivate ();
        Game.gamesPlayed++;

        BallController.Instance.ReturnAllBalls ();

        if (UIScreen.Ins.playerWin) {
            OnPlayerWin ();

        } else if (UIScreen.Ins.playerLose) {
            OnPlayerLose ();
        } else {
            OnPlayerLose ();
        }

        Game.ballTryingIndex = -1;

    }

    public static int GetCurrentStarCount (float persent) {
        int starCount = 0;

        for (int i = 0; i < starPersents.Length; i++) {
            if (persent > starPersents[i]) {
                starCount++;
            }
        }
        return starCount;
    }

    void OnPlayerWin () {
        // pc.ShowGiftPanel(false);
        // pc.ShowRewardPanel(true);

        // if (Game.ballTryingIndex > -1)
        // {
        //     pc.ShowBuyBallPanel(true);
        //     pc.ShowTryBallsPanel(false);
        // }
        // else if (Game.gamesPlayed % 3 == 1 && PanelsController.CanTakeBall())
        // {
        //     pc.ShowBuyBallPanel(false);
        //     pc.ShowTryBallsPanel(true);
        // }
        // else
        // {
        //     pc.ShowGiftPanel(true);
        //     pc.ShowBuyBallPanel(false);
        //     pc.ShowTryBallsPanel(false);
        // }

        if (Game.curChallengeIndex + 1 < ChallengesInfo.GetChall.GetCurrentChallengesData ().Length)
            nextBtnTextButton.gameObject.SetActive (true);
        else
            nextBtnTextButton.gameObject.SetActive (false);

        retryBtnSmallButton.gameObject.SetActive (true);
        retryBtnTextButton.gameObject.SetActive (false);

        starsParent.gameObject.SetActive (true);

        int starCount = GetCurrentStarCount (progressPersent);

        int completedStars = User.GetChallengesData.GetCurrentValue ();
        // starCount = 3;
        // completedStars = 0;

        ShowStars (starCount, completedStars);

        if (completedStars < starCount) {
            int reward = 0;

            for (int i = completedStars; i < starCount; i++) {
                reward += coinsForStars[i];
            }

            User.AddCoin (reward);

            rewardText.text = "+" + reward.ToString ();

            resultText.text = string.Format (LocalizationManager.GetLocalizedText ("challenge_complete"), ((Game.curChallengeGroupIndex + 1) + "-" + (Game.curChallengeIndex + 1)));

            User.GetChallengesData.SetCurrentValue (starCount);
        } else {
            // pc.GiveReward(false);
            rewardText.text = "+0";
            resultText.text = string.Format (LocalizationManager.GetLocalizedText ("challenge_complete_again"), ((Game.curChallengeGroupIndex + 1) + "-" + (Game.curChallengeIndex + 1)));
        }

        User.GetChallengesData.challData[Game.curChallengeGroupIndex].UnlockNextLevel (Game.curChallengeIndex);

        //unlock new challenge group if last challenge cmplte
        if (User.GetChallengesData.GetCountData (Game.curChallengeGroupIndex) == ChallengesInfo.GetChall.challengesGroups[Game.curChallengeGroupIndex].challengesData.Count) {
            if (Game.curChallengeGroupIndex + 1 < User.GetChallengesData.challData.Length)
                User.GetChallengesData.UnlockNextGroup (Game.curChallengeGroupIndex);
        }

        User.SaveChallengesData ();
        AudioManager.PlaySoundFromLibrary ("Success");
    }

    void OnPlayerLose () {
        // pc.ShowGiftPanel(true);
        // pc.ShowRewardPanel(false);

        // if (Game.ballTryingIndex > -1)
        // {
        //     pc.ShowBuyBallPanel(true);
        //     pc.ShowTryBallsPanel(false);
        // }
        // else if (Game.gamesPlayed % 3 == 1 && PanelsController.CanTakeBall())
        // {
        //     pc.ShowBuyBallPanel(false);
        //     pc.ShowTryBallsPanel(true);
        // }
        // else
        // {
        //     pc.ShowGiftPanel(true);
        //     pc.ShowBuyBallPanel(false);
        //     pc.ShowTryBallsPanel(false);
        // }

        starsParent.gameObject.SetActive (false);

        nextBtnTextButton.gameObject.SetActive (false);
        retryBtnSmallButton.gameObject.SetActive (false);
        retryBtnTextButton.gameObject.SetActive (true);

        rewardText.text = "+0";
        resultText.text = string.Format (LocalizationManager.GetLocalizedText ("challenge_failed"), ((Game.curChallengeGroupIndex + 1) + "-" + (Game.curChallengeIndex + 1)));
        AudioManager.PlaySoundFromLibrary ("Failed");
    }

    [SerializeField] ParticleSystem pfStarParticle;
    [SerializeField] ParticleSystem pfConfettiParticle;

    void ShowStars (int starCount, int completedStars) {

        GameObject starParticle = Instantiate<GameObject> (pfStarParticle.gameObject);
        starParticle.transform.SetParent (transform, false);
        starParticle.transform.localScale = Vector3.one * 2;
        starParticle.transform.SetAsLastSibling ();
        ParticleSystem psStar = starParticle.GetComponent<ParticleSystem> ();
        Destroy (starParticle, 10f);

        GameObject confettiParticle = Instantiate<GameObject> (pfConfettiParticle.gameObject);
        confettiParticle.transform.SetParent (transform, false);
        confettiParticle.transform.SetSiblingIndex (1);
        confettiParticle.transform.localScale = Vector3.one * 1.5f;
        confettiParticle.GetComponent<RectTransform> ().anchoredPosition = Vector3.up * 150;
        ParticleSystem psConfetti = confettiParticle.GetComponent<ParticleSystem> ();
        psConfetti.Emit (80);
        Destroy (confettiParticle, 10f);

        for (int i = 0; i < stars.Length; i++) {
            if (i < starCount) {
                stars[i].gameObject.SetActive (true);

                Text plusCoinText = stars[i].transform.GetChild (0).GetComponent<Text> ();
                stars[i].transform.GetChild (0).gameObject.SetActive (false);
                int index = i;
                stars[i].transform.DOScale (Vector3.one, 1f).ChangeStartValue (Vector3.zero).SetDelay (i * .7f).SetEase (Ease.OutElastic).OnComplete (delegate {
                    if (index >= completedStars) {
                        plusCoinText.gameObject.SetActive (true);
                        plusCoinText.transform.DOScale (Vector3.one, .5f).ChangeStartValue (Vector3.zero).SetEase (Ease.OutBounce);
                        plusCoinText.text = "+" + coinsForStars[index];
                        // pc.GiveReward(true, coinsForStars[index], stars[index].transform.position.x, stars[index].transform.position.y, false);
                    }
                }).OnStart (delegate {
                    // Vector3 pos = Camera.main.ScreenToWorldPoint(stars[index].transform.position);
                    psStar.transform.position = stars[index].transform.position;
                    psStar.Emit (15);
                });
                // }
            } else {
                stars[i].gameObject.SetActive (false);
            }
        }
    }

    public void LoadNextChallenge () {
        Game.curChallengeIndex++;
        Game.curChallengeInfo = ChallengesInfo.GetChall.GetCurrentChallengesData () [Game.curChallengeIndex];
        SceneController.RestartLevel ();
    }
}
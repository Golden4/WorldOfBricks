using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : ScreenBase<GameOverScreen>
{
    [SerializeField]
    Text newRecordText;
    [SerializeField]
    Text levelText;
    [SerializeField]
    Text scoreText;
    [SerializeField]
    Text rewardText;

    // public PanelsController pc;

    public override void OnInit()
    {
        base.OnInit();
        // pc = GetComponentInChildren<PanelsController>();
    }

    public override void OnActivate()
    {
        base.OnActivate();
        Game.gamesPlayed++;

        BlocksController.Instance.DestroyAllBlocks();

        BlocksSaver.DeleteBlockMapKeys();
        PlayerPrefs.DeleteKey("TileSize");
        UIScreen.Ins.playerLose = true;
        UIScreen.newGame = true;
        BallController.Instance.ReturnAllBalls();
        UIScreen.Ins.SetTopScore();

        if (UIScreen.Ins.newRecord)
        {
            newRecordText.gameObject.SetActive(true);

        }
        else
        {
            newRecordText.gameObject.SetActive(false);
        }

        levelText.text = LocalizationManager.GetLocalizedText("level") + ": " + UIScreen.Ins.level.ToString();
        scoreText.text = LocalizationManager.GetLocalizedText("score") + ": " + UIScreen.Ins.playerScore;

        int coinAmount = UIScreen.Ins.playerScore / 200;
        User.AddCoin(coinAmount);

        rewardText.text = "+" + coinAmount;

        //if (!User.GetInfo.AllCharactersBought ()) {
        //	openBoxPanel.gameObject.SetActive (true);
        //	if (User.HaveCoin (PrizeScreen.GetBoxPrise ())) {
        //		openBoxBtn.gameObject.SetActive (true);
        //		coinSlider.gameObject.SetActive (false);
        //	} else {
        //		openBoxBtn.gameObject.SetActive (false);
        //		coinSlider.gameObject.SetActive (true);
        //		coinSlider.value = (float)User.Coins / PrizeScreen.GetBoxPrise ();
        //		needCoinText.text = User.Coins + "/" + PrizeScreen.GetBoxPrise ();
        //	}
        //} else {
        //	openBoxPanel.gameObject.SetActive (false);
        //}




        // pc.ShowGiftPanel(false);
        // pc.ShowRewardPanel(true);
        // int coinAmunt = UIScreen.Ins.playerScore / 100;
        // pc.GiveReward(true, coinAmunt);
        // User.AddCoin(coinAmunt);

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

        Game.ballTryingIndex = -1;
        AudioManager.PlaySoundFromLibrary("Failed");

    }

    public override void OnCleanUp()
    {
        base.OnCleanUp();
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
    }

    public void RestartLevel()
    {
        UIScreen.newGame = true;
        SceneController.RestartLevel();
    }

    public void ActivateMenu()
    {
        if (AdManager.Ins != null && Random.Range(0, 2) == 0 && Game.gamesPlayed % 5 == 0)
            AdManager.Ins.showInterstitial();

        SceneController.LoadSceneWithFade(1);
    }

}

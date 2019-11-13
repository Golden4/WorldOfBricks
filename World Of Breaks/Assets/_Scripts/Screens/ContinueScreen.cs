using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContinueScreen : ScreenBase<ContinueScreen>
{
    // [SerializeField] Transform continueAdPanel;
    // [SerializeField] Button continueAdBtn;
    // [SerializeField] Button continueCoinBtn;
    // public Text coinText;

    [SerializeField] Image continueTimer;
    float lastTime;
    float waitTime = 5;
    public int dieCount;

    public bool givedSecondChance = false;
    MessageBox messageBox;
    public override void OnActivate()
    {
        base.OnActivate();

        dieCount++;

        messageBox = MessageBox.ShowStatic("Continue?", MessageBox.BoxType.Continue, () => CloseContinueScreen(), false)
        .SetImageTextBtn((25 * dieCount).ToString(), true, () =>
        {
            if (User.BuyWithCoin(25 * dieCount))
                RetryGame();
            stopTimer = true;
        }, MessageBox.BtnSprites.Coin, default, User.HaveCoin(25 * dieCount))
        .SetImageBtn(true, () => RespawnPlayerVideo(), MessageBox.BtnSprites.Video, default, false);

        // coinText.text = (25 * dieCount).ToString();
        // continueCoinBtn.onClick.RemoveAllListeners();
        // continueCoinBtn.onClick.AddListener(delegate
        // {
        //     if (User.BuyWithCoin(25 * dieCount))
        //         RetryGame();
        // }
        // );

        lastTime = Time.time + .4f;
        AdManager.onRewardedVideoFinishedEvent += RetryGame;
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        stopTimer = false;
        AdManager.onRewardedVideoFinishedEvent -= RetryGame;
    }

    public void CloseContinueScreen()
    {
        messageBox.Hide();
        ScreenController.Ins.DeactivateScreen(this);
        ScreenController.Ins.ActivateScreen(ScreenController.GameScreen.GameOver);
    }

    void Update()
    {
        if (givedSecondChance && !AdController.Ins.interstitialLoaded)
        {
            CloseContinueScreen();
            return;
        }
        if (!stopTimer && messageBox != null)
        {
            messageBox.SetProgress(1 - ((Time.time - lastTime) / waitTime));
            // continueTimer.fillAmount = 1 - ((Time.time - lastTime) / waitTime);
        }

        if (lastTime + waitTime < Time.time && !stopTimer)
        {
            CloseContinueScreen();
        }

    }

    bool stopTimer;

    void RespawnPlayerVideo()
    {
        stopTimer = true;

        if (AdManager.Ins != null)
        {
            AdManager.Ins.showRewardedVideo();

        }
    }

    public void RetryGame()
    {
        ScreenController.Ins.ActivateScreen(ScreenController.GameScreen.UI);
        BlocksController.Instance.DestroyLastLine(false);
        UIScreen.Ins.playerLose = false;
        //Player.Ins.Retry ();
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PauseScreen : ScreenBase<PauseScreen>
{

    public override void OnActivate()
    {
        Game.isPause = true;
        Time.timeScale = 0;
        AdManager.Ins.showBanner();
        ActivatePauseBox();
    }

    public override void OnDeactivate()
    {
        Game.isPause = false;
        Time.timeScale = UIScreen.Ins.GetTimeAccelerationValue();
        AdManager.Ins.hideBanner();
    }

    public void Continue()
    {
        Game.isPause = false;
        Time.timeScale = UIScreen.Ins.GetTimeAccelerationValue();

        if (UIScreen.Ins.playerLose)
            ScreenController.Ins.ActivateScreen("GameOver");
        else
            ScreenController.Ins.ActivateScreen("UI");
    }

    public void ActivatePauseScreen()
    {
        ScreenController.Ins.ActivateScreen("Pause");
    }

    MessageBox pauseMessageBox;

    public void ActivateMenu()
    {
        Action action = () =>
        {
            Game.isPause = false;
            Time.timeScale = 1;
            AdManager.Ins.hideBanner();

            if (!Game.isChallenge)
            {
                ScreenController.Ins.ActivateScreen("GameOver");
            }
            else
            {
                ScreenController.Ins.ActivateScreen("ChallegesResult");
            }

            pauseMessageBox?.Hide();
        };

        if (!Game.isChallenge)
            MessageBox.ShowStatic("Quit game?", MessageBox.BoxType.Failed)
            .SetDesc(LocalizationManager.GetLocalizedText("warning"))
            .SetTextBtn("Ok", true, action)
            .SetTextBtn("Cancel", true);
        else
            action.Invoke();
    }

    public void RestartLevelBtn()
    {
        Action action = delegate
        {
            GameOverScreen.Ins.RestartLevel();
        };

        MessageBox.ShowStatic("Restart game?", MessageBox.BoxType.Retry)
        .SetDesc(LocalizationManager.GetLocalizedText("warning"))
        .SetTextBtn("Ok", true, action)
        .SetTextBtn("Cancel", true);
    }

    public void ActivatePauseBox()
    {
        pauseMessageBox = MessageBox.ShowStatic("Pause", MessageBox.BoxType.Pause, () => Deactivate())
        .SetImageBtn(true, () => ActivateMenu(), MessageBox.BtnSprites.Close, default, false)
        .SetImageBtn(true, () => RestartLevelBtn(), MessageBox.BtnSprites.Retry, default, false)
        .ShowSettings();
    }

}

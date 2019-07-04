﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseScreen : ScreenBase {

	public override void OnActivate ()
	{
		Game.isPause = true;
		Time.timeScale = 0;
	}

    public override void OnDeactivate()
    {
        Game.isPause = false;
        Time.timeScale = 1;
    }

    public void Continue ()
	{
		Game.isPause = false;
		Time.timeScale = 1;

        if(UIScreen.Ins.playerLose)
            ScreenController.Ins.ActivateScreen(ScreenController.GameScreen.GameOver);
        else
            ScreenController.Ins.ActivateScreen (ScreenController.GameScreen.UI);
	}

    public void ActivatePauseScreen()
    {
        ScreenController.Ins.ActivateScreen(ScreenController.GameScreen.Pause);
    }


}

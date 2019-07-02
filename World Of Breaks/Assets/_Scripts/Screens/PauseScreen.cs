using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseScreen : ScreenBase {

	public override void OnActivate ()
	{
		Game.isPause = true;
		Time.timeScale = 0;
	}

	public void Continue ()
	{
		Game.isPause = false;
		Time.timeScale = 1;
		ScreenController.Ins.ActivateScreen (ScreenController.GameScreen.UI);
	}

}

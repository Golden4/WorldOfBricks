using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseScreen : ScreenBase {



	public override void OnInit ()
	{
		base.OnInit ();

	}

	public override void OnActivate ()
	{
		Game.isPause = true;
		Time.timeScale = 0;
	}

	public override void OnDeactivate ()
	{
		Game.isPause = false;
		Time.timeScale = 1;
	}

	public void Continue ()
	{
		Game.isPause = false;
		Time.timeScale = 1;

		if (UIScreen.Ins.playerLose)
			ScreenController.Ins.ActivateScreen (ScreenController.GameScreen.GameOver);
		else
			ScreenController.Ins.ActivateScreen (ScreenController.GameScreen.UI);
	}

	public void ActivatePauseScreen ()
	{
		ScreenController.Ins.ActivateScreen (ScreenController.GameScreen.Pause);
	}

	public void ActivateMenu ()
	{

		BlocksSaver.DeleteBlockMapKeys ();

		UIScreen.newGame = true;
		UIScreen.Ins.playerLose = true;
		BlocksController.Instance.DestroyAllBlocks ();

		UIScreen.Ins.SetTopScore ();

		ScreenController.Ins.ActivateScreen (ScreenController.GameScreen.UI);

		Utility.Invoke (ScreenController.Ins, .5f, () => {
			SceneController.LoadSceneWithFade (1);
		});
	}


}

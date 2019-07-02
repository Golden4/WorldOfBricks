using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Game {

	public static event Action OnGameStarted;

	//public static bool isGameStarted {
	//	get {
	//		return ScreenController.curActiveScreen == ScreenController.GameScreen.UI || ScreenController.curActiveScreen == ScreenController.GameScreen.Pause;
	//	}
	//}

	public static bool isPause;

	public static void OnGameStartedCall ()
	{
		if (Game.OnGameStarted != null)
			Game.OnGameStarted ();
	}

}

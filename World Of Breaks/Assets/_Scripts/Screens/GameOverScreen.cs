using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : ScreenBase {

	public static GameOverScreen Ins;

	public Text newRecordText;
	public Text newRecordCountText;
	public static int playerDieCount = 0;

	public PanelsController pc;

	public override void OnInit ()
	{
		Ins = this;
		pc = GetComponentInChildren <PanelsController> ();

	}

	public override void OnActivate ()
	{
		base.OnActivate ();
		playerDieCount++;

		BlocksController.Instance.DestroyAllBlocks ();

		BlocksSaver.DeleteBlockMapKeys ();
		PlayerPrefs.DeleteKey ("TileSize");

		UIScreen.newGame = true;

		UIScreen.Ins.SetTopScore ();

		if (UIScreen.Ins.newRecord) {
			newRecordText.gameObject.SetActive (true);

		} else {
			newRecordText.gameObject.SetActive (false);
		}

		newRecordCountText.text = UIScreen.Ins.score.ToString ();


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

		pc.ShowGiftPanel ();
		pc.ShowBallsPanel (true);
	}

	public override void OnCleanUp ()
	{
		base.OnCleanUp ();
	}

	public override void OnDeactivate ()
	{
		base.OnDeactivate ();
	}

	public void RestartLevel ()
	{
		UIScreen.newGame = true;
		SceneController.RestartLevel ();
	}


	public void ActivateMenu ()
	{
		SceneController.LoadSceneWithFade (1);
	}

}

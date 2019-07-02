using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : ScreenBase {

	[SerializeField] Transform giftPanel;
	[SerializeField] Button giftBtn;
	[SerializeField] Text giftimerText;

	public Text newRecordText;
	public Text newRecordCountText;
	public static int playerDieCount = 0;

	public override void Init ()
	{
		base.Init ();
		giftBtn.onClick.RemoveAllListeners ();
		giftBtn.onClick.AddListener (GiftBtn);
	}

	public override void OnActivate ()
	{
		base.OnActivate ();
		playerDieCount++;

		UIScreen.Ins.SetTopScore ();

		if (UIScreen.Ins.newRecord) {
			newRecordText.gameObject.SetActive (true);
			newRecordText.GetComponent <GUIAnim> ().MoveIn (GUIAnimSystem.eGUIMove.Self);

		} else {
			newRecordText.gameObject.SetActive (false);
		}

		newRecordCountText.text = UIScreen.Ins.score.ToString ();
		newRecordCountText.GetComponent <GUIAnim> ().MoveIn (GUIAnimSystem.eGUIMove.Self);


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

		giftPanel.gameObject.SetActive (true);
		if (BuyCoinScreen.Ins.CanTakeGift ()) {
			giftBtn.gameObject.SetActive (true);
			giftimerText.gameObject.SetActive (false);
			giftPanel.transform.GetChild (0).GetComponent <Text> ().text = LocalizationManager.GetLocalizedText ("get_gift");
		} else {
			giftBtn.gameObject.SetActive (false);
			giftimerText.gameObject.SetActive (true);
			//string time = string.Format ("{0}", BuyCoinScreen.Ins.timeToGiveGift).Split ('.') [0];
			string timeR = (BuyCoinScreen.Ins.timeToGiveGift.Minutes + 1) + "m";
			giftimerText.text = timeR;
			giftPanel.transform.GetChild (0).GetComponent <Text> ().text = LocalizationManager.GetLocalizedText ("gift_through");
		}

	}

	void Update ()
	{
		if (!BuyCoinScreen.Ins.CanTakeGift ()) {
			string timeR = (BuyCoinScreen.Ins.timeToGiveGift.Minutes + 1) + " m.";
			giftimerText.text = timeR;
		} else if (!giftBtn.gameObject.activeInHierarchy) {
			giftBtn.gameObject.SetActive (true);
			giftimerText.gameObject.SetActive (false);
		}
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
		SceneController.RestartLevel ();
	}

	public void GiftBtn ()
	{
		ScreenController.Ins.ActivateScreen (ScreenController.GameScreen.BuyCoin);
	}


}

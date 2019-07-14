using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelsController : MonoBehaviour {

	public Transform giftPanel;
	public Button giftBtn;
	public Text giftimerText;

	void Update ()
	{
		if (!BuyCoinScreen.CanTakeGift ()) {
			giftBtn.gameObject.SetActive (false);
			giftimerText.gameObject.SetActive (true);
			string timeR = (BuyCoinScreen.timeToGiveGift.Minutes + 1) + " m.";
			giftimerText.text = timeR;

		} else if (!giftBtn.gameObject.activeInHierarchy) {
				giftBtn.gameObject.SetActive (true);
				giftimerText.gameObject.SetActive (false);
			}
	}

	public void ShowGiftPanel ()
	{
		giftBtn.onClick.RemoveAllListeners ();
		giftBtn.onClick.AddListener (GiftBtn);

		giftPanel.gameObject.SetActive (true);
		if (BuyCoinScreen.CanTakeGift ()) {
			giftBtn.gameObject.SetActive (true);
			giftimerText.gameObject.SetActive (false);
			giftPanel.transform.GetChild (0).GetComponentInChildren <Text> ().text = LocalizationManager.GetLocalizedText ("get_gift");
		} else {
			giftBtn.gameObject.SetActive (false);
			giftimerText.gameObject.SetActive (true);
			//string time = string.Format ("{0}", BuyCoinScreen.Ins.timeToGiveGift).Split ('.') [0];
			string timeR = (BuyCoinScreen.timeToGiveGift.Minutes + 1) + "m";
			giftimerText.text = timeR;
			giftPanel.transform.GetChild (0).GetComponentInChildren <Text> ().text = LocalizationManager.GetLocalizedText ("gift_through");
		}
	}

	public void GiftBtn ()
	{
		BuyCoinScreen.GiveGift (15, giftBtn.transform.position);
	}

	int videoItemindex = -1;
	bool clicked;
	int ballsShowCount = 2;
	public Transform ballsListParent;
	public GameObject ballItemPrefab;

	public void ShowBallsPanel ()
	{
		List<int> ballsIndex = new List<int> ();

		for (int i = 0; i < Database.Get.playersData.Length; i++) {

			if (Database.Get.playersData [i].buyType == ItemsInfo.BuyType.Video && !User.GetInfo.userData [i].bought) {
				ballsIndex.Add (i);
			}
		}

		int count = ballsIndex.Count;
		Debug.Log (count);
		while (count > ballsShowCount) {
			ballsIndex.RemoveAt (Random.Range (0, count));
			count--;
			Debug.Log (count + "   " + Random.Range (0, count));
		}

		for (int i = 0; i < ballsIndex.Count; i++) {
			GameObject ball = Instantiate (ballItemPrefab);
			ball.transform.SetParent (ballsListParent, false);
			ball.gameObject.SetActive (true);
			ball.transform.Find ("Icon").GetComponent <Image> ().sprite = Database.Get.playersData [ballsIndex [i]].ballSprite;
			ball.transform.Find ("Text").GetComponent <Text> ().text = LocalizationManager.GetLocalizedText (Database.Get.playersData [ballsIndex [i]].name);
			Button btn = ball.GetComponentInChildren <Button> ();

			btn.onClick.RemoveAllListeners ();
			int inx = ballsIndex [i];
			btn.onClick.AddListener (delegate {
				DialogBox.Show ("Loading video...", null, null, false, true);
				videoItemindex = inx;
				if (AdManager.Ins != null) {
					AdManager.Ins.showRewardedVideo ();
					if (!clicked) {
						clicked = true;
						AdManager.onRewardedVideoFinishedEvent += onRewardedVideoFinishedEvent;
					}
				}
			});
		}
	}

	void onRewardedVideoFinishedEvent ()
	{
		if (videoItemindex >= 0) {

			User.GetInfo.userData [videoItemindex].bought = true;
			User.SetPlayerIndex (videoItemindex);
			User.SaveUserInfo ();

			DialogBox.Hide ();
			clicked = false;
			AdManager.onRewardedVideoFinishedEvent -= onRewardedVideoFinishedEvent;
			GameOverScreen.Ins.ActivateMenu ();
		}
	}

	public Text rewardText;
	public Transform rewardPanel;

	public void ShowRewardPanel ()
	{
		rewardPanel.gameObject.SetActive (true);
	}
}

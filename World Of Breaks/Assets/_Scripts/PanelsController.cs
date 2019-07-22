using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;

public class PanelsController : MonoBehaviour {

	public Transform giftPanel;
	public Button giftBtn;
	public Text giftimerText;

	void Start ()
	{
		PurchaseManager.OnPurchaseNonConsumable += BuyPaidItemSuccess;
	}

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



	public bool ShowGiftPanel (bool show)
	{
		if (!show) {
			giftPanel.gameObject.SetActive (false);
		} else {
			giftPanel.gameObject.SetActive (true);
			giftBtn.onClick.RemoveAllListeners ();
			giftBtn.onClick.AddListener (GiftBtn);

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

		return BuyCoinScreen.CanTakeGift ();
	}

	public void GiftBtn ()
	{
		BuyCoinScreen.GiveGift (15, giftBtn.transform.position);
	}

	int videoItemindex = -1;
	bool clicked;
	int ballsShowCount = 2;
	public GameObject ballsPanel;
	public Transform ballsListParent;
	public GameObject ballItemPrefab;

	public void ShowTryBallsPanel (bool show)
	{
		if (!show) {
			ballsPanel.SetActive (false);
		} else {
			ballsPanel.SetActive (true);

			List<int> ballsIndex = new List<int> ();

			for (int i = 0; i < Database.Get.playersData.Length; i++) {

				if (Database.Get.playersData [i].buyType != ItemsInfo.BuyType.Video && !User.GetInfo.userData [i].bought) {
					ballsIndex.Add (i);
				}
			}

			int count = ballsIndex.Count;

			while (count > ballsShowCount) {
				ballsIndex.RemoveAt (Random.Range (0, count));
				count--;
			}

			if (ballsIndex.Count > 0) {
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
			} else {
				ballsPanel.SetActive (false);
			}
		}
	}

	static long _nextGiveBallTime = -1;

	static long nextGiveBallTime {
		get {
			if (_nextGiveBallTime == -1) {
				if (PlayerPrefs.HasKey ("ballTime"))
					_nextGiveBallTime = long.Parse (PlayerPrefs.GetString ("ballTime"));
				else
					_nextGiveBallTime = 0;
			}

			return _nextGiveBallTime;
		}

		set {
			_nextGiveBallTime = value;
			PlayerPrefs.SetString ("ballTime", _nextGiveBallTime.ToString ());
		}
	}

	public static bool CanTakeBall ()
	{
		return nextGiveBallTime < System.DateTime.Now.Ticks;
	}

	void onRewardedVideoFinishedEvent ()
	{
		if (videoItemindex >= 0) {
			Game.ballTryingIndex = videoItemindex;
			nextGiveBallTime = System.DateTime.Now.Ticks + new System.TimeSpan (0, 15, 0).Ticks;
			DialogBox.Hide ();
			clicked = false;
			AdManager.onRewardedVideoFinishedEvent -= onRewardedVideoFinishedEvent;
			GameOverScreen.Ins.ActivateMenu ();
		}
	}

	public GameObject ballsBuyPanel;
	public GameObject ballItem;
	int ballTryingIndex;

	public void ShowBuyBallPanel (bool show)
	{
		if (!show) {
			ballsBuyPanel.SetActive (false);
		} else {
			ballsBuyPanel.SetActive (true);
			ballTryingIndex = Game.ballTryingIndex;
			ballItem.transform.Find ("Icon").GetComponent <Image> ().sprite = Database.Get.playersData [ballTryingIndex].ballSprite;
			ballItem.transform.Find ("Text").GetComponent <Text> ().text = LocalizationManager.GetLocalizedText (Database.Get.playersData [ballTryingIndex].name);

			Button[] btn = ballItem.GetComponentsInChildren <Button> ();

			btn [0].GetComponentInChildren <Text> ().text = Database.Get.playersData [ballTryingIndex].price;
			btn [0].onClick.RemoveAllListeners ();

			int coinAmount = int.Parse (Database.Get.playersData [ballTryingIndex].price);

			btn [0].gameObject.GetComponent<ButtonIcon> ().EnableBtn (User.HaveCoin (coinAmount));
			int inx = ballTryingIndex;

			btn [0].onClick.AddListener (delegate {
				BuyCoinBall (ballTryingIndex);
			});

			string price = PurchaseManager.Ins.GetLocalizedPrice (Database.Get.playersData [ballTryingIndex].purchaseID);
			Text text = btn [1].GetComponentInChildren <Text> ();
			Image loading = btn [1].transform.GetChild (0).GetComponentInChildren <Image> ();
			if (price != "null") {
				text.gameObject.SetActive (true);
				loading.gameObject.SetActive (false);
				text.text = price;
			} else {
				text.gameObject.SetActive (false);
				loading.gameObject.SetActive (true);
			}
			btn [1].onClick.AddListener (delegate {
				BuyPaidBall (ballTryingIndex);
			});
		}
	}

	void BuyCoinBall (int index)
	{
		int coinAmount = int.Parse (Database.Get.playersData [index].price);

		if (User.BuyWithCoin (coinAmount)) {
			User.GetInfo.userData [index].bought = true;
			User.SaveUserInfo ();
			User.SetPlayerIndex (index);
			SceneController.LoadSceneWithFade (1);
		}
	}

	void BuyPaidBall (int index)
	{
		if (PurchaseManager.Ins.IsInitialized ()) {
			PurchaseManager.Ins.BuyNonConsumable (index);

		} else {
			DialogBox.Show ("Failed", null, null, true, false);
		}

	}

	public void BuyPaidItemSuccess (PurchaseEventArgs args)
	{
		string purchID = args.purchasedProduct.definition.id;

		int index = 0;

		for (int i = 0; i < Database.Get.playersData.Length; i++) {
			if (purchID == Database.Get.playersData [i].purchaseID) {
				index = i;
				break;
			}
		}

		User.GetInfo.userData [index].bought = true;
		User.SaveUserInfo ();
		User.SetPlayerIndex (index);
		Debug.Log ("You bought " + purchID + "  id " + index + " NonCon");

		SceneController.LoadSceneWithFade (1);

	}

	void OnDestroy ()
	{
		PurchaseManager.OnPurchaseNonConsumable -= BuyPaidItemSuccess;
	}

	public Text rewardText;
	public Transform rewardPanel;

	public void ShowRewardPanel (bool show)
	{
		rewardPanel.gameObject.SetActive (show);
	}

	public void GiveReward (bool give, int rewardCount = 0, float xPos = 0, float yPos = 0, bool setRewardText = true)
	{
		if (give) {
			
			if (setRewardText)
				rewardText.text = "+" + rewardCount.ToString ();

			int coinAmount = rewardCount;

			Vector3 fromPos = new Vector3 (xPos, yPos);
			if ((xPos + yPos) == 0) {
				fromPos = rewardText.transform.position;
			}

			Vector3 toPos = CoinUI.Ins.coinImage.transform.position;

			Utility.CoinsAnimateRadial (CoinUI.Ins, CoinUI.Ins.coinImage.gameObject, CoinUI.Ins.transform.parent, (coinAmount / 2) % 100, fromPos, toPos, Screen.width / 3, .5f, CoinUI.Ins.curve, () => {

			});

			Utility.Invoke (CoinUI.Ins, .9f, delegate {
				User.AddCoin (coinAmount);
			}, true);

		} else {
			rewardText.text = "0";
		}

	}
}

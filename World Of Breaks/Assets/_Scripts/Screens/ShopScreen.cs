using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;

public class ShopScreen : ScreenBase {

	public GameObject itemsHolder;

	public ScrollSnap scrollSnap;

	public int curActiveItem {
		get {
			return scrollSnap.GetCurItemIndex;
		}
	}

	public int ItemCount {
		get {
			return scrollSnap.items.Length;
		}
	}

	public Text itemNameText;
	public Text itemAbilityText;

	public Button SelectAndPlayBtn;

	public Button buyPaidBtn;
	public Button buyCoinBtn;
	public Button buyVideoBtn;

	public override void Init ()
	{
		scrollSnap.Init ();

		scrollSnap.OnChangeItemEvent += OnChangeItem;

		SelectAndPlayBtn.onClick.RemoveAllListeners ();
		SelectAndPlayBtn.onClick.AddListener (() => {
			SelectAndPlay (curActiveItem);
		});

		buyPaidBtn.onClick.RemoveAllListeners ();
		buyPaidBtn.onClick.AddListener (() => {
			BuyPaidItem (curActiveItem);
		});

		buyCoinBtn.onClick.RemoveAllListeners ();
		buyCoinBtn.onClick.AddListener (() => {
			BuyCoinItem (curActiveItem);
		});

		buyVideoBtn.onClick.RemoveAllListeners ();
		buyVideoBtn.onClick.AddListener (() => {
			BuyVideoItem (curActiveItem);
		});

		for (int i = 0; i < ItemCount; i++) {
			scrollSnap.SetItemState (i, User.GetInfo.userData [i].bought);
		}

		PurchaseManager.OnPurchaseNonConsumable += BuyPaidItemSuccess;
	}

	public override void OnActivate ()
	{
		for (int i = 0; i < ItemCount; i++) {
			scrollSnap.SetItemState (i, User.GetInfo.userData [i].bought);
		}

		UpdateItemState (curActiveItem);
		scrollSnap.SnapToObj (User.GetInfo.curPlayerIndex, false);
	}

	void onRewardedVideoFinishedEvent ()
	{
		if (videoItemindex >= 0) {
			BuyItemSuccess (videoItemindex);
			DialogBox.Hide ();
			clicked = false;
			AdManager.onRewardedVideoFinishedEvent -= onRewardedVideoFinishedEvent;
		}
	}

	public override void OnDeactivate ()
	{
		base.OnDeactivate ();

	}

	public override void OnCleanUp ()
	{
		scrollSnap.OnChangeItemEvent -= OnChangeItem;
		PurchaseManager.OnPurchaseNonConsumable -= BuyPaidItemSuccess;
	}

	void OnChangeItem (int index)
	{
		itemNameText.text = LocalizationManager.GetLocalizedText (Database.Get.playersData [index].name);
		itemAbilityText.text = LocalizationManager.GetLocalizedText (Database.Get.playersData [index].name + "_desc");
        

		UpdateItemState (index);
	}

	public void SelectAndPlay (int index)
	{
		User.SetPlayerIndex (index);
		ScreenController.Ins.ActivateScreen (ScreenController.GameScreen.Menu);
	}

	/*	public void BuyItemWithCoin (int index)
	{
		if (User.BuyWithCoin (Database.Get.playersData [index].price)) {
			User.GetInfo.userData [index].bought = true;
			UpdateItemState (index);
			scrollSnap.SetItemState (index, User.GetInfo.userData [index].bought);
			User.SaveUserInfo ();
		}
	}*/

	public void BuyPaidItem (int index)
	{
		PurchaseManager.Ins.BuyNonConsumable (index);
	}

	public void BuyCoinItem (int index)
	{

		if (string.IsNullOrEmpty (Database.Get.playersData [index].price)) {
			Debug.LogError ("Not set coinAmount " + index);
			return;
		}

		int coinAmount = int.Parse (Database.Get.playersData [index].price);
        
		if (User.BuyWithCoin (coinAmount)) {
			BuyItemSuccess (index);
		}
	}

	int videoItemindex = -1;
	bool clicked;

	public void BuyVideoItem (int index)
	{
		DialogBox.Show ("Loading video...", null, null, false, true);
		videoItemindex = index;
		if (AdManager.Ins != null) {
			AdManager.Ins.showRewardedVideo ();
			if (!clicked) {
				clicked = true;
				AdManager.onRewardedVideoFinishedEvent += onRewardedVideoFinishedEvent;
			}
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

		Debug.Log ("You bought " + purchID + "  id " + index + " NonCon");
		BuyItemSuccess (index);
	}

	void BuyItemSuccess (int index)
	{
		User.GetInfo.userData [index].bought = true;
		UpdateItemState (index);
		scrollSnap.SetItemState (index, User.GetInfo.userData [index].bought);
		User.SaveUserInfo ();
	}

	int lastItemIndex = -1;

	public void UpdateItemState (int index)
	{
		bool bought = User.GetInfo.userData [index].bought;

		if (lastItemIndex >= 0) {
			scrollSnap.items [lastItemIndex].GetComponent<Animation> ().Stop ();
			scrollSnap.items [lastItemIndex].transform.localPosition = Vector3.right * scrollSnap.distanceItems * lastItemIndex;
		}
		if (bought) {
			scrollSnap.items [index].GetComponent<Animation> ().enabled = true;
			scrollSnap.items [index].GetComponent<Animation> ().Play ();
		}
		lastItemIndex = index;

		SelectAndPlayBtn.gameObject.SetActive (bought);
		ShowBuyBtb (!bought, Database.Get.playersData [index].buyType, index);
        
	}

	Button ShowBuyBtb (bool show, ItemsInfo.BuyType type = ItemsInfo.BuyType.Coin, int indexPlayer = -1)
	{
		Button[] btns = new Button[] {
			buyCoinBtn,
			buyPaidBtn,
			buyVideoBtn
		};

		for (int i = 0; i < btns.Length; i++) {
			if (show)
				btns [i].gameObject.SetActive ((int)type == i);
			else
				btns [i].gameObject.SetActive (false);
		}

		if (!show)
			return null;

		switch (type) {
		case ItemsInfo.BuyType.Coin:
			btns [(int)type].GetComponentInChildren<Text> ().text = Database.Get.playersData [indexPlayer].price;
			break;
		case ItemsInfo.BuyType.RealMoney:
			btns [(int)type].GetComponentInChildren<Text> ().text = PurchaseManager.Ins.GetLocalizedPrice (Database.Get.playersData [indexPlayer].purchaseID);
			break;
		case ItemsInfo.BuyType.Video:

			break;

		default:
			break;
		}

		return btns [(int)type];

	}

	public void BackBtn ()
	{
		ScreenController.Ins.ActivateScreen (ScreenController.GameScreen.Menu);

	}

}

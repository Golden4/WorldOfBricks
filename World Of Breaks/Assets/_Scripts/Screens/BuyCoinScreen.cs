using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BuyCoinScreen : ScreenBase {

	public static BuyCoinScreen Ins;

	public BuyCoinItem[] buyCoinBtns;

	public ButtonIcon buyCoinScreenBtn;
    public ButtonIcon showMenuBtn;

    public Text giftTitle;

	public override void OnActivate ()
	{
		base.OnActivate ();

		for (int i = 0; i < buyCoinBtns.Length; i++) {
			buyCoinBtns [i].priceText.text = PurchaseManager.Ins.GetLocalizedPrice (buyCoinBtns [i].productID).ToString ();
			int index = i;

			buyCoinBtns [i].btn.onClick.RemoveAllListeners ();

			buyCoinBtns [i].btn.onClick.AddListener (() => BuyItem (buyCoinBtns [index].productID));
		}

	}

	public void ShowBuyCoinScreen ()
	{
		ScreenController.Ins.ActivateScreen (ScreenController.GameScreen.BuyCoin);
	}

	public void BuyItem (string id)
	{
		PurchaseManager.Ins.BuyConsumable (id);
	}

	[System.Serializable]
	public class BuyCoinItem {
		public string productID;
		public Button btn;
		public Text priceText;
		public int coinCount;
	}

	public Button getCoinsBtn;

	public Text timer;

	public TimeSpan nextGiftTime = new TimeSpan (0, 10, 0);

    static long _nextGiveGiftTime = -1;

    static long nextGiveGiftTime
    {
        get
        {
            if (_nextGiveGiftTime == -1)
            {
                if (PlayerPrefs.HasKey("giftTime"))
                    _nextGiveGiftTime = long.Parse(PlayerPrefs.GetString("giftTime"));
                else
                    _nextGiveGiftTime = 0;
            }

            return _nextGiveGiftTime;
        }

        set
        {
            _nextGiveGiftTime = value;
            PlayerPrefs.SetString("giftTime", nextGiveGiftTime.ToString());
        }
    }

    public override void Init ()
	{
		base.Init ();

        Ins = this;

        //if (PlayerPrefs.HasKey ("giftTime"))
        //nextGiveGiftTime = new DateTime (long.Parse (PlayerPrefs.GetString ("giftTime")));
        getCoinsBtn.onClick.RemoveAllListeners ();
		getCoinsBtn.onClick.AddListener (GiveGift);

		if (CanTakeGift ()) {
			OnCanTakeGift ();
		} else {
			OnDontTakeGift ();
		}
        
        
		PurchaseManager.OnPurchaseConsumable += OnPurchaseConsumable;
	}

	public override void OnCleanUp ()
	{
		base.OnCleanUp ();
		PurchaseManager.OnPurchaseConsumable -= OnPurchaseConsumable;
	}

	void OnPurchaseConsumable (UnityEngine.Purchasing.PurchaseEventArgs args)
	{
		List<BuyCoinItem> list = new List<BuyCoinItem> (buyCoinBtns);
		int index = list.FindIndex (x => x.productID == args.purchasedProduct.definition.id);

		int coinAmount = list [index].coinCount;
        

		Vector3 fromPos = list [index].btn.transform.position;
		Vector3 toPos = CoinUI.Ins.coinImage.transform.position;

		Utility.CoinsAnimateRadial (CoinUI.Ins, CoinUI.Ins.coinImage.gameObject, CoinUI.Ins.transform, coinAmount / 20, fromPos, toPos, Screen.width / 3, .5f, CoinUI.Ins.curve, () => {
			AudioManager.PlaySoundFromLibrary ("Coin");
		});

        Utility.Invoke(this, .9f, delegate
        {
            User.AddCoin(coinAmount);
        });

    }

	void GiveGift ()
	{
		if (CanTakeGift ()) {

			nextGiveGiftTime = DateTime.Now.Ticks + nextGiftTime.Ticks;

            //PlayerPrefs.SetString ("giftTime", nextGiveGiftTime.Ticks.ToString ());

			int coinAmount = 15;

			Vector3 fromPos = getCoinsBtn.transform.position;
            Vector3 toPos = CoinUI.Ins.coinImage.transform.position;

            Utility.CoinsAnimateRadial(CoinUI.Ins, CoinUI.Ins.coinImage.gameObject, CoinUI.Ins.transform, coinAmount / 2, fromPos, toPos, Screen.width/3, .5f, CoinUI.Ins.curve, () => {
                AudioManager.PlaySoundFromLibrary("Coin");
            });

            Utility.Invoke(this, .9f, delegate
            {
                User.AddCoin(coinAmount);
            });
        }
	}

	public static bool CanTakeGift ()
	{
		return nextGiveGiftTime < DateTime.Now.Ticks;
	}

	public static TimeSpan timeToGiveGift {
		get {
			return new DateTime (nextGiveGiftTime - DateTime.Now.Ticks).TimeOfDay;
		}
	}

	bool lastOnTakeGiftBool;

	void Update ()
	{
		if ((nextGiveGiftTime - DateTime.Now.Ticks) > 0) {

			if (!lastOnTakeGiftBool) {
				lastOnTakeGiftBool = true;
				OnDontTakeGift ();
			}

			timer.text = string.Format ("{0}", timeToGiveGift).Split ('.') [0];
		} else if (lastOnTakeGiftBool) {
			lastOnTakeGiftBool = false;
			OnCanTakeGift ();
		}
	}

	void OnCanTakeGift ()
	{
		buyCoinScreenBtn.changingColor = true;
        showMenuBtn.changingColor = true;
        getCoinsBtn.GetComponent <ButtonIcon> ().changingColor = true;
		getCoinsBtn.GetComponent <ButtonIcon> ().EnableBtn (true);
		print ("OnCanTakeGift");
		timer.gameObject.SetActive (false);
		giftTitle.text = LocalizationManager.GetLocalizedText ("get_gift");
	}

	void OnDontTakeGift ()
	{
		buyCoinScreenBtn.changingColor = false;
        showMenuBtn.changingColor = false;
        getCoinsBtn.GetComponent <ButtonIcon> ().changingColor = false;
		getCoinsBtn.GetComponent <ButtonIcon> ().EnableBtn (false);
		print ("OnDontTakeGift");
		timer.gameObject.SetActive (true);
		giftTitle.text = LocalizationManager.GetLocalizedText ("gift_through");
	}

    public void BackBtn()
    {
        ScreenController.Ins.ActivateScreen(ScreenController.GameScreen.Menu);
    }


}

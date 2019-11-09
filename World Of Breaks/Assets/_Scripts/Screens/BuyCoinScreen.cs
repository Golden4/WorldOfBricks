using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;

public class BuyCoinScreen : ScreenBase<BuyCoinScreen>
{

    public BuyCoinItem[] buyCoinBtns;

    public ButtonIcon buyCoinScreenBtn;
    public ButtonIcon showMenuBtn;

    public Text giftTitle;

    public override void OnActivate()
    {
        base.OnActivate();

        for (int i = 0; i < buyCoinBtns.Length; i++)
        {
            string text = PurchaseManager.Ins.GetLocalizedPrice(buyCoinBtns[i].productID);
            if (text == "null")
            {
                buyCoinBtns[i].loadingImage.gameObject.SetActive(true);
                buyCoinBtns[i].priceText.gameObject.SetActive(false);
            }
            else
            {
                buyCoinBtns[i].loadingImage.gameObject.SetActive(false);
                buyCoinBtns[i].priceText.gameObject.SetActive(true);
                buyCoinBtns[i].priceText.text = text;
            }
            int index = i;

            buyCoinBtns[i].btn.onClick.RemoveAllListeners();

            buyCoinBtns[i].btn.onClick.AddListener(() => BuyItem(buyCoinBtns[index].productID));

            buyCoinBtns[i].btn.transform.DOKill();
            buyCoinBtns[i].btn.transform.DOScale(1, .5f).ChangeStartValue(Vector3.zero).ChangeEndValue(Vector3.one).SetDelay(.07f * i).SetEase(Ease.OutCubic).SetAutoKill(false);

        }

        getCoinsBtn.transform.DOKill();
        getCoinsBtn.transform.DOScale(1, .5f).ChangeStartValue(Vector3.zero).ChangeEndValue(Vector3.one).SetEase(Ease.OutCubic);
    }

    public void ShowBuyCoinScreen()
    {
        ScreenController.Ins.ActivateScreen(ScreenController.GameScreen.BuyCoin);
    }

    public void BuyItem(string id)
    {
        if (PurchaseManager.Ins.IsInitialized())
        {
            PurchaseManager.Ins.BuyConsumable(id);
        }
        else
        {
            MessageBox.ShowStatic("Failed", MessageBox.BoxType.Failed);
        }
    }

    [System.Serializable]
    public class BuyCoinItem
    {
        public string productID;
        public Button btn;
        public Text priceText;
        public Image loadingImage;
        public int coinCount;
    }

    public Button getCoinsBtn;

    public Text timer;

    static TimeSpan nextGiftTime = new TimeSpan(0, 10, 0);

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

    public override void OnInit()
    {
        base.OnInit();

        //if (PlayerPrefs.HasKey ("giftTime"))
        //nextGiveGiftTime = new DateTime (long.Parse (PlayerPrefs.GetString ("giftTime")));
        getCoinsBtn.onClick.RemoveAllListeners();
        getCoinsBtn.onClick.AddListener(delegate
        {
            GiveGift(15, getCoinsBtn.transform.position);
        });

        if (CanTakeGift())
        {
            OnCanTakeGift();
        }
        else
        {
            OnDontTakeGift();
        }


        PurchaseManager.OnPurchaseConsumable += OnPurchaseConsumable;

    }

    public override void OnCleanUp()
    {
        base.OnCleanUp();
        PurchaseManager.OnPurchaseConsumable -= OnPurchaseConsumable;
    }

    void OnPurchaseConsumable(UnityEngine.Purchasing.PurchaseEventArgs args)
    {
        List<BuyCoinItem> list = new List<BuyCoinItem>(buyCoinBtns);
        int index = list.FindIndex(x => x.productID == args.purchasedProduct.definition.id);

        int coinAmount = list[index].coinCount;

        Vector3 fromPos = list[index].btn.transform.position;
        Vector3 toPos = CoinUI.Ins.coinImage.transform.position;

        Utility.CoinsAnimateRadial(CoinUI.Ins, CoinUI.Ins.coinImage.gameObject, CoinUI.Ins.transform, coinAmount / 20, fromPos, toPos, Screen.width / 3, .5f, CoinUI.Ins.curve, () =>
        {

        });

        User.AddCoin(coinAmount);

        Utility.Invoke(CoinUI.Ins, .9f, delegate
        {
            if (CoinUI.Ins != null)
                CoinUI.Ins.AddCoin(coinAmount);
        }, true);
    }

    public static void GiveGift(int coinAmount, Vector3 fromPos)
    {
        if (CanTakeGift())
        {

            nextGiveGiftTime = DateTime.Now.Ticks + nextGiftTime.Ticks;

            //PlayerPrefs.SetString ("giftTime", nextGiveGiftTime.Ticks.ToString ());


            Vector3 toPos = CoinUI.Ins.coinImage.transform.position;

            Utility.CoinsAnimateRadial(CoinUI.Ins, CoinUI.Ins.coinImage.gameObject, CoinUI.Ins.transform, coinAmount, fromPos, toPos, Screen.width / 3, .5f, CoinUI.Ins.curve, () =>
            {

            });

            User.AddCoin(coinAmount);

            Utility.Invoke(CoinUI.Ins, .9f, delegate
            {
                if (CoinUI.Ins != null)
                    CoinUI.Ins.AddCoin(coinAmount);
            }, true);
        }
    }


    public static bool CanTakeGift()
    {
        return nextGiveGiftTime < DateTime.Now.Ticks;
    }

    public static TimeSpan timeToGiveGift
    {
        get
        {
            return new DateTime(nextGiveGiftTime - DateTime.Now.Ticks).TimeOfDay;
        }
    }

    bool lastOnTakeGiftBool;

    void Update()
    {
        if ((nextGiveGiftTime - DateTime.Now.Ticks) > 0)
        {

            if (!lastOnTakeGiftBool)
            {
                lastOnTakeGiftBool = true;
                OnDontTakeGift();
            }

            timer.text = string.Format("{0}", timeToGiveGift).Split('.')[0];
        }
        else if (lastOnTakeGiftBool)
        {
            lastOnTakeGiftBool = false;
            OnCanTakeGift();
        }
    }

    void OnCanTakeGift()
    {
        if (buyCoinScreenBtn != null)
            buyCoinScreenBtn.changingColor = true;
        if (showMenuBtn != null)
            showMenuBtn.changingColor = true;
        getCoinsBtn.GetComponent<ButtonIcon>().changingColor = true;
        getCoinsBtn.GetComponent<ButtonIcon>().EnableBtn(true);
        timer.gameObject.SetActive(false);
        giftTitle.text = LocalizationManager.GetLocalizedText("get_gift");
    }

    void OnDontTakeGift()
    {
        if (buyCoinScreenBtn != null)
            buyCoinScreenBtn.changingColor = false;

        if (showMenuBtn != null)
            showMenuBtn.changingColor = false;

        getCoinsBtn.GetComponent<ButtonIcon>().changingColor = false;
        getCoinsBtn.GetComponent<ButtonIcon>().EnableBtn(false);
        timer.gameObject.SetActive(true);
        giftTitle.text = LocalizationManager.GetLocalizedText("gift_through");
    }

    public void BackBtn()
    {
        if (ScreenController.Ins.curScene == ScreenController.CurScene.Menu)
            ScreenController.Ins.ActivateScreen(ScreenController.GameScreen.Menu);

        if (ScreenController.Ins.curScene == ScreenController.CurScene.Game)
            ScreenController.Ins.ActivateScreen(ScreenController.GameScreen.Pause);
    }


}

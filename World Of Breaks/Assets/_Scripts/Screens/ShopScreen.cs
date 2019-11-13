using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;

public class ShopScreen : ScreenBase<ShopScreen>
{

    public GameObject itemsHolder;

    public ScrollSnap scrollSnap;

    public int curActiveItem
    {
        get
        {
            return scrollSnap.GetCurItemIndex;
        }
    }

    public int ItemCount
    {
        get
        {
            return scrollSnap.items.Length;
        }
    }

    public Text itemNameText;
    public Text itemAbilityText;

    public Button SelectBtn;

    public Button buyPaidBtn;
    public Button buyCoinBtn;
    public Button buyVideoBtn;

    public override void OnInit()
    {
        scrollSnap.Init();
        scrollSnap.OnChangeItemEvent += OnChangeItem;

        SelectBtn.onClick.RemoveAllListeners();
        SelectBtn.onClick.AddListener(() =>
        {
            Select(curActiveItem);
        });

        buyPaidBtn.onClick.RemoveAllListeners();
        buyPaidBtn.onClick.AddListener(() =>
        {
            BuyPaidItem(curActiveItem);
        });

        buyCoinBtn.onClick.RemoveAllListeners();
        buyCoinBtn.onClick.AddListener(() =>
        {
            BuyCoinItem(curActiveItem);
        });

        buyVideoBtn.onClick.RemoveAllListeners();
        buyVideoBtn.onClick.AddListener(() =>
        {
            BuyVideoItem(curActiveItem);
        });

        for (int i = 0; i < ItemCount; i++)
        {
            scrollSnap.SetItemState(i, User.GetInfo.userData[i].bought);
        }


        PurchaseManager.OnPurchaseNonConsumable += BuyPaidItemSuccess;
    }

    public override void OnActivate()
    {
        for (int i = 0; i < ItemCount; i++)
        {
            scrollSnap.SetItemState(i, User.GetInfo.userData[i].bought);
        }

        UpdateItemState(curActiveItem);
        scrollSnap.SnapToObj(User.GetInfo.GetCurPlayerIndex(), false);
    }

    void onRewardedVideoFinishedEvent()
    {
        if (videoItemindex >= 0)
        {
            BuyItemSuccess(videoItemindex);
            MessageBox.HideAll();
            clicked = false;
            AdManager.onRewardedVideoFinishedEvent -= onRewardedVideoFinishedEvent;
        }
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();

    }

    public override void OnCleanUp()
    {
        scrollSnap.OnChangeItemEvent -= OnChangeItem;
        PurchaseManager.OnPurchaseNonConsumable -= BuyPaidItemSuccess;
    }

    void OnChangeItem(int index)
    {
        itemNameText.text = LocalizationManager.GetLocalizedText(ItemsInfo.Get.playersData[index].name);
        itemAbilityText.text = ItemsInfo.Get.playersData[index].GetDescription();

        UpdateItemState(index);
    }

    public void Select(int index)
    {
        User.SetPlayerIndex(index);
        Game.ballTryingIndex = -1;
        OnChangeItem(index);
        // ScreenController.Ins.ActivateScreen(ScreenController.GameScreen.Menu);
    }

    public void BuyPaidItem(int index)
    {
        if (PurchaseManager.Ins.IsInitialized())
        {
            PurchaseManager.Ins.BuyNonConsumable(index);
        }
        else
        {
            MessageBox.ShowStatic("Failed", MessageBox.BoxType.Failed);
        }
    }

    public void BuyCoinItem(int index)
    {

        if (string.IsNullOrEmpty(ItemsInfo.Get.playersData[index].price))
        {
            Debug.LogError("Not set coinAmount " + index);
            return;
        }

        int coinAmount = int.Parse(ItemsInfo.Get.playersData[index].price);

        if (User.BuyWithCoin(coinAmount))
        {
            BuyItemSuccess(index);
        }
    }

    int videoItemindex = -1;
    bool clicked;

    public void BuyVideoItem(int index)
    {
        videoItemindex = index;
        if (AdManager.Ins != null)
        {
            AdManager.Ins.showRewardedVideo();
            if (!clicked)
            {
                clicked = true;
                AdManager.onRewardedVideoFinishedEvent += onRewardedVideoFinishedEvent;
            }
        }
    }

    public void BuyPaidItemSuccess(PurchaseEventArgs args)
    {
        string purchID = args.purchasedProduct.definition.id;

        int index = 0;

        for (int i = 0; i < ItemsInfo.Get.playersData.Length; i++)
        {
            if (purchID == ItemsInfo.Get.playersData[i].purchaseID)
            {
                index = i;
                break;
            }
        }

        Debug.Log("You bought " + purchID + "  id " + index + " NonCon");
        BuyItemSuccess(index);
    }

    void BuyItemSuccess(int index)
    {
        User.GetInfo.userData[index].bought = true;
        UpdateItemState(index);
        scrollSnap.SetItemState(index, User.GetInfo.userData[index].bought);
        User.SaveUserInfo();
    }

    int lastItemIndex = -1;

    public void UpdateItemState(int index)
    {
        bool bought = User.GetInfo.userData[index].bought;

        if (lastItemIndex >= 0)
        {
            scrollSnap.items[lastItemIndex].GetComponent<Animation>().Stop();
            scrollSnap.items[lastItemIndex].transform.localPosition = Vector3.right * scrollSnap.distanceItems * lastItemIndex;
        }
        if (bought)
        {
            scrollSnap.items[index].GetComponent<Animation>().enabled = true;
            scrollSnap.items[index].GetComponent<Animation>().Play();
        }

        lastItemIndex = index;

        if (!bought)
        {
            string text = PurchaseManager.Ins.GetLocalizedPrice(ItemsInfo.Get.playersData[index].purchaseID);
            Text textC = buyPaidBtn.transform.GetChild(0).GetComponentInChildren<Text>();
            Image loading = buyPaidBtn.transform.GetChild(1).GetComponentInChildren<Image>();
            buyPaidBtn.gameObject.SetActive(true);

            if (text != "null")
            {
                loading.gameObject.SetActive(false);
                textC.gameObject.SetActive(true);
                textC.text = text;
            }
            else
            {
                loading.gameObject.SetActive(true);
                textC.gameObject.SetActive(false);
            }

        }
        else
        {
            buyPaidBtn.gameObject.SetActive(false);
        }

        SelectBtn.gameObject.SetActive(bought);

        if (bought)
        {
            if (index == User.GetInfo.GetCurPlayerIndex())
            {
                SelectBtn.gameObject.GetComponentInChildren<Text>().text = "Selected";
                SelectBtn.GetComponent<Image>().color = new Color(0.6128193f, 0.867f, 0.1253494f);
                SelectBtn.interactable = false;
            }
            else
            {
                SelectBtn.gameObject.GetComponentInChildren<Text>().text = "Select";
                SelectBtn.GetComponent<Image>().color = new Color(0.6901961f, 0.9764706f, 0.1411765f);
                SelectBtn.interactable = true;
            }
        }

        ShowBuyBtb(!bought, ItemsInfo.Get.playersData[index].buyType, index);

    }

    Button ShowBuyBtb(bool show, ItemsInfo.BuyType type = ItemsInfo.BuyType.Coin, int indexPlayer = -1)
    {
        Button[] btns = new Button[] {
            buyCoinBtn,
            buyVideoBtn
        };

        for (int i = 0; i < btns.Length; i++)
        {
            if (show)
            {
                btns[i].gameObject.SetActive((int)type == i);
            }
            else
                btns[i].gameObject.SetActive(false);
        }

        if (!show)
            return null;

        switch (type)
        {
            case ItemsInfo.BuyType.Coin:
                btns[(int)type].GetComponentInChildren<Text>().text = ItemsInfo.Get.playersData[indexPlayer].price;
                break;
            case ItemsInfo.BuyType.Video:

                break;

            default:
                break;
        }

        return btns[(int)type];

    }

    public void BackBtn()
    {
        ScreenController.Ins.ActivateScreen(ScreenController.GameScreen.TileSize);

    }

}

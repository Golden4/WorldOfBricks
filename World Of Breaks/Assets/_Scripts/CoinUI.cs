using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CoinUI : MonoBehaviour
{
    public static CoinUI Ins;
    public Text coinText;
    Color origColor;

    public Image coinImage;
    public Text plusCoinText;
    public AnimationCurve curve;

    public int coinsUI;

    void Awake()
    {
        Ins = this;
        coinsUI = User.Coins;
    }

    private void Start()
    {
        ShowCoinCount(User.Coins, User.Coins);
        origColor = coinText.color;
        ButtonCustom buttonCustom = GetComponent<ButtonCustom>();

        if (buttonCustom != null)
            buttonCustom.onClick += delegate
              {
                  BottomPanelMenuUI.Ins.ChangeBtn(0);
              };

        //User.OnCoinChangedEvent += ShowCoinCount;
        User.OnCoinChangedFailedEvent += BuyFailed;
    }

    /*	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Space)) {
			User.AddCoin (Random.Range (10, 100));
		}
	}*/


    float lastShowCountTime;
    float changingValue;

    private void Update()
    {
        if (lastShowCountTime + 1 < Time.time && changingValue != 0)
        {
            changingValue = 0;
            plusCoinText.rectTransform.DOKill();
            plusCoinText.rectTransform.DOAnchorPosY(0, .5f).OnComplete(() =>
            {
                plusCoinText.rectTransform.gameObject.SetActive(false);
            });
        }
    }

    void OnDestroy()
    {
        //User.OnCoinChangedEvent -= ShowCoinCount;
        User.OnCoinChangedFailedEvent -= BuyFailed;
    }

    void BuyFailed(int coin)
    {
        coinText.color = Color.red;
        coinText.transform.localScale = Vector3.one * 1.1f;

        // MessageBox.ShowStatic("Buy coins?", MessageBox.BoxType.Retry).SetTextBtn("Ok", true, () =>
        // {
        //     ScreenController.Ins.ActivateScreen(ScreenController.GameScreen.BuyCoin);
        // });

        StopCoroutine("ChangeColorCorutine");
        StartCoroutine("ChangeColorCorutine");
    }

    IEnumerator ChangeColorCorutine()
    {
        yield return new WaitForSeconds(.5f);
        coinText.transform.localScale = Vector3.one;
        coinText.color = origColor;
    }

    void ShowCoinCount(int fromValue, int toValue)
    {
        coinText.gameObject.SetActive(true);

        if ((fromValue - toValue) != 0)
        {
            lastShowCountTime = Time.time;
            plusCoinText.gameObject.SetActive(true);

            if (changingValue == 0)
            {
                plusCoinText.rectTransform.DOKill();
                plusCoinText.rectTransform.DOAnchorPosY(-45, .5f);
            }

            changingValue += (toValue - fromValue);

            if (changingValue > 0)
                AudioManager.PlaySoundFromLibrary("Coin");

            plusCoinText.GetComponent<Text>().text = ((changingValue >= 0) ? "+" : "") + changingValue;
        }

        if (Mathf.Abs(toValue - fromValue) > 1)
        {
            Utility.AnimateValue(coinText, fromValue, toValue, 2);
        }
        else
        {
            coinText.text = toValue.ToString();
        }

        //Utility.CoinsAnimate (this, coinImage.gameObject, transform, 5, transform.position - Vector3.down * 100, transform.position, .1f);

    }

    public void AddCoin(int value)
    {
        ShowCoinCount(coinsUI, (coinsUI + value));

        coinsUI += value;
    }

}

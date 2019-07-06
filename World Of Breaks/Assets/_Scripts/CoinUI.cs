using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinUI : MonoBehaviour {
	public static CoinUI Ins;
	public Text coinText;
    Color origColor;

	public Image coinImage;
    public GUIAnim plusCoinText;
    public AnimationCurve curve;

	void Awake ()
	{
		Ins = this;
	}

    private void Start()
    {
        ShowCoinCount(User.Coins, User.Coins);
        origColor = coinText.color;
        User.OnCoinChangedEvent += ShowCoinCount;
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
        if(lastShowCountTime + 1 < Time.time && changingValue != 0)
        {
            changingValue = 0;
            plusCoinText.MoveOut(GUIAnimSystem.eGUIMove.Self);
        }
    }

    void OnDestroy ()
	{
		User.OnCoinChangedEvent -= ShowCoinCount;
        User.OnCoinChangedFailedEvent -= BuyFailed;
    }

    void BuyFailed(int coin)
    {
        coinText.color = Color.red;
        coinText.transform.localScale = Vector3.one * 1.1f;
        StopCoroutine("ChangeColorCorutine");
        StartCoroutine("ChangeColorCorutine");
    }

    IEnumerator ChangeColorCorutine()
    {
        yield return new WaitForSeconds(.5f);
        coinText.transform.localScale = Vector3.one;
        coinText.color = origColor;
    }
    

	void ShowCoinCount (int fromValue, int toValue)
	{
		coinText.gameObject.SetActive (true);
        
        if ((fromValue - toValue) != 0)
        {
            lastShowCountTime = Time.time;
            plusCoinText.gameObject.SetActive(true);

                //plusCoinText.m_MoveIn.Actions.OnEnd.RemoveAllListeners();
                //plusCoinText.m_MoveIn.Actions.OnEnd.AddListener(delegate
                //{
                //    plusCoinText.MoveOut(GUIAnimSystem.eGUIMove.Self);
                //});

            if(changingValue==0)
                plusCoinText.MoveIn(GUIAnimSystem.eGUIMove.Self);

            changingValue += (toValue - fromValue);

            plusCoinText.GetComponent<Text>().text = ((changingValue >= 0) ? "+" : "") + changingValue;
        }

        if (Mathf.Abs (toValue - fromValue) > 1) {
			Utility.AnimateValue (coinText, fromValue, toValue, 2);
		} else {
			coinText.text = toValue.ToString ();
		}

		//Utility.CoinsAnimate (this, coinImage.gameObject, transform, 5, transform.position - Vector3.down * 100, transform.position, .1f);

	}

}

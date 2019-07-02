using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinUI : MonoBehaviour {
	public static CoinUI Ins;
	Text coinText;

	public Image coinImage;

	public AnimationCurve curve;

	void Awake ()
	{
		Ins = this;
		coinText = GetComponent <Text> ();

		ShowCoinCount (User.Coins, User.Coins);
		User.OnCoinChangedEvent += ShowCoinCount;

	}

    /*	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Space)) {
			User.AddCoin (Random.Range (10, 100));
		}
	}*/

    void OnDestroy ()
	{
		User.OnCoinChangedEvent -= ShowCoinCount;
	}

	void ShowCoinCount (int fromValue, int toValue)
	{
		coinText.gameObject.SetActive (true);

		if (Mathf.Abs (toValue - fromValue) > 1) {
			Utility.AnimateValue (coinText, fromValue, toValue, 2);
		} else {
			coinText.text = toValue.ToString ();
		}

		//Utility.CoinsAnimate (this, coinImage.gameObject, transform, 5, transform.position - Vector3.down * 100, transform.position, .1f);

	}

}

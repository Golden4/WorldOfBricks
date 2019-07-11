using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCoinsBtn : MonoBehaviour {
	
	void Start ()
	{
		AdManager.onRewardedVideoFinishedEvent += onRewardedVideoFinishedEvent;
	}

	void onRewardedVideoFinishedEvent ()
	{
		int coinAmount = 5;
		User.AddCoin (coinAmount);

		Vector3 fromPos = transform.position;
		Vector3 toPos = CoinUI.Ins.coinImage.transform.position;

		Utility.CoinsAnimate (CoinUI.Ins, CoinUI.Ins.coinImage.gameObject, CoinUI.Ins.transform, coinAmount, fromPos, toPos, .5f, CoinUI.Ins.curve, () => {
			
		});
	}

	void OnDestroy ()
	{
		AdManager.onRewardedVideoFinishedEvent -= onRewardedVideoFinishedEvent;
	}
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FreeCoinsBtn : MonoBehaviour {
	Button btn;

	void Start ()
	{
		btn = GetComponent <Button> ();
		btn.onClick.RemoveAllListeners ();
		btn.onClick.AddListener (ShowAd);

	}

	bool clicked;

	void ShowAd ()
	{
		if (AdManager.Ins != null) {
			AdManager.Ins.showRewardedVideo ();

			if (!clicked) {
				clicked = true;
				AdManager.onRewardedVideoFinishedEvent += onRewardedVideoFinishedEvent;
			}
		}
	}

	void onRewardedVideoFinishedEvent ()
	{
		int coinAmount = 5;
		User.AddCoin (coinAmount);
		clicked = false;
		AdManager.onRewardedVideoFinishedEvent -= onRewardedVideoFinishedEvent;
	}

	void OnDestroy ()
	{
		
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;
using System;

public class AdManager : MonoBehaviour, IInterstitialAdListener, IRewardedVideoAdListener {

	public static AdManager Ins;

	#if UNITY_ANDROID
	public string appKey = "9fc4231d0bc2ab13fac589b653287a74135a09438613c596";
	
#else
	public string appKey = "";
	#endif

	public bool testMode = true;

	void Awake ()
	{
		if (Ins == null)
			Ins = this;
		else if (Ins != this) {
			Destroy (gameObject);
			return;
		}

		testMode = Debug.isDebugBuild;

		DontDestroyOnLoad (gameObject);
	}

	void Start ()
	{
		Appodeal.setTesting (testMode);

		Appodeal.initialize (appKey, Appodeal.INTERSTITIAL | Appodeal.BANNER_VIEW | Appodeal.REWARDED_VIDEO | Appodeal.MREC, false);
	}

	public static event Action onInterstitialClosedEvent;

	public void showInterstitial ()
	{
		if (Appodeal.isLoaded (Appodeal.INTERSTITIAL) && !Appodeal.isPrecache (Appodeal.INTERSTITIAL)) {
			Appodeal.show (Appodeal.INTERSTITIAL);
		} else {
			Appodeal.cache (Appodeal.INTERSTITIAL);
		}
	}


	public void onInterstitialLoaded (bool isPrecache)
	{
	}

	public void onInterstitialFailedToLoad ()
	{
	}

	public void onInterstitialShown ()
	{
	}

	public void onInterstitialClosed ()
	{
		if (onInterstitialClosedEvent != null)
			onInterstitialClosedEvent ();
	}

	public void onInterstitialClicked ()
	{
	}

	public void onInterstitialExpired ()
	{
	}

	public static event Action onRewardedVideoFinishedEvent;

	public void showRewardedVideo ()
	{
		Debug.Log ("Predicted eCPM for Rewarded Video: " + Appodeal.getPredictedEcpm (Appodeal.REWARDED_VIDEO));
		Debug.Log ("Reward currency: " + Appodeal.getRewardParameters ().Key + ", amount: " + Appodeal.getRewardParameters ().Value);
		if (Appodeal.canShow (Appodeal.REWARDED_VIDEO)) {
			Appodeal.show (Appodeal.REWARDED_VIDEO);
		}
	}


	public void onRewardedVideoLoaded (bool precache)
	{
	}

	public void onRewardedVideoFailedToLoad ()
	{
	}

	public void onRewardedVideoShown ()
	{
	}

	public void onRewardedVideoFinished (double amount, string name)
	{
		if (onRewardedVideoFinishedEvent != null)
			onRewardedVideoFinishedEvent ();
	}

	public void onRewardedVideoClosed (bool finished)
	{
		
	}

	public void onRewardedVideoExpired ()
	{
	}

	public void onRewardedVideoClicked ()
	{
	}

}

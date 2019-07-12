using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;
using System;

public class AdManager : MonoBehaviour, IInterstitialAdListener, IRewardedVideoAdListener {

	public static AdManager Ins {
		get {
			if (_Ins == null) {
				GameObject manager = Resources.Load ("Prefabs/AdManager") as GameObject;
				AdManager adManager = Instantiate (manager).GetComponent<AdManager> ();
				adManager.Init ();
				_Ins = adManager;
			}

			return _Ins;
		}
	}

	bool isInit;

	static AdManager _Ins;

	#if UNITY_ANDROID
	public string appKey = "9fc4231d0bc2ab13fac589b653287a74135a09438613c596";
	
	#else
	public string appKey = "";
	#endif

	public bool testMode = true;

	void Awake ()
	{
		if (_Ins == null) {
			_Ins = this;
			DontDestroyOnLoad (gameObject);
		} else if (_Ins != this) {
				Destroy (gameObject);
				return;
			}
	}

	public void Init ()
	{
		if (!isInit) {
			
			testMode = Debug.isDebugBuild;
			Appodeal.disableLocationPermissionCheck ();
			Appodeal.setTesting (testMode);
			Appodeal.initialize (appKey, Appodeal.INTERSTITIAL | Appodeal.BANNER_VIEW | Appodeal.REWARDED_VIDEO, false);
			Appodeal.setInterstitialCallbacks (this);
			Appodeal.setRewardedVideoCallbacks (this);

			isInit = true;
		}
	}

	public void Start ()
	{
		Init ();
	}

	bool isRewardedVideoFinished;
	bool isInterstitialClosed;

	void Update ()
	{
		if (isRewardedVideoFinished) {
			isRewardedVideoFinished = false;
			if (onRewardedVideoFinishedEvent != null)
				onRewardedVideoFinishedEvent ();
		}

		if (isInterstitialClosed) {
			isInterstitialClosed = false;
			if (onInterstitialClosedEvent != null)
				onInterstitialClosedEvent ();
		}
		
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
		isInterstitialClosed = true;
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
		if (Appodeal.isLoaded (Appodeal.REWARDED_VIDEO) && !Appodeal.isPrecache (Appodeal.REWARDED_VIDEO)) {
			Appodeal.show (Appodeal.REWARDED_VIDEO);
		} else {
			Appodeal.cache (Appodeal.REWARDED_VIDEO);
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
		isRewardedVideoFinished = true;
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

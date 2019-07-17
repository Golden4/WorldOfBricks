using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;
using System;

public class AdManager : SingletonResourse<AdManager>, IInterstitialAdListener, IRewardedVideoAdListener, IPermissionGrantedListener {

	#if UNITY_ANDROID
	public string appKey;
	
	#else
	public string appKey = "";
	#endif

	public bool testMode = true;

	public override void OnInit ()
	{
		DontDestroyOnLoad (gameObject);
		InitAppodeal ();
	}

	public void InitAppodeal ()
	{
		Appodeal.requestAndroidMPermissions (this);
		testMode = Debug.isDebugBuild;
		Appodeal.setTesting (testMode);
		Appodeal.initialize (appKey, Appodeal.INTERSTITIAL | Appodeal.BANNER_VIEW | Appodeal.REWARDED_VIDEO, PlayerPrefs.GetInt ("result_gdpr_sdk", 0) == 1);
		Appodeal.setInterstitialCallbacks (this);
		Appodeal.setRewardedVideoCallbacks (this);
	}

	bool isRewardedVideoFinished;
	bool isInterstitialClosed;
	public bool isRewardedVideoLoaded;

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

		Action showVideo = delegate {
			if (Appodeal.isLoaded (Appodeal.REWARDED_VIDEO) && !Appodeal.isPrecache (Appodeal.REWARDED_VIDEO)) {
				Appodeal.show (Appodeal.REWARDED_VIDEO);
			} else {
				Appodeal.cache (Appodeal.REWARDED_VIDEO);
			}
		};

		if (!ShowPrivacyPolicyDialog (showVideo)) {
			showVideo ();
		}
	}

	public void onRewardedVideoLoaded (bool precache)
	{
		isRewardedVideoLoaded = true;
	}

	public void onRewardedVideoFailedToLoad ()
	{
		isRewardedVideoLoaded = false;
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

	public bool ShowPrivacyPolicyDialog (Action afterDialog)
	{
		int consentInt = 0;

		if (PlayerPrefs.HasKey ("result_gdpr")) {
			consentInt = PlayerPrefs.GetInt ("result_gdpr");
		}


		if (consentInt == 0) {
			Time.timeScale = 0;
			DialogBox.Show (mainString, delegate {
				onYesClick ();

				if (afterDialog != null)
					afterDialog ();
			}, delegate {
				onNoClick ();

				if (afterDialog != null)
					afterDialog ();
			}, true, true, 300, "Yes, I agree", "No, thank you");
		}
		
		return consentInt == 0;
	}

	string mainString = "<size=45>Get better Ads and AWESOME REWARDS!</size>\n\nThis app personalizes your advertising experience using our partners. Our partners may collect and process personal data such as device identifiers, location data, and other demographic and interest data to provide advertising experience tailored to you. By consenting to this improved ad experience, you'll see ads that our partners believe are more relevant to you.\n\nBy agreeing, you confirm that you are over the age of 16 and would like a personalized ad experience.";

	public void onYesClick ()
	{
		PlayerPrefs.SetInt ("result_gdpr", 1);
		PlayerPrefs.SetInt ("result_gdpr_sdk", 1);
		Time.timeScale = 1;
		InitAppodeal ();
	}

	public void onNoClick ()
	{
		PlayerPrefs.SetInt ("result_gdpr", 1);
		PlayerPrefs.SetInt ("result_gdpr_sdk", 0);
		Time.timeScale = 1;
		InitAppodeal ();
	}

	public void writeExternalStorageResponse (int result)
	{
		if (result == 0) {
			Debug.Log ("WRITE_EXTERNAL_STORAGE permission granted"); 
		} else {
			Debug.Log ("WRITE_EXTERNAL_STORAGE permission grant refused"); 
		}
	}

	public void accessCoarseLocationResponse (int result)
	{
		if (result == 0) {
			Debug.Log ("ACCESS_COARSE_LOCATION permission granted"); 
		} else {
			Debug.Log ("ACCESS_COARSE_LOCATION permission grant refused"); 
		}
	}
}

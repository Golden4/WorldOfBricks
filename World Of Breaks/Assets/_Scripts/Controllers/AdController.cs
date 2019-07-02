using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*using GoogleMobileAds.Api;
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;*/
using GoogleMobileAds.Api;
using System;

public class AdController : MonoBehaviour {
	 
	public bool testMode = true;

	#if UNITY_ANDROID
	string adInterstitialID = "ca-app-pub-8878808814241755/6374843101";
	string adInterstitialAllID = "ca-app-pub-8878808814241755/2444615260";
	string adBannerID = "ca-app-pub-8878808814241755/1477739641";
	string adRewardedID = "ca-app-pub-8878808814241755/4399540162";
	string appID = "ca-app-pub-8878808814241755~7293420529";

	#else
	string adInterstitialID = "unexpected_platform";
	string adBannerID = "unexpected_platform";
	string adRewardedID = "unexpected_platform";
	string appID = "unexpected_platform";
	#endif

	public BannerView bannerView;
	public InterstitialAd interstitial;
	public InterstitialAd interstitialAll;
	public RewardedAd rewardedAd;

	public bool bannerViewLoaded;
	public bool interstitialLoaded;
	public bool interstitialAllLoaded;
	public bool rewardedAdLoaded;
	public bool needGiveReward;

	public static AdController Ins;

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
			
		MobileAds.Initialize (appID);
		SceneController.OnRestartLevel += RequestAds;
		RequestAds ();
		//	RequestBanner ();
	}

	void RequestAds ()
	{
		RequestInterstitial ();
		RequestRewardedAd ();
		RequestInterstitialAll ();

		if (GameOverScreen.playerDieCount == 2) {
			GameOverScreen.playerDieCount = 0;
			ShowInterstitialAllAD ();
		}
	}

	public void ShowInterstitialAD ()
	{
		if (interstitial.IsLoaded ()) {
			interstitial.Show ();
		}
	}

	public void ShowInterstitialAllAD ()
	{
		if (interstitialAll.IsLoaded ()) {
			interstitialAll.Show ();
		}
	}

	public void ShowBannerAD ()
	{
		RequestBanner ();
	}

	public void ShowRewardedAD ()
	{
		if (this.rewardedAd.IsLoaded ())
			this.rewardedAd.Show ();
	}

	private void RequestBanner ()
	{
		bannerView = new BannerView (adBannerID, AdSize.Banner, AdPosition.Top);

		// Called when an ad request has successfully loaded.
		bannerView.OnAdLoaded += HandleBannerOnAdLoaded;
		// Called when an ad request failed to load.
		bannerView.OnAdFailedToLoad += HandleBannerOnAdFailedToLoad;
		// Called when an ad is clicked.
		bannerView.OnAdOpening += HandleBannerOnAdOpened;
		// Called when the user returned from the app after an ad click.
		bannerView.OnAdClosed += HandleBannerOnAdClosed;
		// Called when the ad click caused the user to leave the application.
		bannerView.OnAdLeavingApplication += HandleBannerOnAdLeavingApplication;

		// Create an empty ad request.
		AdRequest request;

		if (testMode)
			request = new AdRequest.Builder ().AddTestDevice (AdRequest.TestDeviceSimulator).AddTestDevice (SystemInfo.deviceUniqueIdentifier.ToUpper ()).Build ();
		else
			request = new AdRequest.Builder ().Build ();
		
		// Load the banner with the request.
		bannerView.LoadAd (request);
	}

	public void HandleBannerOnAdLoaded (object sender, EventArgs args)
	{
		MonoBehaviour.print ("HandleAdLoaded event received");
	}

	public void HandleBannerOnAdFailedToLoad (object sender, AdFailedToLoadEventArgs args)
	{
		MonoBehaviour.print ("HandleFailedToReceiveAd event received with message: "
		+ args.Message);
	}

	public void HandleBannerOnAdOpened (object sender, EventArgs args)
	{
		MonoBehaviour.print ("HandleAdOpened event received");
	}

	public void HandleBannerOnAdClosed (object sender, EventArgs args)
	{
		MonoBehaviour.print ("HandleAdClosed event received");
	}

	public void HandleBannerOnAdLeavingApplication (object sender, EventArgs args)
	{
		MonoBehaviour.print ("HandleAdLeavingApplication event received");
	}

	private void RequestInterstitial ()
	{
		// Initialize an InterstitialAd.
		this.interstitial = new InterstitialAd (adInterstitialID);
		interstitialLoaded = false;
		// Called when an ad request has successfully loaded.
		this.interstitial.OnAdLoaded += HandleOnAdLoaded;
		// Called when an ad request failed to load.
		this.interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
		// Called when an ad is shown.
		this.interstitial.OnAdOpening += HandleOnAdOpened;
		// Called when the ad is closed.
		this.interstitial.OnAdClosed += HandleOnAdClosed;
		// Called when the ad click caused the user to leave the application.
		this.interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplication;

		// Create an empty ad request.
		AdRequest request;

		if (testMode)
			request = new AdRequest.Builder ().AddTestDevice (AdRequest.TestDeviceSimulator).AddTestDevice (SystemInfo.deviceUniqueIdentifier.ToUpper ()).Build ();
		else
			request = new AdRequest.Builder ().Build ();
		
		// Load the interstitial with the request.
		this.interstitial.LoadAd (request);
	}

	public void HandleOnAdLoaded (object sender, EventArgs args)
	{
		interstitialLoaded = true;
	}

	public void HandleOnAdFailedToLoad (object sender, AdFailedToLoadEventArgs args)
	{
		interstitialLoaded = false;
	}

	public void HandleOnAdOpened (object sender, EventArgs args)
	{
		interstitialLoaded = false;
	}

	public void HandleOnAdClosed (object sender, EventArgs args)
	{
		interstitialLoaded = false;
	}

	public void HandleOnAdLeavingApplication (object sender, EventArgs args)
	{
		MonoBehaviour.print ("HandleAdLeavingApplication event received");
	}

	private void RequestInterstitialAll ()
	{
		// Initialize an InterstitialAd.
		this.interstitialAll = new InterstitialAd (adInterstitialAllID);
		interstitialAllLoaded = false;
		// Called when an ad request has successfully loaded.
		this.interstitialAll.OnAdLoaded += HandleOnAdAllLoaded;
		// Called when an ad request failed to load.
		this.interstitialAll.OnAdFailedToLoad += HandleOnAdAllFailedToLoad;
		// Called when an ad is shown.
		this.interstitialAll.OnAdOpening += HandleOnAdAllOpened;
		// Called when the ad is closed.
		this.interstitialAll.OnAdClosed += HandleOnAdAllClosed;
		// Called when the ad click caused the user to leave the application.
		this.interstitialAll.OnAdLeavingApplication += HandleOnAdAllLeavingApplication;
		// Create an empty ad request.
		AdRequest request;

		if (testMode)
			request = new AdRequest.Builder ().AddTestDevice (AdRequest.TestDeviceSimulator).AddTestDevice (SystemInfo.deviceUniqueIdentifier.ToUpper ()).Build ();
		else
			request = new AdRequest.Builder ().Build ();

		// Load the interstitial with the request.
		this.interstitialAll.LoadAd (request);
	}

	public void HandleOnAdAllLoaded (object sender, EventArgs args)
	{
		interstitialAllLoaded = true;
	}

	public void HandleOnAdAllFailedToLoad (object sender, AdFailedToLoadEventArgs args)
	{
		interstitialAllLoaded = false;
	}

	public void HandleOnAdAllOpened (object sender, EventArgs args)
	{
		interstitialAllLoaded = false;
		MonoBehaviour.print ("HandleAdOpened event received");
	}

	public void HandleOnAdAllClosed (object sender, EventArgs args)
	{
		MonoBehaviour.print ("HandleAdClosed event received");
	}

	public void HandleOnAdAllLeavingApplication (object sender, EventArgs args)
	{
		MonoBehaviour.print ("HandleAdLeavingApplication event received");
	}

	public void RequestRewardedAd ()
	{

		this.rewardedAd = new RewardedAd (adRewardedID);
		rewardedAdLoaded = false;
		// Called when an ad request has successfully loaded.
		this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
		// Called when an ad request failed to load.
		this.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
		// Called when an ad is shown.
		this.rewardedAd.OnAdOpening += HandleRewardedAdOpening;
		// Called when an ad request failed to show.
		this.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
		// Called when the user should be rewarded for interacting with the ad.
		this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
		// Called when the ad is closed.
		this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

		// Create an empty ad request.
		AdRequest request;

		if (testMode)
			request = new AdRequest.Builder ().AddTestDevice (AdRequest.TestDeviceSimulator).AddTestDevice (SystemInfo.deviceUniqueIdentifier.ToUpper ()).Build ();
		else
			request = new AdRequest.Builder ().Build ();
		// Load the rewarded ad with the request.
		this.rewardedAd.LoadAd (request);
	}

	public void HandleRewardedAdLoaded (object sender, EventArgs args)
	{
		rewardedAdLoaded = true;
	}

	public void HandleRewardedAdFailedToLoad (object sender, AdErrorEventArgs args)
	{
		rewardedAdLoaded = false;
	}

	public void HandleRewardedAdOpening (object sender, EventArgs args)
	{
		rewardedAdLoaded = false;
	}

	public void HandleRewardedAdFailedToShow (object sender, AdErrorEventArgs args)
	{
		MonoBehaviour.print (
			"HandleRewardedAdFailedToShow event received with message: "
			+ args.Message);
	}

	public void HandleRewardedAdClosed (object sender, EventArgs args)
	{
		MonoBehaviour.print ("HandleRewardedAdClosed event received");
	}

	public void HandleUserEarnedReward (object sender, Reward args)
	{
		
		/*string type = args.Type;
		double amount = args.Amount;*/
		needGiveReward = true;

		/*MonoBehaviour.print (
			"HandleRewardedAdRewarded event received for "
			+ amount.ToString () + " " + type);*/
	}

	void OnDestroy ()
	{
		SceneController.OnRestartLevel -= RequestAds;
	}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;
using System;

public class AdManager : SingletonResourse<AdManager>, IInterstitialAdListener, IRewardedVideoAdListener, IBannerAdListener
{

#if UNITY_ANDROID
    public string appKey;

#else
	public string appKey = "";
#endif

    public bool testMode = true;

    public override void OnInit()
    {
        DontDestroyOnLoad(gameObject);
        InitAppodeal();
    }

    public void InitAppodeal()
    {
        Appodeal.disableNetwork("facebook");
        testMode = Debug.isDebugBuild;
        Appodeal.setTesting(testMode);
        Appodeal.muteVideosIfCallsMuted(true);

        Appodeal.initialize(appKey, Appodeal.INTERSTITIAL | Appodeal.BANNER_VIEW | Appodeal.REWARDED_VIDEO, PlayerPrefs.GetInt("result_gdpr_sdk", 0) == 1);
        Appodeal.setInterstitialCallbacks(this);
        Appodeal.setRewardedVideoCallbacks(this);
        Appodeal.setBannerCallbacks(this);
    }

    bool isRewardedVideoFinished;
    bool isInterstitialClosed;
    public bool isRewardedVideoLoaded;

    void Update()
    {
        if (isRewardedVideoFinished)
        {
            isRewardedVideoFinished = false;
            if (onRewardedVideoFinishedEvent != null)
                onRewardedVideoFinishedEvent();
        }

        if (isInterstitialClosed)
        {
            isInterstitialClosed = false;
            if (onInterstitialClosedEvent != null)
                onInterstitialClosedEvent();
        }
    }

    public void showBanner()
    {
        if (Appodeal.isLoaded(Appodeal.BANNER_BOTTOM))
            Appodeal.show(Appodeal.BANNER_BOTTOM);
    }

    public void hideBanner()
    {
        Appodeal.hide(Appodeal.BANNER_BOTTOM);
    }


    public void onBannerLoaded(bool isPrecache)
    {
    }

    public void onBannerFailedToLoad()
    {
    }

    public void onBannerShown()
    {
    }

    public void onBannerClicked()
    {
    }

    public void onBannerExpired()
    {
    }

    public static event Action onInterstitialClosedEvent;

    public void showInterstitial()
    {
        if (Appodeal.isLoaded(Appodeal.INTERSTITIAL) && !Appodeal.isPrecache(Appodeal.INTERSTITIAL))
        {
            Appodeal.show(Appodeal.INTERSTITIAL);
        }
        else
        {
            Appodeal.cache(Appodeal.INTERSTITIAL);
        }
    }


    public void onInterstitialLoaded(bool isPrecache)
    {
    }

    public void onInterstitialFailedToLoad()
    {

    }

    public void onInterstitialShown()
    {
    }

    public void onInterstitialClosed()
    {
        isInterstitialClosed = true;
    }

    public void onInterstitialClicked()
    {
    }

    public void onInterstitialExpired()
    {
    }

    public static event Action onRewardedVideoFinishedEvent;

    public void showRewardedVideo()
    {

        Action showVideo = delegate
        {
            if (Appodeal.isLoaded(Appodeal.REWARDED_VIDEO) && !Appodeal.isPrecache(Appodeal.REWARDED_VIDEO))
            {
                Appodeal.show(Appodeal.REWARDED_VIDEO);
            }
            else
            {
                Appodeal.cache(Appodeal.REWARDED_VIDEO);
            }

            if (isRewardedVideoLoaded)
                MessageBox.ShowStatic("Loading video...", MessageBox.BoxType.Retry);

            // DialogBox.Show("Loading video...", null, null, false, true);

            else

                MessageBox.ShowStatic("Video failed to load...", MessageBox.BoxType.Failed)
                .SetDesc("Please check internet connection...");
            // DialogBox.Show("Video failed to load...", null, null, true, false);
        };

        if (!ShowPrivacyPolicyDialog(showVideo))
        {
            showVideo();
        }
    }

    public void onRewardedVideoLoaded(bool precache)
    {
        isRewardedVideoLoaded = true;
    }

    public void onRewardedVideoFailedToLoad()
    {
        isRewardedVideoLoaded = false;
    }

    public void onRewardedVideoShown()
    {
    }

    public void onRewardedVideoFinished(double amount, string name)
    {
        isRewardedVideoFinished = true;
    }

    public void onRewardedVideoClosed(bool finished)
    {
    }

    public void onRewardedVideoExpired()
    {
    }

    public void onRewardedVideoClicked()
    {
    }

    public bool ShowPrivacyPolicyDialog(Action afterDialog)
    {
        int consentInt = 0;

        if (PlayerPrefs.HasKey("result_gdpr"))
        {
            consentInt = PlayerPrefs.GetInt("result_gdpr");
        }

        if (consentInt == 0)
        {
            Time.timeScale = 0;
            MessageBox.ShowStatic("Get better Ads and AWESOME REWARDS!", MessageBox.BoxType.Info)
            .SetDesc(mainString)
            .SetTextBtn("Yes, I agree", true, delegate
            {
                onYesClick();

                if (afterDialog != null)
                    afterDialog();
            })
            .SetTextBtn("No, thank you", true, delegate
            {
                onNoClick();

                if (afterDialog != null)
                    afterDialog();
            });
        }

        return consentInt == 0;
    }

    string mainString = "This app personalizes your advertising experience using our partners. Our partners may collect and process personal data such as device identifiers, location data, and other demographic and interest data to provide advertising experience tailored to you. By consenting to this improved ad experience, you'll see ads that our partners believe are more relevant to you.\n\nBy agreeing, you confirm that you are over the age of 16 and would like a personalized ad experience.";

    public void onYesClick()
    {
        PlayerPrefs.SetInt("result_gdpr", 1);
        PlayerPrefs.SetInt("result_gdpr_sdk", 1);
        InitAppodeal();
    }

    public void onNoClick()
    {
        PlayerPrefs.SetInt("result_gdpr", 1);
        PlayerPrefs.SetInt("result_gdpr_sdk", 0);
        InitAppodeal();
    }
}

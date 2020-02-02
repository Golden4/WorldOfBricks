using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuScreen : ScreenBase<MenuScreen>
{

    public static event System.Action OnStartGame;

    public Button startGameBtn;

    public Text gameTitleText;
    public Image menuBall;

    public override void OnInit()
    {
        base.OnInit();
    }

    void Start()
    {
        ShowGameTitle(true, true);
        //if (AdController.Ins.RewardedADLoaded ()) {

        /*freeCoinsBtn.onClick.RemoveAllListeners ();
		freeCoinsBtn.onClick.AddListener (GetFreeCoins);*/
        /*} else {
			freeCoinsBtn.gameObject.SetActive (false);
		}*/

        startGameBtn.onClick.RemoveAllListeners();
        startGameBtn.onClick.AddListener(ShowTileSizeScreen);
        //freeCoinsBtn.gameObject.GetComponent<ButtonIcon> ().EnableBtn (true);

    }

    void Update()
    {
        /*		if (AdController.Ins.needGiveReward) {
                    AdController.Ins.needGiveReward = false;

                    int coinAmount = 15;
                    User.AddCoin (coinAmount);

                    Vector3 fromPos = freeCoinsBtn.transform.position;
                    Vector3 toPos = CoinUI.Ins.coinImage.transform.position;

                    Utility.CoinsAnimate (CoinUI.Ins, CoinUI.Ins.coinImage.gameObject, CoinUI.Ins.transform, coinAmount, fromPos, toPos, .5f, CoinUI.Ins.curve, () => {
                        AudioManager.PlaySoundFromLibrary ("Coin");
                    });
                }*/

        //if (!AdController.Ins.rewardedAdLoaded) {
        //	if (freeCoinsBtn.gameObject.activeInHierarchy)
        //		freeCoinsBtn.gameObject.SetActive (false);
        //} else {
        //	if (!freeCoinsBtn.gameObject.activeInHierarchy)
        //		freeCoinsBtn.gameObject.SetActive (true);
        //}
    }

    /*void Update ()
	{
		#if UNITY_EDITOR || UNITY_STANDALONE_WIN

		if (Input.anyKeyDown && !EventSystem.current.IsPointerOverGameObject ()) {
			
			StartGame ();
		}
		#else
		if (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Began) {
			if (!EventSystem.current.IsPointerOverGameObject (Input.GetTouch (0).fingerId))
				StartGame ();
		}

		#endif
	}*/

    public void ShowGameTitle(bool show, bool fade)
    {
        gameTitleText.enabled = true;

        if (fade)
        {
            if (show)
            {
                gameTitleText.GetComponent<GUIAnim>().MoveIn();
            }
            else
            {
                gameTitleText.GetComponent<GUIAnim>().MoveOut();
            }
        }
        else
        {
            if (show)
            {

                gameTitleText.enabled = true;
            }
            else
            {

                gameTitleText.enabled = false;
            }
        }
    }

    void GetFreeCoins()
    {
        if (AdManager.Ins != null)
        {
            AdManager.Ins.showRewardedVideo();
        }
        //freeCoinsBtn.gameObject.GetComponent<ButtonIcon> ().EnableBtn (false);


        /*		int coinAmount = 10;
                User.AddCoin (coinAmount);

                Vector3 fromPos = MenuScreen.Ins.freeCoinsBtn.transform.position;
                Vector3 toPos = CoinUI.Ins.coinImage.transform.position;

                Utility.CoinsAnimate (CoinUI.Ins, CoinUI.Ins.coinImage.gameObject, CoinUI.Ins.transform, coinAmount, fromPos, toPos, .5f, CoinUI.Ins.curve, () => {
                    AudioManager.PlaySoundFromLibrary ("Coin");
                });*/

    }


    public TitleColor[] titleColors;

    [System.Serializable]
    public class TitleColor
    {
        public Color main;
        public Color outline;
        public Color shadow;
    }

    public override void OnActivate()
    {
        base.OnActivate();

        if (titleColors.Length > 0)
        {

            TitleColor color = titleColors[Random.Range(0, titleColors.Length)];
            gameTitleText.color = color.main;
            color.shadow.a = .7f;
            gameTitleText.GetComponent<Shadow>().effectColor = color.shadow;
            //gameTitleText.GetComponent <Outline> ().effectColor = color.outline;
        }

        ShowGameTitle(true, true);
        //freeCoinsBtn.gameObject.SetActive (AdController.Ins.rewardedAdLoaded);
        menuBall.sprite = ItemsInfo.Get.playersData[User.GetInfo.GetCurPlayerIndex()].ballSprite;

        int _checkpoint = 0;

        if (PlayerPrefs.HasKey("Checkpoint"))
            _checkpoint = PlayerPrefs.GetInt("Checkpoint");
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();

        ShowGameTitle(false, false);
    }

    public void ShowTileSizeScreen()
    {
        ScreenController.Ins.ActivateScreen("TileSize");
    }

    public void ShowShopScreen()
    {
        ScreenController.Ins.ActivateScreen("Shop");
    }

    public void ShowChallengesScreen()
    {
        ScreenController.Ins.ActivateScreen("Challeges");
    }
}

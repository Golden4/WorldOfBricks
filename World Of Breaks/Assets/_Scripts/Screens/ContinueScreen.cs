using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContinueScreen : ScreenBase {
	[SerializeField] Transform continueAdPanel;
	[SerializeField] Button continueAdBtn;
	[SerializeField] Button continueCoinBtn;

	[SerializeField] Image continueTimer;
	float lastTime;
	float waitTime = 5;

	public bool givedSecondChance = false;

	public override void OnInit ()
	{
		base.OnInit ();
		continueAdBtn.onClick.RemoveAllListeners ();
		continueAdBtn.onClick.AddListener (RespawnPlayerVideo);
		continueCoinBtn.onClick.RemoveAllListeners ();
		continueCoinBtn.onClick.AddListener (delegate {
			if (User.BuyWithCoin (25))
				RetryGame ();
		}
		);
	}

	public MeshRenderer mr;

	public override void OnActivate ()
	{
		base.OnActivate ();

		lastTime = Time.time + .4f;
		continueAdPanel.gameObject.SetActive (true);
		continueAdPanel.GetComponent<GUIAnim> ().MoveIn (GUIAnimSystem.eGUIMove.Self);
		AdManager.onInterstitialClosedEvent += RetryGame;
		/*
		continueAdBtn.gameObject.SetActive (true);
		GUIAnimSystem.Instance.MoveIn (transform, true);
		continueAdBtn.GetComponent <ButtonIcon> ().EnableBtn (AdController.Ins.interstitialLoaded);
	} else {
		continueAdPanel.gameObject.SetActive (false);*/
	}

	public override void OnDeactivate ()
	{
		base.OnDeactivate ();
		AdManager.onInterstitialClosedEvent -= RetryGame;

	}

	public void CloseContinueScreen ()
	{
		waitTime = 0;
	}

	void Update ()
	{
		if (givedSecondChance && !AdController.Ins.interstitialLoaded) {
			ScreenController.Ins.ActivateScreen (ScreenController.GameScreen.GameOver);
			return;
		}
		
		if (lastTime + waitTime < Time.time) {
			ScreenController.Ins.ActivateScreen (ScreenController.GameScreen.GameOver);
		}

		continueTimer.fillAmount = 1 - ((Time.time - lastTime) / waitTime);
	}

	void RespawnPlayerVideo ()
	{
		if (AdManager.Ins != null)
			AdManager.Ins.showInterstitial ();
	}

	public void RetryGame ()
	{
		ScreenController.Ins.ActivateScreen (ScreenController.GameScreen.UI);
		BlocksController.Instance.DestroyLastLine (false);
		UIScreen.Ins.playerLose = false;
		//Player.Ins.Retry ();
	}

}

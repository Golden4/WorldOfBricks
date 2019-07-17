using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContinueScreen : ScreenBase {
	[SerializeField] Transform continueAdPanel;
	[SerializeField] Button continueAdBtn;
	[SerializeField] Button continueCoinBtn;
	public Text coinText;

	[SerializeField] Image continueTimer;
	float lastTime;
	float waitTime = 5;
	public int dieCount;

	public bool givedSecondChance = false;

	public override void OnInit ()
	{
		base.OnInit ();
		continueAdBtn.onClick.RemoveAllListeners ();
		continueAdBtn.onClick.AddListener (RespawnPlayerVideo);

	}

	public MeshRenderer mr;

	public override void OnActivate ()
	{
		base.OnActivate ();

		dieCount++;
		coinText.text = (25 * dieCount).ToString ();
		continueCoinBtn.onClick.RemoveAllListeners ();
		continueCoinBtn.onClick.AddListener (delegate {
			if (User.BuyWithCoin (25 * dieCount))
				RetryGame ();
		}
		);

		lastTime = Time.time + .4f;
		continueAdPanel.gameObject.SetActive (true);
		continueAdPanel.GetComponent<GUIAnim> ().MoveIn (GUIAnimSystem.eGUIMove.Self);
		AdManager.onRewardedVideoFinishedEvent += RetryGame;
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
		AdManager.onRewardedVideoFinishedEvent -= RetryGame;
	}

	public void CloseContinueScreen ()
	{
		ScreenController.Ins.ActivateScreen (ScreenController.GameScreen.GameOver);
	}

	void Update ()
	{
		if (givedSecondChance && !AdController.Ins.interstitialLoaded) {
			ScreenController.Ins.ActivateScreen (ScreenController.GameScreen.GameOver);
			return;
		}
		
		if (lastTime + waitTime < Time.time && !clickedToVideoBtn) {
			ScreenController.Ins.ActivateScreen (ScreenController.GameScreen.GameOver);
		}

		if (!clickedToVideoBtn)
			continueTimer.fillAmount = 1 - ((Time.time - lastTime) / waitTime);
	}

	bool clickedToVideoBtn;

	void RespawnPlayerVideo ()
	{
		clickedToVideoBtn = true;
		if (AdManager.Ins != null) {
			AdManager.Ins.showRewardedVideo ();

		}
	}

	public void RetryGame ()
	{
		ScreenController.Ins.ActivateScreen (ScreenController.GameScreen.UI);
		BlocksController.Instance.DestroyLastLine (false);
		UIScreen.Ins.playerLose = false;
		//Player.Ins.Retry ();
	}

}

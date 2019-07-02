using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContinueScreen : ScreenBase {
	[SerializeField] Transform continueAdPanel;
	[SerializeField] Button continueAdBtn;

	[SerializeField] Image continueTimer;
	float lastTime;
	float waitTime = 5;

	public bool givedSecondChance = false;

	public override void Init ()
	{
		base.Init ();
		continueAdBtn.onClick.RemoveAllListeners ();
		continueAdBtn.onClick.AddListener (RespawnPlayer);
	}

	public override void OnActivate ()
	{
		base.OnActivate ();
		lastTime = Time.time + .4f;
		continueAdPanel.gameObject.SetActive (true);
		continueAdPanel.GetComponent <GUIAnim> ().MoveIn (GUIAnimSystem.eGUIMove.Self);

		/*
		continueAdBtn.gameObject.SetActive (true);
		GUIAnimSystem.Instance.MoveIn (transform, true);
		continueAdBtn.GetComponent <ButtonIcon> ().EnableBtn (AdController.Ins.interstitialLoaded);
	} else {
		continueAdPanel.gameObject.SetActive (false);*/
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

	void RespawnPlayer ()
	{
		continueAdBtn.GetComponent <ButtonIcon> ().EnableBtn (false);

		if (AdController.Ins.interstitialLoaded && !givedSecondChance) {
			givedSecondChance = true;
			RetryGame ();

			if (AdController.Ins != null)
				AdController.Ins.ShowInterstitialAD ();
		}
	}

	public void RetryGame ()
	{
		ScreenController.Ins.ActivateScreen (ScreenController.GameScreen.UI);

		//Player.Ins.Retry ();
	}

}

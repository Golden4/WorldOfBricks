using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeResultScreen : ScreenBase {
	
	public Text resultText;
	public Button retryBtn;
	public Button nextBtn;
	public PanelsController pc;

	public override void OnInit ()
	{
		pc.GetComponentInChildren <PanelsController> ();
	}

	public override void OnActivate ()
	{
		base.OnActivate ();
		resultText.gameObject.SetActive (true);
		resultText.GetComponent<GUIAnim> ().MoveIn (GUIAnimSystem.eGUIMove.Self);

		if (UIScreen.Ins.playerWin) {
			OnPlayerWin ();

		} else if (UIScreen.Ins.playerLose) {
				OnPlayerLose ();
			}

		pc.ShowGiftPanel ();
		pc.ShowRewardPanel ();
	}

	void OnPlayerWin ()
	{
		if (Game.curChallengeIndex + 1 < Database.GetChall.challengesData.Length)
			nextBtn.gameObject.SetActive (true);
		else
			nextBtn.gameObject.SetActive (false);

		retryBtn.gameObject.SetActive (false);
		resultText.text = "Challenge " + (Game.curChallengeIndex + 1) + "\nCompleted!";

		if (!User.GetChallengesData.challData [Game.curChallengeIndex]) {
			GiveReward (true);
		} else {
			GiveReward (false);
		}

		User.GetChallengesData.challData [Game.curChallengeIndex] = true;
		User.SaveChallengesData ();
		AudioManager.PlaySoundFromLibrary ("Success");
	}

	void GiveReward (bool give)
	{
		if (give) {
			pc.rewardText.text = "+" + Database.GetChall.challengesData [Game.curChallengeIndex].reward.ToString ();

			int coinAmount = 25;

			Vector3 fromPos = pc.rewardText.transform.parent.parent.parent.position;
			Vector3 toPos = CoinUI.Ins.coinImage.transform.position;
			Debug.Log (fromPos + "   " + toPos);
			Utility.CoinsAnimateRadial (CoinUI.Ins, CoinUI.Ins.coinImage.gameObject, CoinUI.Ins.transform, coinAmount / 2, fromPos, toPos, Screen.width / 3, .5f, CoinUI.Ins.curve, () => {

			});

			Utility.Invoke (CoinUI.Ins, .9f, delegate {
				User.AddCoin (coinAmount);
			});
		} else {
			pc.rewardText.text = "0";
		}

	}

	void OnPlayerLose ()
	{
		nextBtn.gameObject.SetActive (false);
		retryBtn.gameObject.SetActive (true);
		resultText.text = "Challenge " + (Game.curChallengeIndex + 1) + "\nFailed!";
		AudioManager.PlaySoundFromLibrary ("Failed");
	}

	public void LoadNextChallenge ()
	{
		Game.curChallengeIndex++;
		Game.curChallengeInfo = Database.GetChall.challengesData [Game.curChallengeIndex];
		SceneController.RestartLevel ();
	}
}

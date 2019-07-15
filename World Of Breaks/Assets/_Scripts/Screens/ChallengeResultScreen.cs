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
		pc = GetComponentInChildren <PanelsController> ();
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



	}

	void OnPlayerWin ()
	{
		pc.ShowGiftPanel ();
		pc.ShowRewardPanel (true);
		pc.ShowBallsPanel (false);

		if (Game.curChallengeIndex + 1 < Database.GetChall.challengesData.Length)
			nextBtn.gameObject.SetActive (true);
		else
			nextBtn.gameObject.SetActive (false);

		retryBtn.gameObject.SetActive (false);


		if (!User.GetChallengesData.challData [Game.curChallengeIndex]) {
			GiveReward (true);
			resultText.text = string.Format (LocalizationManager.GetLocalizedText ("challenge_complete"), (Game.curChallengeIndex + 1));
		} else {
			GiveReward (false);
			resultText.text = string.Format (LocalizationManager.GetLocalizedText ("challenge_complete_again"), (Game.curChallengeIndex + 1));
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
		pc.ShowGiftPanel ();
		pc.ShowRewardPanel (false);
		pc.ShowBallsPanel (true);
		nextBtn.gameObject.SetActive (false);
		retryBtn.gameObject.SetActive (true);
		resultText.text = string.Format (LocalizationManager.GetLocalizedText ("challenge_failed"), (Game.curChallengeIndex + 1));
		AudioManager.PlaySoundFromLibrary ("Failed");
	}

	public void LoadNextChallenge ()
	{
		Game.curChallengeIndex++;
		Game.curChallengeInfo = Database.GetChall.challengesData [Game.curChallengeIndex];
		SceneController.RestartLevel ();
	}
}

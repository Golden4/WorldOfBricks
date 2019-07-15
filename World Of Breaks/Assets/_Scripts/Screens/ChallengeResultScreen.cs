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

		if (UIScreen.Ins.playerWin) {
			OnPlayerWin ();

		} else if (UIScreen.Ins.playerLose) {
				OnPlayerLose ();
			} else {
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
			pc.GiveReward (true, Database.GetChall.challengesData [Game.curChallengeIndex].reward);
			resultText.text = string.Format (LocalizationManager.GetLocalizedText ("challenge_complete"), (Game.curChallengeIndex + 1));
		} else {
			pc.GiveReward (false);
			resultText.text = string.Format (LocalizationManager.GetLocalizedText ("challenge_complete_again"), (Game.curChallengeIndex + 1));
		}

		User.GetChallengesData.challData [Game.curChallengeIndex] = true;
		User.SaveChallengesData ();
		AudioManager.PlaySoundFromLibrary ("Success");
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

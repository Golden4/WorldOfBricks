using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeResultScreen : ScreenBase {
	
	public Text resultText;
	public Button retryBtn;
	public Button nextBtn;

	public Transform starsParent;
	public Image[] stars;

	public PanelsController pc;

	public static float[] starPersents = new float[] {
		0.1f,
		0.4f,
		0.75f
	};

	public static float progressPersent {
		get {
			return (float)UIScreen.Ins.playerScore / BlocksController.Instance.maxScore / .9f;
		}
	}

	public override void OnInit ()
	{
		pc = GetComponentInChildren <PanelsController> ();
	}

	public override void OnActivate ()
	{
		base.OnActivate ();

		BallController.Instance.ReturnAllBalls ();
		if (UIScreen.Ins.playerWin) {
			OnPlayerWin ();

		} else if (UIScreen.Ins.playerLose) {
				OnPlayerLose ();
			} else {
				OnPlayerLose ();
			}

		Game.ballTryingIndex = -1;
	}

	public static int GetCurrentStarCount (float persent)
	{
		int starCount = 0;

		for (int i = 0; i < starPersents.Length; i++) {
			if (persent > starPersents [i]) {
				starCount++;
			}
		}
		return starCount;
	}

	void OnPlayerWin ()
	{
		pc.ShowGiftPanel (false);
		pc.ShowRewardPanel (true);
		pc.GiveReward (true, UIScreen.Ins.playerScore / 100);

		if (Game.ballTryingIndex > -1) {
			pc.ShowBuyBallPanel (true);
			pc.ShowTryBallsPanel (false);
		} else {
			pc.ShowBuyBallPanel (false);
			pc.ShowTryBallsPanel (true);
		}

		if (Game.curChallengeIndex + 1 < Database.GetChall.challengesData.Length)
			nextBtn.gameObject.SetActive (true);
		else
			nextBtn.gameObject.SetActive (false);

		retryBtn.gameObject.SetActive (false);

		starsParent.gameObject.SetActive (true);

		int starCount = GetCurrentStarCount (progressPersent);

		ShowStars (starCount);

		int completedStars = User.GetChallengesData.challData [Game.curChallengeIndex];

		if (completedStars < starCount) {
			pc.GiveReward (true, (starCount - completedStars) * Database.GetChall.challengesData [Game.curChallengeIndex].reward);
			resultText.text = string.Format (LocalizationManager.GetLocalizedText ("challenge_complete"), (Game.curChallengeIndex + 1));
			User.GetChallengesData.challData [Game.curChallengeIndex] = starCount;
		} else {
			pc.GiveReward (false);
			resultText.text = string.Format (LocalizationManager.GetLocalizedText ("challenge_complete_again"), (Game.curChallengeIndex + 1));
		}

		User.SaveChallengesData ();
		AudioManager.PlaySoundFromLibrary ("Success");
	}

	void OnPlayerLose ()
	{
		pc.ShowGiftPanel (false);
		pc.ShowRewardPanel (false);

		if (Game.ballTryingIndex > -1) {
			pc.ShowBuyBallPanel (true);
			pc.ShowTryBallsPanel (false);
		} else {
			pc.ShowBuyBallPanel (false);
			pc.ShowTryBallsPanel (true);
		}

		nextBtn.gameObject.SetActive (false);
		retryBtn.gameObject.SetActive (true);
		resultText.text = string.Format (LocalizationManager.GetLocalizedText ("challenge_failed"), (Game.curChallengeIndex + 1));
		AudioManager.PlaySoundFromLibrary ("Failed");
	}

	void ShowStars (int starCount)
	{
		for (int i = 0; i < stars.Length; i++) {
			if (i < starCount) {
				stars [i].gameObject.SetActive (true);
				stars [i].GetComponent <GUIAnim> ().MoveIn (GUIAnimSystem.eGUIMove.Self);
			} else {
				stars [i].gameObject.SetActive (false);
			}
		}
	}

	public void LoadNextChallenge ()
	{
		Game.curChallengeIndex++;
		Game.curChallengeInfo = Database.GetChall.challengesData [Game.curChallengeIndex];
		SceneController.RestartLevel ();
	}
}

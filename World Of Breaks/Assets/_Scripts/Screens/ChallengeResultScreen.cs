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

	public int[] coinsForStars = new int[] {
		5, 6, 7
	};

	public static float progressPersent {
		get {
			return (float)UIScreen.Ins.playerScore / BlocksController.Instance.maxScore;
		}
	}

	public override void OnInit ()
	{
		pc = GetComponentInChildren <PanelsController> ();
	}

	public override void OnActivate ()
	{
		base.OnActivate ();
		Game.gamesPlayed++;

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

		if (Game.ballTryingIndex > -1) {
			pc.ShowBuyBallPanel (true);
			pc.ShowTryBallsPanel (false);
		} else if (Game.gamesPlayed % 3 == 1 && PanelsController.CanTakeBall ()) {
				pc.ShowBuyBallPanel (false);
				pc.ShowTryBallsPanel (true);
			} else {
				pc.ShowGiftPanel (true);
				pc.ShowBuyBallPanel (false);
				pc.ShowTryBallsPanel (false);
			}

		if (Game.curChallengeIndex + 1 < Database.GetChall.challengesData.Length)
			nextBtn.gameObject.SetActive (true);
		else
			nextBtn.gameObject.SetActive (false);

		retryBtn.gameObject.SetActive (false);

		starsParent.gameObject.SetActive (true);

		int starCount = GetCurrentStarCount (progressPersent);

		int completedStars = User.GetChallengesData.challData [Game.curChallengeIndex];
		ShowStars (starCount, completedStars);

		if (completedStars < starCount) {
			int reward = 0;

			for (int i = completedStars; i < starCount; i++) {
				reward += coinsForStars [i];
			}

			User.AddCoin (reward);

			pc.rewardText.text = "+" + reward.ToString ();

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
		pc.ShowGiftPanel (true);
		pc.ShowRewardPanel (false);

		if (Game.ballTryingIndex > -1) {
			pc.ShowBuyBallPanel (true);
			pc.ShowTryBallsPanel (false);
		} else if (Game.gamesPlayed % 3 == 1 && PanelsController.CanTakeBall ()) {
				pc.ShowBuyBallPanel (false);
				pc.ShowTryBallsPanel (true);
			} else {
				pc.ShowGiftPanel (true);
				pc.ShowBuyBallPanel (false);
				pc.ShowTryBallsPanel (false);
			}
		starsParent.gameObject.SetActive (false);
		nextBtn.gameObject.SetActive (false);
		retryBtn.gameObject.SetActive (true);
		resultText.text = string.Format (LocalizationManager.GetLocalizedText ("challenge_failed"), (Game.curChallengeIndex + 1));
		AudioManager.PlaySoundFromLibrary ("Failed");
	}

	void ShowStars (int starCount, int completedStars)
	{
		for (int i = 0; i < stars.Length; i++) {
			
			if (i < starCount) {
				stars [i].gameObject.SetActive (true);

				Text plusCoinText = stars [i].transform.GetChild (0).GetComponent<Text> ();
				stars [i].transform.GetChild (0).gameObject.SetActive (false);
				if (i >= completedStars) {
					int index = i;
					GUIAnim anim = stars [i].GetComponent <GUIAnim> ();
					anim.m_ScaleIn.Actions.OnEnd.RemoveAllListeners ();
					anim.m_ScaleIn.Actions.OnEnd.AddListener (delegate {
						plusCoinText.gameObject.SetActive (true);
						plusCoinText.GetComponent<GUIAnim> ().MoveIn (GUIAnimSystem.eGUIMove.Self);
						plusCoinText.text = "+" + coinsForStars [index];
						pc.GiveReward (true, coinsForStars [index], stars [index].transform.position.x, stars [index].transform.position.y, false);
					});

				}

				Vector3 pos = Camera.main.ScreenToWorldPoint (stars [i].transform.position);
				pos.z = 10;
				GameObject go = Instantiate<GameObject> (Resources.Load<GameObject> ("Particles/StarParticle"), pos, Quaternion.identity);
				go.transform.localScale = Vector3.one;
				ParticleSystem ps = go.GetComponent<ParticleSystem> ();
				ps.startDelay = i * .5f;
				ps.Play ();
				Destroy (go, 2);

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

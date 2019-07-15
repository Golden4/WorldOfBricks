using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Diagnostics;

public class UIScreen : ScreenBase {
	public static UIScreen Ins;

	public static event System.Action OnLoseEvent;
	public static event System.Action OnWinEvent;

	public bool playerLose = false;
	public bool playerWin;
	public static bool newGame;

	public Button timeAcceleratorBtn;
	public Button returnBallsBtn;

	public Text clearText;
	public Text newCheckpointText;

	int _topScore = -1;

	public int topScore {
		get {
			if (_topScore == -1) {
				if (PlayerPrefs.HasKey ("TopScore"))
					_topScore = PlayerPrefs.GetInt ("TopScore");
				else
					_topScore = 0;
			}

			return _topScore;
		}

		private set {
			_topScore = value;
			PlayerPrefs.SetInt ("TopScore", value);
		}
	}

	int _checkpoint = -1;

	public int checkpoint {
		get {
			if (_checkpoint == -1) {
				LoadCheckpoint (TileSizeScreen.tileSize);
			}

			return _checkpoint;
		}

		private set {
			_checkpoint = value;
			SaveCheckpoint (_checkpoint, TileSizeScreen.tileSize);
		}
	}

	public Text topScoreText;

	public int level = 0;
	public int playerScore = 0;

	public Text levelText;
	public Text playerScoreText;
	public Text newRecordScoreText;

	public Text checkpointText;
    
	public bool newRecord;

	public GameObject tutorialPrefab;
	public ButtonIcon destroyLastLineBtn;

	public StarsBarPanel starsBarPanel;

	public override void OnInit ()
	{
		Ins = this;
	}

	private void Start ()
	{
		//if (PlayerPrefs.HasKey("CurLevel"))
		//{
		//    UpdateScore(PlayerPrefs.GetInt("CurLevel"));
		//    newGame = false;
		//} else
		//{
		//    newGame = true;
		//}

		//if (newGame)
		//{
		//    PlayerPrefs.DeleteKey("CurLevel");
		//    UpdateScore(UIScreen.Ins.checkpoint);
		//    SetCheckpoint(0);
		//}

		if (!Game.isChallenge) {
			if (newGame) {
				BlocksSaver.DeleteBlockMapKeys ();
				UpdateScore (UIScreen.Ins.checkpoint);
				SetCheckpoint (0);
			}
			starsBarPanel.gameObject.SetActive (false);

		} else {
			checkpointText.gameObject.SetActive (false);
			UIScreen.Ins.UpdateScore (Game.curChallengeInfo.lifeCount);
		}

		BlocksController.Instance.OnChangeTopLine += HideTimeAcceleratorBtn;
	}

	bool timeAcceleratorBtnEnabled;

	public void ShowTimeAcceleratorBtn ()
	{
		if (!timeAcceleratorBtnEnabled) {
			timeAcceleratorBtnEnabled = true;
			timeAcceleratorBtn.gameObject.SetActive (true);
			timeAcceleratorBtn.GetComponent<ButtonIcon> ().EnableBtn (true);
			timeAcceleratorBtn.GetComponent<GUIAnim> ().MoveIn (GUIAnimSystem.eGUIMove.Self);
		}
            
	}

	public void HideTimeAcceleratorBtn ()
	{
		if (timeAcceleratorBtnEnabled) {
			timeAcceleratorBtnEnabled = false;
			timeAcceleratorBtn.GetComponent<GUIAnim> ().MoveOut (GUIAnimSystem.eGUIMove.Self);
		}
		Time.timeScale = 1;
	}

	public void TimeAcceleratorEnable ()
	{
		timeAcceleratorBtn.GetComponent<ButtonIcon> ().EnableBtn (false);
		Time.timeScale = 2;
	}

	public void ShowClearText ()
	{
		if (!UIScreen.Ins.playerLose && checkpoint != level) {
			SetCheckpoint (level);
			ShowPopUpText (LocalizationManager.GetLocalizedText ("clear"), LocalizationManager.GetLocalizedText ("new_checkpoint") + ": " + checkpoint.ToString ());
		}
	}

	public void ChallengeCompleted ()
	{
		if (!UIScreen.Ins.playerLose) {
			UIScreen.Ins.playerWin = true;
			ShowPopUpText (LocalizationManager.GetLocalizedText ("clear"));
		}
	}

	public void ShowPopUpText (string popUpText, string secondText = "")
	{
		if (secondText == "")
			newCheckpointText.gameObject.SetActive (false);
		else {
			newCheckpointText.gameObject.SetActive (true);
			newCheckpointText.text = secondText;
		}
		AudioManager.PlaySoundFromLibrary ("PopUp");
		clearText.gameObject.SetActive (true);
		clearText.text = popUpText;

		GUIAnim textAnim = clearText.GetComponent<GUIAnim> ();
		textAnim.m_ScaleIn.Actions.OnEnd.RemoveAllListeners ();
		textAnim.m_ScaleIn.Actions.OnEnd.AddListener (delegate {
			textAnim.MoveOut (GUIAnimSystem.eGUIMove.Self);
		});

		textAnim.MoveIn (GUIAnimSystem.eGUIMove.Self);
	}

	public void EnableReturnBallsBtn (bool enable)
	{
		if (enable) {
			returnBallsBtn.gameObject.SetActive (true);
			returnBallsBtn.GetComponent<GUIAnim> ().MoveIn (GUIAnimSystem.eGUIMove.Self);
		} else {
			returnBallsBtn.GetComponent<GUIAnim> ().MoveOut (GUIAnimSystem.eGUIMove.Self);
		}
        
	}

	public override void OnCleanUp ()
	{
		BlocksController.Instance.OnChangeTopLine -= HideTimeAcceleratorBtn;
	}

	public void SetTopScore ()
	{
		if (topScore < level) {
			topScore = level;
			newRecord = true;
		}
	}

	public void SetCheckpoint (int check)
	{
		checkpoint = check;
		if (check != 0)
			checkpointText.text = LocalizationManager.GetLocalizedText ("checkpoint") + ": " + checkpoint.ToString ();
		else
			checkpointText.text = LocalizationManager.GetLocalizedText ("no_checkpoint");
	}

	void LoadCheckpoint (TileSizeScreen.TileSize tileSize)
	{
		if (PlayerPrefs.HasKey ("Checkpoint" + (int)tileSize))
			_checkpoint = PlayerPrefs.GetInt ("Checkpoint" + (int)tileSize);
		else
			_checkpoint = 0;
	}

	void SaveCheckpoint (int value, TileSizeScreen.TileSize tileSize)
	{
		PlayerPrefs.SetInt ("Checkpoint" + (int)tileSize, value);
	}

	bool gameStarted;

	GameObject tutrlTemp;

	void ShowTutorial ()
	{
		if (tutrlTemp == null && !PlayerPrefs.HasKey ("TutorialComplete")) {
			tutrlTemp = Instantiate (tutorialPrefab);
			tutrlTemp.transform.SetParent (transform, false);
		}
	}

	public void HideTutorial ()
	{
		if (tutrlTemp != null) {
			Destroy (tutrlTemp);
			PlayerPrefs.SetString ("TutorialComplete", "yes");
		}
	}

	public override void OnActivate ()
	{
		if (!Game.isChallenge) {
			topScoreText.gameObject.SetActive (true);
			topScoreText.text = LocalizationManager.GetLocalizedText ("top_score") + ": " + topScore.ToString ();
		}

		if (!gameStarted)
			Game.OnGameStartedCall ();
		
		ShowTutorial ();

		gameStarted = true;

		SetCheckpoint (checkpoint);

		destroyLastLineBtn.EnableBtn (!Game.isChallenge);
	}

	public void UpdateScore (int curLevel)
	{
		level = curLevel;

		if (!Game.isChallenge) {
			levelText.text = LocalizationManager.GetLocalizedText ("level") + ": " + curLevel.ToString ();
		} else {
			levelText.gameObject.SetActive (false);
			topScoreText.gameObject.SetActive (true);
			topScoreText.text = LocalizationManager.GetLocalizedText ("attempts") + ": " + curLevel.ToString ();
		}


		
		if (curLevel > topScore && !Game.isChallenge) {
			newRecord = true;
			newRecordScoreText.gameObject.SetActive (true);
		} else {
			newRecordScoreText.gameObject.SetActive (false);

		}
	}

	float t;
	int fromValue;

	public void AddPlayerScore (int value)
	{
		if (playerLose)
			return;
		
		t = 1;
		playerScore += value;

		if (Game.isChallenge)
			starsBarPanel.SetProgress (ChallengeResultScreen.progressPersent);
	}

	void Update ()
	{
		if (t > 0) {
			t -= Time.deltaTime / .3f;
			fromValue = Mathf.RoundToInt (Mathf.Lerp ((float)fromValue, (float)playerScore, 1 - t));
			playerScoreText.text = fromValue.ToString ();
		} else if (t <= -0.01f) {
				t = 0;
				fromValue = playerScore;
				playerScoreText.text = playerScore.ToString ();
			}
	}

	public static void OnLoseEventCall ()
	{
		if (OnLoseEvent != null) {
			OnLoseEvent ();
		}

		Ins.playerLose = true;

		if (Game.isChallenge)
			ScreenController.Ins.ActivateScreen (ScreenController.GameScreen.ChallegesResult);
		else
			ScreenController.Ins.ActivateScreen (ScreenController.GameScreen.Continue);
        
	}

	public static void OnWinEventCall ()
	{
		if (OnWinEvent != null) {
			OnWinEvent ();
		}

		Ins.playerWin = true;
		ScreenController.Ins.ActivateScreen (ScreenController.GameScreen.ChallegesResult);

	}

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScreen : ScreenBase {
	public static UIScreen Ins;

    public static event System.Action OnLoseEvent;
    public bool playerLose = false;
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

    public int checkpoint
    {
        get
        {
            if (_checkpoint == -1)
            {
                if (PlayerPrefs.HasKey("Checkpoint"))
                    _checkpoint = PlayerPrefs.GetInt("Checkpoint");
                else
                    _checkpoint = 0;
            }

            return _checkpoint;
        }

        private set
        {
            _checkpoint = value;
            PlayerPrefs.SetInt("Checkpoint", value);
        }
    }

    public Text topScoreText;

	public int score = 0;

	public Text scoreText;
    public Text newRecordScoreText;

    public Text checkpointText;
    
    public bool newRecord;

    public override void Init()
    {
        Ins = this;
    }

    private void Start()
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

        if (newGame)
        {
            BlocksSaver.DeleteBlockMapKeys();
            UIScreen.Ins.UpdateScore(UIScreen.Ins.checkpoint);
            UIScreen.Ins.SetCheckpoint(0);
        }

        BlocksController.Instance.OnChangeTopLine += HideTimeAcceleratorBtn;
    }

    bool timeAcceleratorBtnEnabled;

    public void ShowTimeAcceleratorBtn()
    {
        if (!timeAcceleratorBtnEnabled)
        {
            timeAcceleratorBtnEnabled = true;
            timeAcceleratorBtn.gameObject.SetActive(true);
            timeAcceleratorBtn.GetComponent<ButtonIcon>().EnableBtn(true);
            timeAcceleratorBtn.GetComponent<GUIAnim>().MoveIn(GUIAnimSystem.eGUIMove.Self);
        }
            
    }

    public void HideTimeAcceleratorBtn()
    {
        if (timeAcceleratorBtnEnabled)
        {
            timeAcceleratorBtnEnabled = false;
            timeAcceleratorBtn.GetComponent<GUIAnim>().MoveOut(GUIAnimSystem.eGUIMove.Self);
            Time.timeScale = 1;
        }
    }

    public void TimeAcceleratorEnable()
    {
        timeAcceleratorBtn.GetComponent<ButtonIcon>().EnableBtn(false);
        Time.timeScale = 2;
    }

    public void ShowClearText()
    {
        clearText.gameObject.SetActive(true);

        SetCheckpoint(score);

        GUIAnim textAnim = clearText.GetComponent<GUIAnim>();
        textAnim.m_ScaleIn.Actions.OnEnd.RemoveAllListeners();
        textAnim.m_ScaleIn.Actions.OnEnd.AddListener(delegate
        {
            textAnim.MoveOut(GUIAnimSystem.eGUIMove.Self);
        });

        textAnim.MoveIn(GUIAnimSystem.eGUIMove.Self);

        
    }



    public void EnableReturnBallsBtn(bool enable)
    {
        if (enable)
        {
            returnBallsBtn.gameObject.SetActive(true);
            returnBallsBtn.GetComponent<GUIAnim>().MoveIn(GUIAnimSystem.eGUIMove.Self);
        } else
        {
            returnBallsBtn.GetComponent<GUIAnim>().MoveOut(GUIAnimSystem.eGUIMove.Self);
        }
        
    }

    public override void OnCleanUp ()
	{
        BlocksController.Instance.OnChangeTopLine -= HideTimeAcceleratorBtn;
    }

	public void SetTopScore ()
	{
		if (topScore < score) {
			topScore = score;
			newRecord = true;
		}
	}

    public void SetCheckpoint(int check)
    {
        checkpoint = check;
        checkpointText.text =  LocalizationManager.GetLocalizedText("checkpoint") + ": " + checkpoint.ToString();
        newCheckpointText.text = LocalizationManager.GetLocalizedText("new_checkpoint") + ": " + checkpoint.ToString();
    }

    bool gameStarted;

	public override void OnActivate ()
	{
		topScoreText.text = LocalizationManager.GetLocalizedText ("top_score") + ": " + topScore.ToString ();

		if (!gameStarted)
			Game.OnGameStartedCall ();

		gameStarted = true;

        SetCheckpoint(checkpoint);
    }

	public void UpdateScore (int curScore)
	{
		score = curScore;
        scoreText.text = curScore.ToString();

        if(curScore > topScore)
        {
            newRecord = true;
            newRecordScoreText.gameObject.SetActive(true);
        } else
        {
            newRecordScoreText.gameObject.SetActive(false);
        }
	}

    public static void OnLoseEventCall()
    {
        if(OnLoseEvent != null)
        {
            OnLoseEvent();
        }
        Ins.playerLose = true;
        ScreenController.Ins.ActivateScreen(ScreenController.GameScreen.Continue);
        
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Diagnostics;
using DG.Tweening;

public class UIScreen : ScreenBase<UIScreen>
{
    public bool playerLose = false;
    public bool playerWin;
    public static bool newGame;

    public Button timeAcceleratorBtn;
    public Button returnBallsBtn;
    public Button retryThrowBtn;

    public Text clearText;
    public Text newCheckpointText;

    int _topScore = -1;

    public int topScore
    {
        get
        {
            if (_topScore == -1)
            {
                if (PlayerPrefs.HasKey("TopScore"))
                    _topScore = PlayerPrefs.GetInt("TopScore");
                else
                    _topScore = 0;
            }

            return _topScore;
        }

        private set
        {
            _topScore = value;
            PlayerPrefs.SetInt("TopScore", value);
        }
    }

    int _checkpoint = -1;

    public int checkpoint
    {
        get
        {
            if (_checkpoint == -1)
            {
                LoadCheckpoint(TileSizeScreen.tileSize);
            }

            return _checkpoint;
        }

        private set
        {
            _checkpoint = value;
            SaveCheckpoint(_checkpoint, TileSizeScreen.tileSize);
        }
    }

    public Text topScoreText;

    public int level = 0;
    public int playerScore = 0;
    public int multiplyer;

    public Text levelText;
    public Text playerScoreText;
    public Text multiplyerText;
    public Text newRecordScoreText;
    public Text checkpointText;

    public bool newRecord;

    public ButtonIcon destroyLastLineBtn;
    public ParticleSystem popUpParticle;


    public StarsBarPanel starsBarPanel;
    public override void OnInit()
    {
        base.OnInit();
        if (!Game.isChallenge)
        {
            if (newGame)
            {
                BlocksSaver.DeleteBlockMapKeys();
                UpdateScore(checkpoint);
                SetCheckpoint(0);
            }
            starsBarPanel.gameObject.SetActive(false);

        }
        else
        {
            checkpointText.gameObject.SetActive(false);
            UpdateScore(Game.curChallengeInfo.lifeCount);
        }
    }
    private void Start()
    {

        mySequencePopUp = DOTween.Sequence();
        mySequencePopUp.Append(clearText.transform.DOScale(Vector3.one, .3f).ChangeStartValue(Vector3.zero))
          .AppendInterval(1)
          .Append(clearText.transform.DOScale(Vector3.zero, .3f).ChangeStartValue(Vector3.one)).SetAutoKill(false).Pause();

        BlocksController.Instance.OnChangeTopLine += HideTimeAcceleratorBtn;

        /*	playerScore = 100000;
		playerWin = true;
		if (Game.isChallenge)
			ScreenController.Ins.ActivateScreen (ScreenController.GameScreen.ChallegesResult);*/

    }

    bool timeAcceleratorBtnEnabled;

    public void ShowTimeAcceleratorBtn()
    {

        if (!timeAcceleratorBtnEnabled && !BlocksController.Instance.retryThrow)
        {
            timeAcceleratorBtnEnabled = true;
            timeAcceleratorBtn.gameObject.SetActive(true);
            timeAcceleratorBtn.GetComponent<ButtonIcon>().EnableBtn(true);

            timeAcceleratorBtn.transform.DOKill();
            timeAcceleratorBtn.transform.DOScale(Vector3.zero, .3f).From().ChangeEndValue(Vector3.one);
        }
    }

    public void HideTimeAcceleratorBtn()
    {
        if (timeAcceleratorBtnEnabled)
        {
            timeAcceleratorBtnEnabled = false;

            timeAcceleratorBtn.transform.DOKill();
            timeAcceleratorBtn.transform.DOScale(Vector3.zero, .3f).ChangeStartValue(Vector3.one);
        }

        Time.timeScale = 1;
        timeAccelerated = false;
    }

    public bool timeAccelerated;

    public void TimeAcceleratorEnable()
    {
        timeAccelerated = true;
        timeAcceleratorBtn.GetComponent<ButtonIcon>().EnableBtn(false);
        Time.timeScale = 2;
    }

    public float GetTimeAccelerationValue()
    {
        if (timeAccelerated)
        {
            return 2;
        }

        return 1;
    }

    public void ShowClearText()
    {
        if (!UIScreen.Ins.playerLose && checkpoint != level)
        {

            float persentCheck = .8f;

            if (Ball.HaveAblity(Ball.Ability.HigherChekpoint))
                persentCheck = 1;

            int newCheck = Mathf.RoundToInt(level * persentCheck);

            SetCheckpoint(newCheck);

            ShowPopUpText(LocalizationManager.GetLocalizedText("clear"), LocalizationManager.GetLocalizedText("new_checkpoint") + ": " + checkpoint.ToString());
        }
    }

    public void ChallengeCompleted()
    {
        if (!UIScreen.Ins.playerLose)
        {
            UIScreen.Ins.playerWin = true;
            ShowPopUpText(LocalizationManager.GetLocalizedText("clear"));
            BallController.Instance.ReturnAllBalls();
        }
    }

    Sequence mySequencePopUp;

    public void ShowPopUpText(string popUpText, string secondText = "", Action afterPopUp = null)
    {
        if (mySequencePopUp.IsPlaying())
        {
            return;
        }
        if (secondText == "")
            newCheckpointText.gameObject.SetActive(false);
        else
        {
            newCheckpointText.gameObject.SetActive(true);
            newCheckpointText.text = secondText;
        }

        AudioManager.PlaySoundFromLibrary("PopUp");

        clearText.gameObject.SetActive(true);
        clearText.text = popUpText;

        mySequencePopUp.Restart();
        mySequencePopUp.OnComplete(() => afterPopUp());
        GameObject go = Instantiate<GameObject>(popUpParticle.gameObject);
        go.transform.SetParent(GetComponentInParent<Canvas>().transform, false);
        go.transform.SetAsFirstSibling();
        go.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        Destroy(go, 2);
    }

    Vector3 returnBallsBtnStartPos = default;

    public void EnableReturnBallsBtn(bool enable)
    {
        if (returnBallsBtnStartPos == default)
        {
            returnBallsBtnStartPos = returnBallsBtn.GetComponent<RectTransform>().anchoredPosition;
        }

        returnBallsBtn.transform.DOKill();

        if (enable)
        {
            returnBallsBtn.gameObject.SetActive(true);
            returnBallsBtn.GetComponent<RectTransform>().DOAnchorPos(returnBallsBtnStartPos + Vector3.up * -100, .3f).From().ChangeEndValue(returnBallsBtnStartPos + Vector3.zero);
        }
        else
        {

            returnBallsBtn.GetComponent<RectTransform>().DOAnchorPos(returnBallsBtnStartPos + Vector3.up * -100, .3f).ChangeStartValue(returnBallsBtnStartPos + Vector3.zero);
        }

    }

    public override void OnCleanUp()
    {
        BlocksController.Instance.OnChangeTopLine -= HideTimeAcceleratorBtn;
    }

    public void SetTopScore()
    {
        if (topScore < level)
        {
            topScore = level;
            newRecord = true;
        }
    }

    public void SetCheckpoint(int check)
    {
        checkpoint = check;

        if (check != 0)
            checkpointText.text = LocalizationManager.GetLocalizedText("checkpoint") + ": " + check.ToString();
        else
            checkpointText.text = LocalizationManager.GetLocalizedText("no_checkpoint");
    }

    void LoadCheckpoint(TileSizeScreen.TileSize tileSize)
    {
        if (PlayerPrefs.HasKey("Checkpoint" + (int)tileSize))
            _checkpoint = PlayerPrefs.GetInt("Checkpoint" + (int)tileSize);
        else
            _checkpoint = 0;
    }

    void SaveCheckpoint(int value, TileSizeScreen.TileSize tileSize)
    {
        PlayerPrefs.SetInt("Checkpoint" + (int)tileSize, value);
    }

    bool gameStarted;

    public override void OnActivate()
    {
        if (!Game.isChallenge)
        {
            topScoreText.gameObject.SetActive(true);
            topScoreText.text = LocalizationManager.GetLocalizedText("top_score") + ": " + topScore.ToString();
        }

        if (!gameStarted)
            Game.OnGameStartedCall();

        Tutorial.Ins?.ShowTutorial();

        gameStarted = true;

        SetCheckpoint(checkpoint);

        destroyLastLineBtn.EnableBtn(!Game.isChallenge);
    }


    public void UpdateScore(int curLevel)
    {
        level = curLevel;
        multiplyer = (curLevel + 10) / 10;
        multiplyerText.text = "<size=10>x</size>" + multiplyer;

        if (!Game.isChallenge)
        {
            multiplyerText.gameObject.SetActive(true);
            levelText.text = LocalizationManager.GetLocalizedText("level") + ": " + curLevel.ToString();
        }
        else
        {
            multiplyerText.gameObject.SetActive(false);
            levelText.gameObject.SetActive(false);
            topScoreText.gameObject.SetActive(true);
            topScoreText.text = LocalizationManager.GetLocalizedText("attempts") + ": " + curLevel.ToString();
        }

        if (curLevel > topScore && !Game.isChallenge)
        {
            newRecord = true;
            newRecordScoreText.gameObject.SetActive(true);
        }
        else
        {
            newRecordScoreText.gameObject.SetActive(false);
        }
    }

    float t;
    int fromValue;

    public void AddPlayerScore(int value)
    {
        if (playerLose)
            return;

        t = 1;
        playerScore += value;

        if (Game.isChallenge)
            starsBarPanel.SetProgress(ChallengeResultScreen.progressPersent);
    }

    public void SetPlayerScore(int score)
    {
        fromValue = score;
        playerScore = score;
        playerScoreText.text = score.ToString();
        t = 0;
        if (Game.isChallenge)
            starsBarPanel.SetProgress(ChallengeResultScreen.progressPersent);
    }

    void Update()
    {
        if (t > 0)
        {
            t -= Time.deltaTime / .3f;
            fromValue = Mathf.RoundToInt(Mathf.Lerp((float)fromValue, (float)playerScore, 1 - t));
            playerScoreText.text = fromValue.ToString();
        }
        else if (t <= -0.01f)
        {
            t = 0;
            fromValue = playerScore;
            playerScoreText.text = playerScore.ToString();
        }

        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     UIScreen.Ins.playerWin = true;
        //     ScreenController.Ins.ActivateScreen(ScreenController.GameScreen.ChallegesResult);
        // }
    }

    public void EnableDestroyLastLineBtn(bool enable)
    {
        destroyLastLineBtn.EnableBtn(enable);
    }

    public void EnableRetryThrowBtn(bool enable)
    {
        if (enable && BlocksController.Instance.CanRetry())
        {
            retryThrowBtn.gameObject.SetActive(true);
            retryThrowBtn.transform.DOKill();
            retryThrowBtn.transform.DOScale(Vector3.zero, .3f).From().ChangeEndValue(Vector3.one);
        }
        else
        {
            retryThrowBtn.transform.DOKill();
            retryThrowBtn.transform.DOScale(Vector3.zero, .3f);
        }
    }

    public void ActivateScreenPause()
    {
        ScreenController.Ins.ActivateScreen("Pause");
    }

}

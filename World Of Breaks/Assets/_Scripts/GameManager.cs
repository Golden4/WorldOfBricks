using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : SingletonGeneric<GameManager> {

	public bool playerLose = false;

	public int curScore = 1;
	public int highScore;

	public Text curScoreText;
	public Text highScoreText;

	void Start ()
	{
		EventManager.OnLoseEvent += PlayerLose;

		if (PlayerPrefs.HasKey ("highscore")) {
			highScoreText.text = "Highscore: " + PlayerPrefs.GetInt ("highscore");
		}
	}

	void OnDestroy ()
	{
		EventManager.OnLoseEvent -= PlayerLose;
	}

	void PlayerLose ()
	{
		playerLose = true;
		highScore = curScore;
		if (PlayerPrefs.HasKey ("highscore")) {
			if (PlayerPrefs.GetInt ("highscore") < highScore) {
				PlayerPrefs.SetInt ("highscore", highScore);
			}
			
		} else {
			PlayerPrefs.SetInt ("highscore", highScore);
		}

	}

	public void UpdateCurScoreText ()
	{
		curScoreText.text = "Score: " + curScore;
	}

}

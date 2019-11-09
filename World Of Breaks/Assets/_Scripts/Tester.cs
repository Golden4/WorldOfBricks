using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class Tester : SingletonResourse<Tester>
{

    public int timeScale = 100;
    public bool startTest;
    bool isStartedTest;
    int curTestCount;
    List<string> saveInfo = new List<string>();
    public int testCount = 50;
    public bool needWhile3Stars;
    bool saved;
    bool playerWinRealy;

    public int[] resultsCount = new int[4];
    public float[] resultsTryCount = new float[4];

    public override void OnInit()
    {
        base.OnInit();

        if (Application.isMobilePlatform)
        {
            Destroy(gameObject);
            return;
        }

        SceneController.OnRestartLevel += OnRestartLevel;
        DontDestroyOnLoad(gameObject);
    }

    /*	[UnityEditor.MenuItem ("MyMenu/UnlockChallenges")]
	static void UnlockAll ()
	{
		for (int i = 0; i < User.GetChallengesData.challData.Length; i++) {
			User.GetChallengesData.challData [i] = 2;
		}
	}*/

    void Update()
    {
        if (startTest)
        {
            if (UIScreen.Ins.playerWin || UIScreen.Ins.playerLose)
            {
                if (!SceneController.sceneLoading)
                {

                    if (UIScreen.Ins.playerWin || playerWinRealy)
                    {
                        saveInfo.Add("Result: Challenge complete: " + ChallengeResultScreen.GetCurrentStarCount(ChallengeResultScreen.progressPersent) + " stars");

                        if (playerWinRealy)
                            saveInfo.Add("playerWinRealy");

                        resultsCount[ChallengeResultScreen.GetCurrentStarCount(ChallengeResultScreen.progressPersent)]++;
                        resultsTryCount[ChallengeResultScreen.GetCurrentStarCount(ChallengeResultScreen.progressPersent)] += throwCount;

                        if (needWhile3Stars)
                            if (ChallengeResultScreen.GetCurrentStarCount(ChallengeResultScreen.progressPersent) == 3)
                                startTest = false;

                    }
                    else if (UIScreen.Ins.playerLose)
                    {
                        resultsCount[0]++;
                        saveInfo.Add("Result: Challenge lose");
                        saveInfo.Add("\tBlock Life: " + BlocksController.Instance.CalculateBlockLife());
                    }

                    saveInfo.Add("------------------------------------------------");

                    if (curTestCount >= testCount)
                        StopTest();

                    if (startTest)
                    {
                        GameOverScreen.Ins.RestartLevel();
                    }
                    else
                    {
                        StopTest();
                    }
                }

                return;
            }

            if (!isStartedTest)
            {
                isStartedTest = true;
                StartTest();
            }

            Testing();
        }
    }

    void OnRestartLevel()
    {
        playerWinRealy = false;
        throwCount = 0;
        curTestCount++;
        saveInfo.Add("Current Test: " + curTestCount);
        saveInfo.Add("------------------------------------------------");
    }

    void StartTest()
    {
        if (Game.isChallenge)
        {
            saveInfo.Clear();
            saved = false;
            curTestCount = 0;

            for (int i = 0; i < resultsCount.Length; i++)
            {
                resultsTryCount[i] = 0;
                resultsCount[i] = 0;
            }

            AudioManager.EnableAudio(false);
            saveInfo.Add("Challenge: " + (Game.curChallengeIndex + 1));
            saveInfo.Add("------------------------------------------------");
        }
    }

    int throwCount = 0;

    void Testing()
    {
        if (Time.timeScale != timeScale)
            Time.timeScale = timeScale;

        if (BallController.Instance.canThrow)
        {
            throwCount++;
            int angle = Random.Range(0, 90);
            BallController.Instance.ThrowBalls(angle);
            saveInfo.Add(throwCount + ".\tAngle = " + angle);
            saveInfo.Add("\tBlock Life: " + BlocksController.Instance.CalculateBlockLife());
            saveInfo.Add("\tPlayer Score: " + UIScreen.Ins.playerScore);

            if (BlocksController.Instance.CalculateBlockLife() <= ChallengesInfo.GetChall.GetCurrentChallengesData()[Game.curChallengeIndex].ballCount)
                playerWinRealy = true;

        }
    }

    void StopTest()
    {
        Time.timeScale = 1;
        startTest = false;
        SaveFile();
    }



    void SaveFile()
    {
        if (saved)
            return;

        saved = true;
        string path = Application.dataPath + "/Tests/TestChallengeReport" + (Game.curChallengeIndex + 1) + ".txt";
        StreamWriter file = new StreamWriter(path);

        saveInfo.Add("TestCount: " + testCount);

        for (int i = 0; i < resultsCount.Length; i++)
        {
            string info;
            if (i == 0)
            {
                info = "Challenge Lose Count: " + resultsCount[i];
            }
            else
            {
                info = "Challenge Win " + i + " Stars Count: " + resultsCount[i];
            }

            saveInfo.Add(info);
        }
        saveInfo.Add("------------------------------------------------");

        for (int i = 1; i < resultsTryCount.Length; i++)
        {
            string info;

            info = "Challenge Win " + i + " Stars Middle Try Count: " + resultsTryCount[i] / resultsCount[i];

            saveInfo.Add(info);
        }


        for (int i = 0; i < saveInfo.Count; i++)
        {
            file.WriteLine(saveInfo[i]);
        }

        file.Close();

        Debug.Log("TestFileSaved");

    }

    void OnDestroy()
    {
        if (isInit && isStartedTest)
        {
            SaveFile();
            SceneController.OnRestartLevel -= OnRestartLevel;
        }
    }
}

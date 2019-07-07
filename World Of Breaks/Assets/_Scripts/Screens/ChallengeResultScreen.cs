using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeResultScreen : ScreenBase
{
    public Text resultText;
    public Button retryBtn;
    public Button nextBtn;

    public override void OnActivate()
    {
        base.OnActivate();
        resultText.gameObject.SetActive(true);
        resultText.GetComponent<GUIAnim>().MoveIn(GUIAnimSystem.eGUIMove.Self);

        if (UIScreen.Ins.playerWin)
        {
            if(Game.curChallengeIndex+1 < Database.GetChall.challengesData.Length)
                nextBtn.gameObject.SetActive(true);
            else
                nextBtn.gameObject.SetActive(false);

            retryBtn.gameObject.SetActive(false);
            resultText.text = "Challenge " + (Game.curChallengeIndex + 1) + "\nCompleted!";
        }

        else if (UIScreen.Ins.playerLose)
        {
            nextBtn.gameObject.SetActive(false);
            retryBtn.gameObject.SetActive(true);
            resultText.text = "Challenge " + (Game.curChallengeIndex + 1) + "\nFailed!";
        }
    }

    public void LoadNextChallenge()
    {
        Game.curChallengeIndex++;
        Game.curChallengeInfo = Database.GetChall.challengesData[Game.curChallengeIndex];
        SceneController.RestartLevel();
    }
}

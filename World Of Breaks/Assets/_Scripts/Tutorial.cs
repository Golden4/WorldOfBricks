using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public static Tutorial Ins;
    bool needShowTutorial;
    bool isShowingTutorial;
    public GameObject tutorialPrefab;
    GameObject tutrlTemp;

    void Awake()
    {
        Ins = this;
        needShowTutorial = !PlayerPrefs.HasKey("TutorialComplete");
    }

    public void ShowTutorial()
    {
        if (needShowTutorial)
        {
            tutrlTemp = Instantiate(tutorialPrefab);
            tutrlTemp.transform.SetParent(UIScreen.Ins.transform, false);
            isShowingTutorial = true;
            needShowTutorial = false;
        }
    }

    public void HideTutorial()
    {
        if (tutrlTemp != null && isShowingTutorial)
        {
            Destroy(tutrlTemp);
            PlayerPrefs.SetString("TutorialComplete", "yes");
            isShowingTutorial = false;
            needShowTutorial = false;
        }
    }
}

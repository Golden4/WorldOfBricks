using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarsBarPanel : MonoBehaviour {
	
	public Image barImage;
	public Image[] stars;

	void Start ()
	{
		for (int i = 0; i < ChallengeResultScreen.starPersents.Length; i++) {
			stars [i].rectTransform.anchoredPosition = new Vector2 (150 * ChallengeResultScreen.starPersents [i], 0);
		}

		SetProgress (ChallengeResultScreen.progressPersent);
	}

	public void SetProgress (float persent)
	{
		barImage.fillAmount = persent;
		ShowStars (ChallengeResultScreen.GetCurrentStarCount (persent));
	}

	void ShowStars (int starCount)
	{
		for (int i = 0; i < stars.Length; i++) {
			if (i < starCount) {
				//if (!stars [i].gameObject.activeInHierarchy) {
				stars [i].transform.GetChild (0).gameObject.SetActive (true);
				stars [i].transform.GetChild (0).GetComponent <GUIAnim> ().MoveIn (GUIAnimSystem.eGUIMove.Self);
				//}
			} else {
				stars [i].transform.GetChild (0).gameObject.SetActive (false);
			}
		}
	}

}

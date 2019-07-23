using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarsBarPanel : MonoBehaviour {
	
	public Image barImage;
	public Image[] stars;
	public ParticleSystem starParticle;

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
		int starsAmount = ChallengeResultScreen.GetCurrentStarCount (persent);

		if (starsShowed < starsAmount)
			ShowStars (starsAmount);
	}

	int starsShowed;

	void ShowStars (int starCount)
	{
		for (int i = starsShowed; i < stars.Length; i++) {
			if (i < starCount) {
				Vector3 pos = Camera.main.ScreenToWorldPoint (stars [i].transform.position);
				pos.z = 0;
				GameObject go = Instantiate<GameObject> (starParticle.gameObject, pos, Quaternion.identity);
				ParticleSystem ps = go.GetComponent<ParticleSystem> ();
				ps.Play ();
				Destroy (go, 2);
				AudioManager.PlaySoundFromLibrary ("Star");
				//if (!stars [i].gameObject.activeInHierarchy) {
				stars [i].transform.GetChild (0).gameObject.SetActive (true);
				stars [i].transform.GetChild (0).GetComponent <GUIAnim> ().MoveIn (GUIAnimSystem.eGUIMove.SelfAndChildren);
				//}
			} else {
				stars [i].transform.GetChild (0).gameObject.SetActive (false);
			}
		}
		starsShowed = starCount;
	}

}

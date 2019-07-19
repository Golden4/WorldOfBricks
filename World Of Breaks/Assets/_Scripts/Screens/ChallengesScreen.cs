using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChallengesScreen : ScreenBase {
	public RectTransform challengesHolder;
	public GameObject challengePrefab;
	public List<Button> challList = new List<Button> ();
	public Sprite lockedSprite;
	public Sprite complitedSprite;

	public override void OnInit ()
	{
		base.OnInit ();

		challengesHolder.sizeDelta = new Vector2 (challengesHolder.sizeDelta.x, 150 * ((Database.GetChall.challengesData.Length / 3) + 1) + 20);

		for (int i = 0; i < Database.GetChall.challengesData.Length; i++) {
			GameObject go = Instantiate (challengePrefab);
			go.transform.SetParent (challengesHolder, false);
			go.gameObject.SetActive (true);
			go.GetComponentInChildren<Text> ().text = (i + 1).ToString ();
			int index = i;
			go.GetComponent<Button> ().onClick.AddListener (delegate {
				StartChallenges (index, Database.GetChall.challengesData [index]);
			});

			challList.Add (go.GetComponent<Button> ());
			UpdateButtonState (i, User.GetChallengesData.challData [i]);
		}
	}

	int canPlayChallenegesCount = 0;

	void UpdateButtonState (int index, int count)
	{
		Image spriteState = challList [index].transform.GetChild (0).Find ("StateIcon").GetComponent<Image> ();

		if (User.GetChallengesData.challData [index] > 0) {
			ShowStars (true, index, count);
			challList [index].GetComponent<ButtonIcon> ().EnableBtn (true);
		} else if (canPlayChallenegesCount < 3 && User.GetChallengesData.challData [index] == 0 || index == 0 && User.GetChallengesData.challData [index] == 0) {
				canPlayChallenegesCount++;
				ShowStars (true, index, count);
				spriteState.gameObject.SetActive (false);
				challList [index].GetComponent<ButtonIcon> ().EnableBtn (true);
			} else {
				ShowStars (false, index);
				spriteState.gameObject.SetActive (true);
				spriteState.sprite = lockedSprite;
				challList [index].GetComponent<ButtonIcon> ().EnableBtn (false);
			}


	}

	void ShowStars (bool show, int index, int count = 0)
	{
		if (show) {
			
			Image[] stars = new Image[3];

			for (int i = 0; i < 3; i++) {
				stars [i] = challList [index].transform.GetChild (0).GetChild (0).GetChild (i).GetChild (0).GetComponent<Image> ();

				stars [i].gameObject.SetActive (i < count);
			}
		} else {
			challList [index].transform.GetChild (0).GetChild (0).gameObject.SetActive (false);
		}
	}

	void StartChallenges (int indexChallenge, ChallengesInfo.ChallengeInfo info)
	{
		Game.curChallengeIndex = indexChallenge;
		Game.curChallengeInfo = info;
		MenuScreen.Ins.StartGame (true, true);
	}



}

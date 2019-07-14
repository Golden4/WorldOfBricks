using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingText : MonoBehaviour {
	
	[SerializeField]Text loadingText;

	float minLoadingTime = .5f;
	float loadingTime;
	public Image loadingBar;

	void Start ()
	{
		loadingTime = 0;
		AdManager.Ins.OnInit ();
		PurchaseManager.Ins.TryInit ();
		StartCoroutine (PingPongText (SceneController.nextSceneToLoad, loadingText, 0.7f));

	}

	IEnumerator PingPongText (int sceneIndex, Text text, float time)
	{
		text.gameObject.SetActive (true);

		float alpha = 1;

		Color color = text.color;
		AsyncOperation ao = SceneManager.LoadSceneAsync (sceneIndex);
		ao.allowSceneActivation = false;

		while (ao.progress <= .89f || loadingTime <= minLoadingTime) {
			
			loadingTime += Time.deltaTime;
			loadingBar.fillAmount = Mathf.Clamp01 (loadingTime / minLoadingTime) * .9f;

			alpha = Mathf.PingPong (Time.timeSinceLevelLoad / time, 0.7f) + 0.3f;
			color.a = alpha;
			text.color = color;
			yield return null;
		}

		loadingTime = 0;

		while (loadingTime >= .1f) {
			loadingTime += Time.deltaTime;
			loadingBar.fillAmount = .9f + Mathf.Clamp01 (loadingTime / .1f) * .1f;
			yield return null;
		}

		loadingBar.fillAmount = 1;

		color.a = 1;
		yield return null;

		ao.allowSceneActivation = true;
	}
}

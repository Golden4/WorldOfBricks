using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingText : MonoBehaviour {
	
	[SerializeField]Text loadingText;

	float minLoadingTime = 1;
	float loadingTime;

	void Start ()
	{
		StartCoroutine (PingPongText (1, loadingText, 0.7f));
		loadingTime = 0;
	}

	IEnumerator PingPongText (int sceneIndex, Text text, float time)
	{
		text.gameObject.SetActive (true);

		float alpha = 1;

		Color color = text.color;
		AsyncOperation ao = SceneManager.LoadSceneAsync (sceneIndex);
		ao.allowSceneActivation = false;

		while (!ao.isDone && loadingTime <= minLoadingTime) {

			loadingTime += Time.deltaTime;

			alpha = Mathf.PingPong (Time.timeSinceLevelLoad / time, 0.7f) + 0.3f;
			color.a = alpha;
			text.color = color;
			yield return null;
		}

		color.a = 1;
		yield return null;

		ao.allowSceneActivation = true;
	}
}

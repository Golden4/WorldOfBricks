using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class SceneController : MonoBehaviour {
	public static SceneController Ins;

	public Image image;

	static bool sceneLoading = false;

	public static int nextSceneToLoad = 1;
	const int loadingSceneIndex = 0;

	void Start ()
	{
		if (Ins != this) {
			print ("Destroyed " + this);
			Destroy (transform.parent.gameObject);
			return;
		}
	}

	public static void Init ()
	{
		if (Ins == null) {
			GameObject go = Instantiate (Resources.Load<GameObject> ("Prefabs/ScreenFader"));
			SceneController sc = go.GetComponentInChildren <SceneController> ();
			Ins = sc;
			DontDestroyOnLoad (go);
		}
	}

	public static void LoadSceneWithFade (int index)
	{
		Init ();
		Ins.StartCoroutine (LoadSceneCoroutine (index, true));
	}

	public static event Action OnRestartLevel;

	public static void RestartLevel ()
	{
		if (OnRestartLevel != null) {
			OnRestartLevel ();
		}
		LoadSceneWithFade (SceneManager.GetActiveScene ().buildIndex);
	}

	public static IEnumerator LoadSceneCoroutine (int index, bool fadeIn)
	{
		Ins.image.raycastTarget = true;

		AsyncOperation ao = SceneManager.LoadSceneAsync (index);
		ao.allowSceneActivation = false;

		sceneLoading = true;

		if (fadeIn) {
			yield return FadeImage (Ins.image, true, .2f);
		}

		while (ao.isDone) {
			yield return null;
		}

		ao.allowSceneActivation = true;

		Ins.image.raycastTarget = false;

		sceneLoading = false;
		yield return new WaitForSecondsRealtime (0.01f);
		yield return FadeImage (Ins.image, false, .2f);
	}

	static IEnumerator FadeImage (Image image, bool fadeIn, float time)
	{
		image.gameObject.SetActive (true);
		float alpha = 0;

		Color color = image.color;

		while (alpha < 1) {
			alpha += Time.deltaTime / time;
			color.a = (fadeIn) ? alpha : 1 - alpha;
			image.color = color;
			yield return null;
		}

		color.a = (fadeIn) ? 1 : 0;
		image.color = color;

		image.gameObject.SetActive (fadeIn);
	}

}

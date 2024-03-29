﻿using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : MonoBehaviour {
    public static SceneController Ins;

    public Image image;

    public static bool sceneLoading = false;

    public static int nextSceneToLoad {
        get {
            if (PlayerPrefs.HasKey ("BlocksMap")) {
                return 2;
            }
            return 1;
        }
    }

    const int loadingSceneIndex = 0;

    void Start () {
        if (Ins != this) {
            print ("Destroyed " + this);
            Destroy (transform.parent.gameObject);
            return;
        }
    }

    public static void Init () {
        if (Ins == null) {
            GameObject go = Instantiate (Resources.Load<GameObject> ("Prefabs/ScreenFader"));
            SceneController sc = go.GetComponentInChildren<SceneController> ();
            Ins = sc;
            DontDestroyOnLoad (go);
        }
    }

    public static void LoadSceneWithFade (int index) {
        Init ();
        Time.timeScale = 1;
        sceneLoading = true;
        Ins.StartCoroutine (LoadSceneCoroutine (index, true));
        DOTween.KillAll ();
    }

    public static event Action OnRestartLevel;

    public static void RestartLevel () {
        if (OnRestartLevel != null) {
            OnRestartLevel ();
        }
        LoadSceneWithFade (SceneManager.GetActiveScene ().buildIndex);
    }

    public static IEnumerator LoadSceneCoroutine (int index, bool fadeIn) {
        sceneLoading = true;
        Ins.image.raycastTarget = true;

        if (fadeIn) {
            yield return FadeImage (Ins.image, true, .3f);
        }

        AsyncOperation ao = SceneManager.LoadSceneAsync (index);
        ao.allowSceneActivation = false;

        float lastTime = Time.time;

        yield return null;

        while (ao.progress <= .89f || lastTime + .8f > Time.time) {
            yield return null;
        }

        ao.allowSceneActivation = true;

        while (!ao.isDone) {
            yield return null;
        }

        Ins.image.raycastTarget = false;

        sceneLoading = false;

        yield return FadeImage (Ins.image, false, .3f);
    }

    static IEnumerator FadeImage (Image image, bool fadeIn, float time) {
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
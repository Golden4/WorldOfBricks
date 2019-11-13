using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;
using DG.Tweening;

public class LoadingText : MonoBehaviour, IPermissionGrantedListener
{
    [SerializeField] Text loadingText;

    float minLoadingTime = .5f;
    float loadingTime;
    public Image loadingBar;

    void Awake()
    {
        Appodeal.requestAndroidMPermissions(this);
    }

    void Start()
    {
        loadingTime = 0;
        AdManager.Ins.OnInit();
        PurchaseManager.Ins.TryInit();
        StartCoroutine(PingPongText(SceneController.nextSceneToLoad, 0.7f));

        // loadingText.gameObject.SetActive(true);
        // loadingText.DOFade(1f, .5f).ChangeStartValue(Color.clear).SetLoops(-1, LoopType.Yoyo);
        // SceneManager.LoadSceneAsync(SceneController.nextSceneToLoad);
    }

    IEnumerator PingPongText(int sceneIndex, float time)
    {
        loadingText.gameObject.SetActive(true);

        loadingText.DOFade(0.5f, .5f).ChangeStartValue(1f).SetLoops(-1, LoopType.Yoyo);

        float alpha = 1;

        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneIndex);
        ao.allowSceneActivation = false;

        while (ao.progress <= .89f || loadingTime <= minLoadingTime)
        {

            loadingTime += Time.deltaTime;
            loadingBar.fillAmount = Mathf.Clamp01(loadingTime / minLoadingTime) * .9f;

            alpha = Mathf.PingPong(Time.timeSinceLevelLoad / time, 0.7f) + 0.3f;
            yield return null;
        }

        loadingTime = 0;

        while (loadingTime >= .1f)
        {
            loadingTime += Time.deltaTime;
            loadingBar.fillAmount = .9f + Mathf.Clamp01(loadingTime / .1f) * .1f;
            yield return null;
        }

        loadingBar.fillAmount = 1;

        yield return null;

        ao.allowSceneActivation = true;
    }

    public void writeExternalStorageResponse(int result)
    {
        if (result == 0)
        {
            Debug.Log("WRITE_EXTERNAL_STORAGE permission granted");
        }
        else
        {
            Debug.Log("WRITE_EXTERNAL_STORAGE permission grant refused");
        }
    }

    public void accessCoarseLocationResponse(int result)
    {
        if (result == 0)
        {
            Debug.Log("ACCESS_COARSE_LOCATION permission granted");
        }
        else
        {
            Debug.Log("ACCESS_COARSE_LOCATION permission grant refused");
        }
    }
}

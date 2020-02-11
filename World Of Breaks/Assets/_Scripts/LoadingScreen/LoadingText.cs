using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class LoadingText : MonoBehaviour
{
    [SerializeField] Text loadingText;

    float minLoadingTime = .5f;
    float loadingTime;
    public Image loadingBar;

    void Start()
    {
        loadingTime = 0;
        AdManager.Ins.OnInit();
        PurchaseManager.Ins.TryInit();
        AudioManager.Ins.OnInit();
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
        ao.completed += Ao_completed;

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

        ao.allowSceneActivation = true;
        
        yield return null;

    }

    private void Ao_completed(AsyncOperation obj)
    {
        loadingBar.fillAmount = 1;
    }
}

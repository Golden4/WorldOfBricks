using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StarsBarPanel : MonoBehaviour
{
    public Image barImage;
    public Image[] stars;
    public ParticleSystem starParticle;
    public ParticleSystem progressParticle;
    RectTransform RTprogressParticle;
    void Start()
    {
        for (int i = 0; i < ChallengeResultScreen.starPersents.Length; i++)
        {
            stars[i].rectTransform.anchoredPosition = new Vector2(transform.GetComponent<RectTransform>().sizeDelta.x * ChallengeResultScreen.starPersents[i], 0);
            stars[i].transform.GetChild(0).gameObject.SetActive(false);
        }

        RTprogressParticle = progressParticle.GetComponent<RectTransform>();

        SetProgress(ChallengeResultScreen.progressPersent);
    }

    public void SetProgress(float persent)
    {
        barImage.fillAmount = persent;
        int starsAmount = ChallengeResultScreen.GetCurrentStarCount(persent);

        float width = GetComponent<RectTransform>().sizeDelta.x;
        persent = Mathf.Clamp01(persent);
        if (width * persent > 0 && width * persent < 1)
        {
            RTprogressParticle.anchoredPosition = new Vector2(width * persent, RTprogressParticle.anchoredPosition.y);
            progressParticle.Play();
        }

        if (starsShowed < starsAmount)
            ShowStars(starsAmount);
    }

    int starsShowed = 0;

    void ShowStars(int starCount)
    {
        for (int i = starsShowed; i < stars.Length; i++)
        {
            if (i < starCount)
            {
                GameObject go = Instantiate<GameObject>(starParticle.gameObject);
                go.transform.SetParent(transform, false);
                go.transform.position = stars[i].transform.position;

                ParticleSystem ps = go.GetComponent<ParticleSystem>();
                ps.Play();
                Destroy(go, 2);
                AudioManager.PlaySoundFromLibrary("Star");
                stars[i].transform.GetChild(0).gameObject.SetActive(true);
                stars[i].transform.GetChild(0).transform.DOScale(Vector3.one, .5f).ChangeStartValue(Vector3.zero).SetEase(Ease.OutBounce);
            }
            else
            {
                stars[i].transform.GetChild(0).gameObject.SetActive(false);
            }
        }
        starsShowed = starCount;
    }

}

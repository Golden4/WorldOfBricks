using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class RetryBox : MonoBehaviour
{
    public Image backgroundImage;
    public Button videoBtn;
    public Button coinBtn;
    public Button cancelBtn;
    public Text title;
    public RectTransform box;

    public GUIAnim anim;

    public static RetryBox instance;

    static void Init()
    {
        GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/RetryBox"));
        go.transform.SetParent(FindObjectOfType<ScreenController>().transform, false);
        instance = go.GetComponent<RetryBox>();
        instance.anim = instance.GetComponentInChildren<GUIAnim>();
    }

    public static void Show(string title, Action onClickVideoBtn, Action onClickCoinBtn = null, Action onClickCancelBtn = null, bool videoBtnEnabled = true, bool coinBtnEnabled = true, string coinBtnText = "")
    {
        if (instance == null)
            Init();

        instance.backgroundImage.raycastTarget = true;
        instance.gameObject.SetActive(true);

        instance.coinBtn.GetComponentInChildren<Text>().text = coinBtnText;

        instance.anim.MoveIn(GUIAnimSystem.eGUIMove.SelfAndChildren);

        instance.title.text = title;

        instance.videoBtn.onClick.RemoveAllListeners();
        instance.coinBtn.onClick.RemoveAllListeners();
        instance.cancelBtn.onClick.RemoveAllListeners();

        instance.videoBtn.GetComponent<ButtonIcon>().EnableBtn(videoBtnEnabled);
        instance.coinBtn.GetComponent<ButtonIcon>().EnableBtn(coinBtnEnabled);

        instance.videoBtn.onClick.AddListener(() =>
        {
            if (onClickVideoBtn != null)
                onClickVideoBtn();
        });

        instance.coinBtn.onClick.AddListener(() =>
        {
            if (onClickCoinBtn != null)
                onClickCoinBtn();
        });

        instance.cancelBtn.onClick.AddListener(() =>
        {
            if (onClickCancelBtn != null)
                onClickCancelBtn();
            Hide();
        });
    }

    public static void Hide()
    {
        instance.anim.MoveOut(GUIAnimSystem.eGUIMove.SelfAndChildren);
        instance.backgroundImage.raycastTarget = false;
    }
}

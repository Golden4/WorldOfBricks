using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MessageBox : MonoBehaviour
{
    public static List<MessageBox> messageBoxes = new List<MessageBox>();
    public Image box;
    public Image backgroundImage;
    public Image iconImage;
    public Button backgroundButton;
    public Button imageBtn;
    public Button imageTextBtn;
    public Button textBtn;
    public Button cancelBtn;
    public Text title;
    public RectTransform descText;
    public Image progressImage;
    public Image progressImageCircle;
    public RectTransform settingsBtns;
    public Sprite[] iconSprites;
    public Sprite[] btnSprites;

    public enum BoxType
    {
        Retry,
        Failed,
        Pause,
        Settings,
        Continue,
        Info
    }

    public enum BtnSprites
    {
        Home,
        Retry,
        Video,
        Coin,
        Close,
    }

    static MessageBox CreateNewMessageBox()
    {
        GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/MessageBox"));
        go.transform.name = "MessageBox" + messageBoxes.Count;
        go.transform.SetAsLastSibling();
        go.GetComponent<Canvas>().sortingOrder = 20 + 10 * messageBoxes.Count;
        MessageBox newMsgBox = go.GetComponent<MessageBox>();
        newMsgBox.imageBtn.gameObject.SetActive(false);
        newMsgBox.imageTextBtn.gameObject.SetActive(false);
        newMsgBox.textBtn.gameObject.SetActive(false);
        newMsgBox.progressImage.gameObject.SetActive(false);
        newMsgBox.progressImageCircle.gameObject.SetActive(false);
        newMsgBox.settingsBtns.gameObject.SetActive(false);
        newMsgBox.descText.gameObject.SetActive(false);
        messageBoxes.Add(newMsgBox);

        return newMsgBox;
    }

    public static MessageBox ShowStatic(string title, BoxType type, Action onClickCancelBtn = null, bool hideOnClickBackground = true)
    {
        MessageBox box = CreateNewMessageBox();
        box.Show(title, type, onClickCancelBtn, hideOnClickBackground);

        return box;
    }

    public void Show(string title, BoxType type, Action onClickCancelBtn = null, bool hideOnClickBackground = true)
    {
        this.title.text = title;
        iconImage.sprite = iconSprites[(int)type];

        backgroundImage.raycastTarget = true;
        gameObject.SetActive(true);

        cancelBtn.onClick.RemoveAllListeners();
        cancelBtn.onClick.AddListener(() =>
        {
            if (onClickCancelBtn != null)
                onClickCancelBtn();

            Hide();
        });

        backgroundButton.onClick.RemoveAllListeners();
        backgroundButton.onClick.AddListener(() =>
        {
            if (hideOnClickBackground)
            {
                if (onClickCancelBtn != null)
                    onClickCancelBtn();

                Hide();
            }
        });

        backgroundImage.DOFade(0, .5f).From().SetUpdate(true);
        box.rectTransform.DOAnchorPos(Vector3.zero, .5f).ChangeStartValue(Vector3.up * 300).SetUpdate(true);
        box.rectTransform.DOScale(0, .5f).From().SetEase(Ease.InSine).SetUpdate(true).SetEase(Ease.OutQuint);
    }

    public MessageBox SetDesc(string descText, Sprite image = null)
    {
        this.descText.gameObject.SetActive(true);

        if (image != null)
        {
            Image image1 = this.descText.GetComponentInChildren<Image>(true);
            image1.gameObject.SetActive(true);
            image1.sprite = image;
        }

        this.descText.GetComponentInChildren<Text>().text = descText;
        return this;
    }

    public MessageBox SetProgress(float amount)
    {
        amount = Mathf.Clamp01(amount);
        progressImageCircle.gameObject.SetActive(true);

        if (amount > 0.43f && amount < 0.572f)
        {
            float amount1 = (amount - 0.435f) / (0.572f - 0.435f);
            progressImageCircle.fillAmount = amount1;
        }
        else
        {
            if (amount > 0.5f)
            {
                progressImageCircle.fillAmount = 1;
            }
            else
            {
                progressImageCircle.fillAmount = 0;
            }
        }

        progressImage.gameObject.SetActive(true);
        progressImage.fillAmount = amount;

        return this;
    }

    public MessageBox SetImageBtn(bool enabled = true, Action onBtnClick = null, BtnSprites btnSprites = BtnSprites.Home, Color color = default, bool hideOnClick = true)
    {

        if (!imageBtn.gameObject.activeInHierarchy)
            imageBtn.gameObject.SetActive(true);
        else
        {
            GameObject newBtn = Instantiate(imageBtn.gameObject);
            newBtn.transform.SetParent(imageBtn.transform.parent, false);
            newBtn.gameObject.SetActive(true);
            imageBtn = newBtn.GetComponent<Button>();
        }

        if (color != default)
            imageTextBtn.GetComponent<Image>().color = color;

        imageBtn.transform.Find("Image").GetComponentInChildren<Image>().sprite = this.btnSprites[(int)btnSprites];

        imageBtn.onClick.RemoveAllListeners();
        imageBtn.GetComponent<ButtonIcon>().EnableBtn(enabled);
        imageBtn.onClick.AddListener(() =>
                {
                    if (onBtnClick != null)
                        onBtnClick();
                    if (hideOnClick)
                        Hide();
                });

        return this;
    }

    public MessageBox SetImageTextBtn(string text, bool enabled = true, Action onBtnClick = null, BtnSprites btnSprites = BtnSprites.Home, Color color = default, bool hideOnClick = true)
    {

        if (!imageTextBtn.gameObject.activeInHierarchy)
            imageTextBtn.gameObject.SetActive(true);
        else
        {
            GameObject newBtn = Instantiate(imageTextBtn.gameObject);
            newBtn.transform.SetParent(imageTextBtn.transform.parent, false);
            newBtn.gameObject.SetActive(true);
            imageTextBtn = newBtn.GetComponent<Button>();
        }

        if (color != default)
            imageTextBtn.GetComponent<Image>().color = color;

        imageTextBtn.transform.Find("Image").GetComponentInChildren<Image>().sprite = this.btnSprites[(int)btnSprites];

        imageTextBtn.GetComponentInChildren<Text>().text = text;
        imageTextBtn.onClick.RemoveAllListeners();
        imageTextBtn.GetComponent<ButtonIcon>().EnableBtn(enabled);

        imageTextBtn.onClick.AddListener(() =>
                {
                    if (onBtnClick != null)
                        onBtnClick();
                    if (hideOnClick)
                        Hide();
                });

        return this;
    }

    public MessageBox SetTextBtn(string text, bool enabled = true, Action onBtnClick = null, Color color = default, bool hideOnClick = true)
    {

        if (!textBtn.gameObject.activeInHierarchy)
            textBtn.gameObject.SetActive(true);
        else
        {
            GameObject newBtn = Instantiate(textBtn.gameObject);
            newBtn.transform.SetParent(textBtn.transform.parent, false);
            newBtn.gameObject.SetActive(true);
            textBtn = newBtn.GetComponent<Button>();
        }

        if (color != default)
            textBtn.GetComponent<Image>().color = color;


        textBtn.GetComponentInChildren<Text>().text = text;

        textBtn.onClick.RemoveAllListeners();
        textBtn.onClick.AddListener(() =>
         {
             if (onBtnClick != null)
                 onBtnClick();
             if (hideOnClick)
                 Hide();
         });

        return this;
    }

    public MessageBox ShowSettings()
    {
        settingsBtns.gameObject.SetActive(true);
        return this;
    }

    public static void HideAll()
    {
        for (int i = 0; i < messageBoxes.Count; i++)
        {
            messageBoxes[i].Hide();
            i--;

        }
    }

    public void Hide()
    {
        messageBoxes.Remove(this);
        backgroundImage.DOFade(0, .5f).SetUpdate(true);

        Utility.FadeUITo(box.transform, 0, .2f, true);

        box.rectTransform.DOAnchorPos(Vector3.zero, .3f).ChangeEndValue(Vector3.up * -300).SetEase(Ease.OutSine).SetUpdate(true).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
}

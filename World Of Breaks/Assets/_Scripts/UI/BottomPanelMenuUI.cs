using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BottomPanelMenuUI : MonoBehaviour
{
    public static BottomPanelMenuUI Ins;
    public BottomPanelBtnInfo[] btnInfos;
    public Button selectedBtn;
    Text selectedBtnText;
    Image selectedBtnSprite;
    int curActiveBtnIndex = 2;
    float lastClickTime = -1;

    void Awake()
    {
        Ins = this;
    }

    IEnumerator Start()
    {
        selectedBtnText = selectedBtn.transform.GetComponentInChildren<Text>();
        selectedBtnSprite = selectedBtn.transform.Find("Image").GetComponent<Image>();

        selectedBtnSprite.transform.DOScale(1, .4f).ChangeStartValue(Vector3.zero).ChangeEndValue(Vector3.one).SetEase(Ease.OutBounce).SetAutoKill(false).Pause();
        selectedBtn.transform.DOScale(1, .4f).ChangeStartValue(Vector3.one * .8f).ChangeEndValue(Vector3.one).SetEase(Ease.OutBounce).SetAutoKill(false).Pause();
        yield return null;
        selectedBtn.onClick.AddListener(() => { ChangeBtn(curActiveBtnIndex); });
        ChangeBtn(curActiveBtnIndex);
    }

    public void ChangeBtn(int index)
    {
        ChangeBtn(index, true);
    }

    public void ChangeBtn(int index, bool activateScreen = true)
    {
        if (lastClickTime + .2f > Time.time)
            return;

        lastClickTime = Time.time;
        curActiveBtnIndex = index;
        ChangeBtn(btnInfos[index], activateScreen);
    }

    public void ChangeBtn(BottomPanelBtnInfo info, bool activateScreen = true)
    {
        selectedBtn.gameObject.SetActive(true);
        selectedBtnText.text = info.name;
        selectedBtnSprite.sprite = info.image.sprite;
        selectedBtn.transform.position = info.button.transform.position;
        selectedBtnSprite.color = info.color;
        selectedBtnText.color = info.color;
        selectedBtnSprite.transform.DORestart(false);
        selectedBtn.transform.DORestart(false);

        if (activateScreen && ScreenController.curActiveScreen != info.screenToChange)
            ScreenController.Ins.ActivateScreen(info.screenToChange);
    }

    [System.Serializable]
    public class BottomPanelBtnInfo
    {
        public string name;
        public Image image;
        public Button button;
        public Color color = Color.white;
        public ScreenController.GameScreen screenToChange;
    }
}
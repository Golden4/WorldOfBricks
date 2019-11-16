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
        ChangeBtn(btnInfos[index], activateScreen, index);
        curActiveBtnIndex = index;
    }

    public void ChangeBtn(BottomPanelBtnInfo info, bool activateScreen = true, int index = -1)
    {
        selectedBtn.gameObject.SetActive(true);
        selectedBtn.transform.DOMove(info.button.transform.position, .15f);
        selectedBtn.transform.DORestart(false);

        selectedBtnText.text = info.name;
        selectedBtnText.color = info.color;

        selectedBtnSprite.sprite = info.image.sprite;
        selectedBtnSprite.color = info.color;
        selectedBtnSprite.transform.DORestart(false);

        if (activateScreen && ScreenController.curActiveScreen != info.screenToChange.screenType)
        {
            ScreenController.Ins.ActivateScreen(info.screenToChange.screenType);

            if(index != curActiveBtnIndex)
                AnimateScreen(info.screenToChange, (index - curActiveBtnIndex) > 0);
        }
    }

    public void AnimateScreen(ScreenBase screenBase, bool directionRight)
    {
        screenBase.GetComponent<RectTransform>().DOKill();
        screenBase.GetComponent<RectTransform>().DOAnchorPosX(150 * (directionRight ? 1 : -1), .5f).From().SetEase(Ease.OutCubic);
    }

    [System.Serializable]
    public class BottomPanelBtnInfo
    {
        public string name;
        public Image image;
        public Button button;
        public Color color = Color.white;
        public ScreenBase screenToChange;
    }
}
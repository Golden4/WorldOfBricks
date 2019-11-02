using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BottomPanelMenuUI : MonoBehaviour
{
    public BottomPanelBtnInfo[] btnInfos;
    public Image selectedBtn;
    Text selectedBtnText;
    Image selectedBtnSprite;

    void Start()
    {
        selectedBtnText = selectedBtn.transform.Find("Text").GetComponent<Text>();
        selectedBtnSprite = selectedBtn.transform.Find("Image").GetComponent<Image>();
    }

    [System.Serializable]
    public class BottomPanelBtnInfo
    {
        public Sprite sprite;
        public string name;
        public ScreenController.GameScreen screen;
    }

    public void ChangeBtn(int index)
    {

    }

    public void ChangeBtn(BottomPanelBtnInfo info)
    {

    }
}
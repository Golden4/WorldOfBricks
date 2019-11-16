using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeButton : MonoBehaviour
{
    [SerializeField]
    Image topImage;

    [SerializeField]
    Image bottomImage;

    [SerializeField]
    Image lockedIconImage;

    [SerializeField]
    Text lvlNumText;

    [SerializeField]
    Image[] starsImages;

    [SerializeField]
    Color lockedColorTop;

    [SerializeField]
    Color lockedColorBottom;

    public void SetButton(bool locked, int lvlNumber, Color topColor, Color bottomColor, int starsCount)
    {
        lockedIconImage.gameObject.SetActive(locked);

        if (locked)
        {
            topImage.color = lockedColorTop;
            bottomImage.color = lockedColorBottom;
            GetComponent<Button>().interactable = false;
            lvlNumText.color = lockedColorBottom;
        } else
        {
            topImage.color = topColor;
            bottomImage.color = bottomColor;
            GetComponent<Button>().interactable = true;
            lvlNumText.color = bottomColor;
        }

        lvlNumText.text = lvlNumber.ToString();

        for (int i = 0; i < 3; i++)
        {
            starsImages[i].gameObject.SetActive(i < starsCount);
        }

    }
}

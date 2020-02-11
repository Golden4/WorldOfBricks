using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuBackground : MonoBehaviour
{
    public RawImage backroundImage;
    public Texture2D[] icons;

    public void ChangeBackground(int index)
    {
        backroundImage.texture = icons[index];
    }

}

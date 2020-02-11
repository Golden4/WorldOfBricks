using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuBackground : MonoBehaviour
{
    public RawImage backroundImage;
    public Texture2D[] icons;

    private void Update()
    {
        Rect rect = backroundImage.uvRect;
        rect.y += Time.deltaTime * .1f;
        backroundImage.uvRect = rect;
    }

    public void ChangeBackground(int index)
    {
        backroundImage.texture = icons[index];
    }

}

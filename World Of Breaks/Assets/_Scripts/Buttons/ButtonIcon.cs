using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonIcon : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{

	public bool changingColor = false;
	public float changingTime = 0.3f;
    public bool fadingChangeColor = false;
	public Color colorToChange;
	public Color colorToChangeOutline;
	private Color colorOrig;
	private Color colorOrigOutline;
	private Image image;
	private Outline outline;
	private float lastChagingTime = -1;
	private int colorIndex;
	public bool btnEnabled = true;

    string clickSoundName = "ButtonClick1";

    Vector3 prevPos = Vector3.zero;

	public void OnPointerDown (PointerEventData eventData)
	{
		if (btnEnabled && transform.childCount > 0) {
			if (prevPos == Vector3.zero)
				prevPos = transform.GetChild (0).localPosition;
		
			transform.GetChild (0).localPosition = prevPos - Vector3.up * 5;
		}
	}


	public void OnPointerUp (PointerEventData eventData)
	{
		if (btnEnabled && transform.childCount > 0)
			transform.GetChild (0).localPosition = prevPos;
	}

    public void OnPointerClick(PointerEventData eventData)
    {
        if(!string.IsNullOrEmpty(clickSoundName))
            AudioManager.PlaySoundFromLibrary(clickSoundName);
    }

    void Start ()
	{
		image = GetComponent <Image> ();

		if (image != null) {
			colorOrig = image.color;

		}
		outline = GetComponent <Outline> ();

		if (outline != null)
			colorOrigOutline = outline.effectColor;
	}
    float t = 0;

    void Update ()
	{
        if (image == null)
            return;

        if (!fadingChangeColor)
        {
            if (lastChagingTime + changingTime < Time.time && changingColor)
            {
                lastChagingTime = Time.time;
                colorIndex = (colorIndex + 1) % 2;

                if (colorIndex == 0)
                {
                    image.color = colorToChange;
                }
                else
                {
                    image.color = colorOrig;
                }

                if (outline != null)
                {
                    if (colorIndex == 0)
                    {
                        outline.effectColor = colorToChangeOutline;
                    }
                    else
                    {
                        outline.effectColor = colorOrigOutline;
                    }
                }
            }
            else if (!changingColor && image.color != colorOrig)
            {
                image.color = colorOrig;
                outline.effectColor = colorOrigOutline;
            }

        } else
        {
            if (t > 0)
            {
                t -= Time.deltaTime / changingTime;
                image.color = Color.Lerp(colorToChange, colorOrig, Mathf.Abs(t - 1));

                if (outline != null)
                    outline.effectColor = Color.Lerp(colorToChangeOutline, colorOrigOutline, Mathf.Abs(t - 1));

                if (!changingColor)
                    t = -1;

            }
            else if (t <= 0.01f)

                if (!changingColor)
                {
                    t = 0;
                    image.color = colorOrig;
                    outline.effectColor = colorOrigOutline;
                }
                else t = 2;
            
        }
	}

	public void EnableBtn (bool enable)
	{
		btnEnabled = enable;

		gameObject.GetComponent <Button> ().interactable = enable;

	}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonIcon : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

	public bool changingColor = false;
	public float changingTime = 0.3f;
	public Color colorToChange;
	public Color colorToChangeOutline;
	private Color colorOrig;
	private Color colorOrigOutline;
	private Image image;
	private Outline outline;
	private float lastChagingTime = -1;
	private int colorIndex;
	public bool btnEnabled = true;

	Vector3 prevPos = Vector3.zero;

	public void OnPointerDown (PointerEventData eventData)
	{
		if (btnEnabled) {
			if (prevPos == Vector3.zero)
				prevPos = transform.GetChild (0).localPosition;
		
			transform.GetChild (0).localPosition = prevPos - Vector3.up * 5;
		}
	}


	public void OnPointerUp (PointerEventData eventData)
	{
		if (btnEnabled)
			transform.GetChild (0).localPosition = prevPos;
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

	void Update ()
	{
		if (changingColor && image != null && lastChagingTime + changingTime < Time.time) {
			lastChagingTime = Time.time;
			colorIndex = (colorIndex + 1) % 2;

			if (colorIndex == 0) {
				image.color = colorToChange;
			} else {
				image.color = colorOrig;
			}

			if (outline != null) {
				if (colorIndex == 0) {
					outline.effectColor = colorToChangeOutline;
				} else {
					outline.effectColor = colorOrigOutline;
				}
			}
		} else if (!changingColor && image.color != colorOrig) {
			image.color = colorOrig;
			outline.effectColor = colorOrigOutline;
		}
	}

	public void EnableBtn (bool enable)
	{
		btnEnabled = enable;

		gameObject.GetComponent <Button> ().interactable = enable;

	}

}

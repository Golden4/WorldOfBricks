using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollSnap : MonoBehaviour, IBeginDragHandler, IEndDragHandler {
	public GameObject[] items;

	public Transform itemsHolder;
    public Animation ballPrefab;
	ScrollRect sr;

	public event System.Action<int> OnChangeItemEvent;

	public float ContentSize {
		get {
			return 200 + 210 * items.Length;
		}
	}

	public int GetCurItemIndex {
		get {
			return (-Mathf.FloorToInt (sr.content.localPosition.x) + 75) / 200;
		}
	}

	bool dragging;

	public void Init ()
	{
		sr = GetComponent <ScrollRect> ();

		SpawnItemsFromData ();

		sr.content.sizeDelta = new Vector2 (ContentSize, sr.content.sizeDelta.y);
	}

	void SpawnItemsFromData ()
	{
		items = new GameObject[Database.Get.playersData.Length];
		for (int i = 0; i < Database.Get.playersData.Length; i++) {
            items [i] = Instantiate(ballPrefab.gameObject);
			items [i].transform.SetParent (itemsHolder);
			items [i].transform.localPosition = Vector3.right * 200 * i;
			items [i].transform.localEulerAngles = Vector3.zero;
			//items [i].layer = LayerMask.NameToLayer ("ShopItem");

            Image image = items [i].GetComponent <Image> ();
            image.rectTransform.sizeDelta = Vector2.one * 30;
            items[i].GetComponent<Animation>().playAutomatically = false;

            image.sprite = Database.Get.playersData[i].playerPrefab.GetComponent<SpriteRenderer>().sprite;
		}
	}

	public Color boughtColor;
	public Color notBoughtColor;

	public void SetItemState (int itemIndex, bool bought)
	{
        Image image = items[itemIndex].GetComponent<Image>();

        image.color = (bought) ? boughtColor : notBoughtColor;

	}

	int lastItemIndex = -10;

	void Update ()
	{

		FocusToObject (GetCurItemIndex);

		if (!dragging && Mathf.Abs(sr.velocity.x) < 50) {
			SnapToObj (GetCurItemIndex);
		}

		if (lastItemIndex != GetCurItemIndex) {

            if (lastItemIndex >= 0)
            {
                items[lastItemIndex].GetComponent<Animation>().Stop();
                items[lastItemIndex].transform.localPosition = Vector3.right * 200 * lastItemIndex;
            }

            items[GetCurItemIndex].GetComponent<Animation>().Play();
            lastItemIndex = GetCurItemIndex;


            if (OnChangeItemEvent != null)
				OnChangeItemEvent (GetCurItemIndex);

		}

	}


	void FocusToObject (int index)
	{
		for (int i = 0; i < items.Length; i++) {

			float targetScale = (i == index) ? 1.6f : 1;

			items [i].transform.localScale = Vector3.Lerp (items [i].transform.localScale, Vector3.one * targetScale, Time.deltaTime * 8);
		}
	}

	public void SnapToObj (int index, bool lerp = true)
	{
		Vector3 pos = sr.content.localPosition;
		pos.x = -index * 200;
		if (lerp)
			sr.content.localPosition = Vector3.Lerp (sr.content.localPosition, pos, Time.deltaTime * 20);
		else
			sr.content.localPosition = pos;

	}

	public void OnBeginDrag (PointerEventData eventData)
	{
		dragging = true;

	}

	public void OnEndDrag (PointerEventData eventData)
	{
		dragging = false;
	}
}

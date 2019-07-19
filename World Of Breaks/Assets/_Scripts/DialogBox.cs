using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class DialogBox : MonoBehaviour {
	public Image backgroundImage;
	public Button okBtn;
	public Button cancelBtn;
	public Text title;
	public RectTransform box;

	public GUIAnim anim;

	public static DialogBox instance;

	static void Init ()
	{
		GameObject go = Instantiate (Resources.Load<GameObject> ("Prefabs/DialogBox"));
		go.transform.SetParent (FindObjectOfType<ScreenController> ().transform, false);
		instance = go.GetComponent<DialogBox> ();
		instance.anim = instance.GetComponentInChildren<GUIAnim> ();
	}

	public static void Show (string title, Action onClickOk, Action onClickCancel = null, bool OkBtnEnable = true, bool CancelBtnEnable = true, float size = -1, string btnOkText = "", string btnCancelText = "")
	{
		if (instance == null)
			Init ();
		
		if (size != -1) {
			instance.box.sizeDelta = new Vector2 (instance.box.sizeDelta.x, size);
		} else {
			instance.box.sizeDelta = new Vector2 (instance.box.sizeDelta.x, 170);
		}

		instance.backgroundImage.raycastTarget = true;
		instance.gameObject.SetActive (true);
		instance.cancelBtn.gameObject.SetActive (CancelBtnEnable);
		instance.okBtn.gameObject.SetActive (OkBtnEnable);

		if (btnOkText != "") {
			instance.okBtn.GetComponentInChildren <Text> ().text = btnOkText;
		} else {
			instance.okBtn.GetComponentInChildren <Text> ().text = "Ok";
		}

		if (btnCancelText != "") {
			instance.cancelBtn.GetComponentInChildren <Text> ().text = btnCancelText;
		} else {
			instance.cancelBtn.GetComponentInChildren <Text> ().text = "Cancel";
		}

		instance.anim.MoveIn (GUIAnimSystem.eGUIMove.SelfAndChildren);

		instance.title.text = title;

		instance.okBtn.onClick.RemoveAllListeners ();
		instance.cancelBtn.onClick.RemoveAllListeners ();

		instance.okBtn.onClick.AddListener (() => {
			if (onClickOk != null)
				onClickOk ();
			Hide ();
		});

		instance.cancelBtn.onClick.AddListener (() => {
			if (onClickCancel != null)
				onClickCancel ();
			Hide ();
		});

	}

	public static void Hide ()
	{
		instance.anim.MoveOut (GUIAnimSystem.eGUIMove.SelfAndChildren);
		instance.backgroundImage.raycastTarget = false;
	}

}

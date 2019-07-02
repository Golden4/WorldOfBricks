using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class SwitchButton : MonoBehaviour {

	Button btn;

	public SwitchInfo[] switchInfos;

	public int curSwitchIndex;

	void Start ()
	{
		btn = GetComponent <Button> ();

		btn.onClick.AddListener (() => {
			SwitchNext ();
		});
	}

	void OnEnable ()
	{
		UpdateEvents ();
		Switch (curSwitchIndex);
	}

	void Switch (int index)
	{
		transform.GetChild (0).GetComponent <Image> ().sprite = switchInfos [index].sprite;

		if (switchInfos [index].Event != null)
			switchInfos [index].Event.Invoke ();
	}

	void SwitchNext ()
	{
		curSwitchIndex = (curSwitchIndex + 1) % switchInfos.Length;
		Switch (curSwitchIndex);
	}

	public virtual void UpdateEvents ()
	{
	}

	[System.Serializable]
	public class SwitchInfo {
		public Sprite sprite;
		public Action Event;
	}

}

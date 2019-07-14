﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenBase : MonoBehaviour {
	
	protected GUIAnim[] anims;
	protected bool animate = true;

	[System.NonSerialized]
	public bool isActive;

	public void Init ()
	{
		OnInit ();

		if (animate) {
			anims = GetComponentsInChildren <GUIAnim> ();
		}
	}

	public virtual void OnInit ()
	{
		
	}

	public void Activate ()
	{
		isActive = true;
		gameObject.SetActive (true);

		if (animate && anims != null) {
			for (int i = 0; i < anims.Length; i++) {
				anims [i].MoveIn (GUIAnimSystem.eGUIMove.Self);
			}
		}

		OnActivate ();
	}

	public void Deactivate ()
	{
		if (isActive)
			OnDeactivate ();
		
		gameObject.SetActive (false);
		isActive = false;
	}

	public virtual void OnActivate ()
	{
		
	}

	public virtual void OnDeactivate ()
	{
	}

	public virtual void OnCleanUp ()
	{
	}

}

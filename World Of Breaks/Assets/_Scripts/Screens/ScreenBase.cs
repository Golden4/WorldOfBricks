using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenBase : MonoBehaviour {
	
	[System.NonSerialized]
	public bool isActive;

	public virtual void Init ()
	{
		
	}

	public void Activate ()
	{
		isActive = true;
		gameObject.SetActive (true);
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

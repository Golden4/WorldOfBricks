using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowController : MonoBehaviour {

	public Transform[] windowsList;

	void Start ()
	{
		ActivateWindow (0);
	}

	void ActivateWindow (int index)
	{
		for (int i = 0; i < windowsList.Length; i++) {
			windowsList [i].gameObject.SetActive (index == i);
		}

	}

	void Awake ()
	{
		EventManager.OnLoseEvent += OnPlayerLose;
	}

	void OnDestroy ()
	{
		EventManager.OnLoseEvent -= OnPlayerLose;
	}

	void OnPlayerLose ()
	{
		Invoke ("LoseScreen", .5f);
	}

	void LoseScreen ()
	{
		ActivateWindow (1);
	}

}

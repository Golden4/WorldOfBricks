using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class EventManager {
	public static event Action OnLoseEvent;

	public static void OnLose ()
	{
		if (OnLoseEvent != null)
			OnLoseEvent ();
	}
}

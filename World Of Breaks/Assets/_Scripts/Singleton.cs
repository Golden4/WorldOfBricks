using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonGeneric<T> : MonoBehaviour where T: MonoBehaviour {
	
	private static T instance;

	public static T Instance {
		get {
			if (SingletonGeneric<T>.instance == null) {
				SingletonGeneric<T>.instance = FindObjectOfType<T> ();
			} /*else if (SingletonGeneric<T>.instance != FindObjectOfType<T> ()) {
				Destroy (Object.FindObjectOfType<T> ());
			}*/

			return SingletonGeneric<T>.instance;
		}
	}
}

 


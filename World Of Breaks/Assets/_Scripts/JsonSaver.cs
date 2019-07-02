using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonSaver {

	public static void SaveData (string key, object obj)
	{
		PlayerPrefs.SetString (key, JsonUtility.ToJson (obj));
		PlayerPrefs.Save ();
		Debug.Log ("Saved: " + JsonUtility.ToJson (obj));
	}

	public static T LoadData<T> (string key) where T: class
	{

		T obj = null;

		if (PlayerPrefs.HasKey (key)) {
			obj = JsonUtility.FromJson<T> (PlayerPrefs.GetString (key));
			Debug.Log ("Loaded: " + PlayerPrefs.GetString (key));
		}


		return obj;
	}

}

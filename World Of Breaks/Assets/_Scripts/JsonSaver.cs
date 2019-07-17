using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonSaver {

	public static void SaveData (string key, object obj)
	{
		ZPlayerPrefs.SetString (key, JsonUtility.ToJson (obj));
		Debug.Log ("Saved: " + JsonUtility.ToJson (obj));
	}

	public static T LoadData<T> (string key) where T: class
	{
		T obj = null;

		if (ZPlayerPrefs.HasKey (key)) {
			obj = JsonUtility.FromJson<T> (ZPlayerPrefs.GetString (key));
			Debug.Log (obj.ToString ());

		}


		return obj;
	}

}

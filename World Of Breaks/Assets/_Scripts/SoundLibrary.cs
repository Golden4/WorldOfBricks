using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundLibrary : MonoBehaviour {

	public SoundArray[] sounds;

	public Dictionary<string, SoundArray> soundsDic = new Dictionary<string, SoundArray> ();

	void Awake ()
	{
		for (int i = 0; i < sounds.Length; i++) {
			soundsDic [sounds [i].name] = sounds [i];
		}
	}

	public AudioClip GetSoundByName (string soundName)
	{
		SoundArray array;

		if (soundsDic.TryGetValue (soundName, out array)) {

			int randomIndex = Random.Range (0, array.sounds.Length);

			return array.sounds [randomIndex];


		} else {
			Debug.Log ("Sound Not Found!!" + soundName);
			return null;
		}


	}


	[System.Serializable]
	public class SoundArray {
		public string name;
		public AudioClip[] sounds;
	}

}

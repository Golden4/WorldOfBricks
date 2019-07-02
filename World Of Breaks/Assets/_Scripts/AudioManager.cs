using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour {
	public static AudioManager Ins;

	AudioSource source;

	public SoundLibrary soundLibrary;

	void Awake ()
	{
		if (Ins == null) {
			Ins = this;
			DontDestroyOnLoad (gameObject);
		} else if (Ins != this) {
			Destroy (gameObject);
			return;
		}

		source = gameObject.AddComponent<AudioSource> ();

		soundLibrary = GetComponent <SoundLibrary> ();
	}

	public static void PlaySound (Sound sound)
	{
		Ins.PlaySound (sound.clip);
	}

	public void PlaySound (AudioClip sound)
	{
		if (!audioEnabled)
			return;
		
		source.PlayOneShot (sound);
	}

	public static bool audioEnabled = true;

	public static void EnableAudio (bool enable)
	{
		audioEnabled = enable;

		AudioListener.volume = enable ? 1 : 0;

		/*if (listener != null)
			listener.enabled = enable;*/

//		print ("Audio " + enable);
	}

	public static void PlaySoundFromLibrary (string name)
	{
		Ins.PlaySound (Ins.soundLibrary.GetSoundByName (name));
	}

	public static void PlaySoundAtPosition (Sound sound, Vector3 pos)
	{
		AudioSource.PlayClipAtPoint (sound.clip, pos);
	}

	public static void PlaySoundAtObject (Sound sound, GameObject obj)
	{
		string name = sound.name;
		if (name == "") {
			name = "sound";
		}

		GameObject go = new GameObject (name, typeof(AudioSource));

		go.transform.SetParent (obj.transform, false);

		AudioSource source = go.GetComponent <AudioSource> ();

		source.clip = sound.clip;
		source.loop = sound.loop;

		source.Play ();
	}

}

[System.Serializable]
public class Sound {
	public string name = "Sound";
	public AudioClip clip;
	public bool loop;

	public Sound (AudioClip clip)
	{
		this.clip = clip;
		loop = false;
	}

	public Sound (string name, AudioClip clip, bool loop)
	{
		this.name = name;
		this.clip = clip;
		this.loop = loop;
	}

	public void Play ()
	{
		AudioManager.PlaySound (this);
	}

	public void PlayAtObject (GameObject go)
	{
		AudioManager.PlaySoundAtObject (this, go);
	}
	
}

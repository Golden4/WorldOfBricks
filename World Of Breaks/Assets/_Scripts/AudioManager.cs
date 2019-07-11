using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour {
	private static AudioManager _Ins;

	public static AudioManager Ins {
		get {
			if (_Ins == null) {
				GameObject manager = Resources.Load ("Prefabs/AudioManager") as GameObject;
				AudioManager audioManager = Instantiate (manager).GetComponent<AudioManager> ();
				_Ins = audioManager;
			}

			return _Ins;
		}
	}

	AudioSource[] source = new AudioSource[5];

	public SoundLibrary soundLibrary;

	void Awake ()
	{
		if (_Ins == null) {
			DontDestroyOnLoad (gameObject);
			_Ins = this;
		} else if (this != _Ins) {
			Destroy (gameObject);
			return;
		}

		for (int i = 0; i < 5; i++) {
			source [i] = gameObject.AddComponent<AudioSource> ();
		}

		soundLibrary = GetComponent<SoundLibrary> ();

	}

	public static void PlaySound (Sound sound)
	{
		if (!audioEnabled)
			return;
		
		sound.PlaySound (Ins.GetAvaibleSource ());
	}

	public AudioSource GetAvaibleSource ()
	{
		for (int i = 0; i < source.Length; i++) {
			if (!source [i].isPlaying) {
				return source [i];
			}
		}
		return source [0];
	}

	public void PlaySound (AudioClip sound)
	{
		if (!audioEnabled)
			return;

		source [0].PlayOneShot (sound);
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

	static float lastPlayTime;

	public static void PlaySoundFromLibrary (string name)
	{
		try {
			lastPlayTime = Time.time;
			Sound sound = Ins.soundLibrary.GetSoundByName (name);
			PlaySound (sound);

		} catch (System.Exception except) {
			Debug.LogError (except);
			throw;
		}
		
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

	[Range (0, 1.5f)]
	public float volume = 0.7f;

	[Range (0.5f, 1.5f)]
	public float pitch = 1f;

	[Range (0f, 0.5f)]
	public float randomVolume = 0.1f;

	[Range (0f, 0.5f)]
	public float randomPitch = 0.1f;

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

	public void PlaySound (AudioSource source)
	{
		source.clip = clip;
		source.volume = volume * (1 + Random.Range (-randomVolume / 2f, randomVolume / 2f));
		source.pitch = pitch * (1 + Random.Range (-randomPitch / 2f, randomPitch / 2f));
		source.Play ();
	}

}

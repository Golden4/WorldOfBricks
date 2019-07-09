//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class AudioManager : MonoBehaviour {

//	public float masterVolumePercent = 1f;
//	public float sfxVolumePercent = 1f;
//	public float musicVolumePercent = 1f;

//	public static bool musicEnabled = true;
//	public static bool sfxEnabled = true;

//	int activeMusicSourceIndex;

//	bool startedMusic;
//	bool startedSFX;

//    public SoundLibrary library;

//	private static AudioManager StaticInstance;

//	public static AudioManager Instance {
//		get {
//			if (StaticInstance == null) {
//				GameObject manager = Resources.Load ("Prefabs/AudioManager") as GameObject;
//				AudioManager audioManager = Instantiate (manager).GetComponent<AudioManager> ();
//				DontDestroyOnLoad (audioManager);
//				StaticInstance = audioManager;
//			}

//			return StaticInstance;
//		}
//	}

//	AudioSource[] musicSources;
//	AudioSource sfxSource;

//	void Start ()
//	{
//		if (StaticInstance == null) {
//			DontDestroyOnLoad (gameObject);
//			StaticInstance = this;
//		} else if (this != StaticInstance) {
//			Destroy (gameObject);
//		}
//	}

//	public void PauseSounds ()
//	{
//		foreach (AudioSource source in musicSources) {
//			source.Pause ();
//		}

//		sfxSource.Pause ();
//	}

//	public void UnpauseSounds ()
//	{
//		foreach (AudioSource source in musicSources) {
//			source.UnPause ();
//		}

//		sfxSource.UnPause ();
//	}

//	public void StopSounds ()
//	{
//		foreach (AudioSource source in musicSources) {
//			source.Stop ();
//		}

//		sfxSource.Stop ();
//	}

//	public void StartSounds ()
//	{
//		foreach (AudioSource source in musicSources) {
//			source.Play ();
//		}

//		sfxSource.Play ();
//	}

//	void Update ()
//	{

//	}

//	void CreateSourses ()
//	{
//		musicSources = new AudioSource[2];

//		for (int i = 0; i < 2; i++) {
//			GameObject go = new GameObject ("MusicSource" + (i + 1));
//			musicSources [i] = go.AddComponent<AudioSource> ();
//			musicSources [i].loop = true;
//			musicSources [i].transform.parent = transform;
//		}

//		GameObject newGO = new GameObject ("SfxSource");
//		sfxSource = newGO.AddComponent<AudioSource> ();
//		newGO.transform.parent = transform;

//	}

//	public void PlayMusic (AudioClip clip)
//	{
//		if (musicSources == null) {
//			CreateSourses ();
//		}

//		if (!musicEnabled)
//			return;

//		activeMusicSourceIndex = 1 - activeMusicSourceIndex;

//		musicSources [activeMusicSourceIndex].clip = clip;
//		musicSources [activeMusicSourceIndex].volume = masterVolumePercent * musicVolumePercent;
//		musicSources [activeMusicSourceIndex].Play ();
//	}

//	//public void PlayMusic (string name)
//	//{

//	//	if (!musicEnabled)
//	//		return;

//	//	if (!GetComponent<MusicManager> ().playMusic)
//	//		return;

//	//	if (name == "Shop") {
//	//		PlayMusic (GetComponent<MusicManager> ().shopTheme);
//	//	}

//	//	if (name == "Main") {
//	//		PlayMusic (GetComponent<MusicManager> ().mainTheme);
//	//	}
//	//}

//	public void StopMusic ()
//	{

//		for (int i = 0; i < musicSources.Length; i++) {
//			musicSources [i].Stop ();
//			musicSources [i].clip = null;
//		}
//	}

//	public void PlaySound (AudioClip clip, Vector3 pos)
//	{
//		if (!sfxEnabled)
//			return;

//		AudioSource.PlayClipAtPoint (clip, pos, sfxVolumePercent * masterVolumePercent);
//	}

//	public void PlaySound (string clipName, Vector3 pos)
//	{
//		if (!sfxEnabled)
//			return;
//		PlayClipAtPoint (library.GetRandomClipFromName (clipName), pos, sfxVolumePercent * masterVolumePercent);

//	}

//	public void PlaySound2D (string clipName)
//	{
//		if (!sfxEnabled)
//			return;
//        library.GetRandomClipFromName (clipName).PlaySound (sfxSource);

//		//sfxSource.PlayOneShot (SoundLibrary.Instance.GetRandomClipFromName (clipName).clip, sfxVolumePercent * masterVolumePercent);
//	}


//	public void PlaySound2D (Sound clip)
//	{
//		if (!sfxEnabled)
//			return;
//		clip.PlaySound (sfxSource);

//		//sfxSource.PlayOneShot (SoundLibrary.Instance.GetRandomClipFromName (clipName).clip, sfxVolumePercent * masterVolumePercent);
//	}

//	public void PlayClipAtPoint (Sound sound, Vector3 pos, float volume)
//	{
//		if (!sfxEnabled)
//			return;

//		GameObject go = new GameObject ("Playing " + sound.name);
//		go.transform.position = pos;

//		AudioSource source = go.AddComponent<AudioSource> ();

//		source.volume = volume;

//		sound.PlaySound (source);

//		Destroy (go, sound.clip.length);

//	}

//}

//[System.Serializable]
//public class Sound {
//	public string name;
//	public AudioClip clip;

//	[Range (0, 1.5f)]
//	public float volume = 0.7f;

//	[Range (0.5f, 1.5f)]
//	public float pitch = 1f;

//	[Range (0f, 0.5f)]
//	public float randomVolume = 0.1f;

//	[Range (0f, 0.5f)]
//	public float randomPitch = 0.1f;

//	AudioSource source;

//	public void PlaySound (AudioSource source)
//	{
//		source.clip = clip;
//		source.volume = volume * (1 + Random.Range (-randomVolume / 2f, randomVolume / 2f));
//		source.pitch = pitch * (1 + Random.Range (-randomPitch / 2f, randomPitch / 2f));
//		source.Play ();
//	}

//}

	

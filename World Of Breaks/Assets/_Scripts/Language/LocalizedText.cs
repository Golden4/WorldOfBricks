using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
	private Text text;
	[SerializeField]
	string key;

	public string Key
	{
		get
		{
			return key;
		}
		set
		{
			key = value;
			if (!string.IsNullOrEmpty (key))
				text.text = LocalizationManager.GetLocalizedText (key);
		}
	}

	void Awake ()
	{
		LocalizationManager.OnLanguageChangeEvent += Start;
		text = GetComponent<Text> ();
	}

	void Start ()
	{
		if (!string.IsNullOrEmpty (key))
			text.text = LocalizationManager.GetLocalizedText (key);
		text.font = LocalizationManager.curLanguage.font;
		text.fontStyle = LocalizationManager.curLanguage.fontStyle;
		//		text.GetComponent <Renderer> ().sharedMaterial = LocalizationManager.GetLocalizedFont ().material;
	}

	void OnDestroy ()
	{
		LocalizationManager.OnLanguageChangeEvent -= Start;
	}



}

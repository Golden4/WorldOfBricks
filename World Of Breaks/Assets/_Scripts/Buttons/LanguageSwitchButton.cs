using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LanguageSwitchButton : SwitchButton {

	public override void UpdateEvents ()
	{
		base.UpdateEvents ();

		curSwitchIndex = LanguagesList.GetLanguageIndex (LocalizationManager.curLanguage.systemLanguage);

		Action[] actions = new Action[LanguagesList.Count ()];

		for (int i = 0; i < LanguagesList.Count (); i++) {
			SystemLanguage lang = LanguagesList.languages [i].systemLanguage;
			actions [i] = delegate() {
				Debug.Log (lang);
				LocalizationManager.ChangeLanguage (lang);
			};
		}

		for (int i = 0; i < switchInfos.Length; i++) {
			switchInfos [i].Event = actions [i];
		}
	}
}

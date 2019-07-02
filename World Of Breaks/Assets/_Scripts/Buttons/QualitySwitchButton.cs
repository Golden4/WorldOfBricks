using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class QualitySwitchButton : SwitchButton {
	public override void UpdateEvents ()
	{
		base.UpdateEvents ();
		
		curSwitchIndex = (QualitySettings.GetQualityLevel () == 0) ? 0 : 1;

		Action[] actions = {
			delegate {
				QualitySettings.SetQualityLevel (0);
			}, delegate {
				QualitySettings.SetQualityLevel (QualitySettings.names.Length - 1);
			}
		};

		for (int i = 0; i < switchInfos.Length; i++) {
			switchInfos [i].Event = actions [i];
		}
	}
}

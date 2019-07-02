using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioSwitchButton : SwitchButton {

	public override void UpdateEvents ()
	{
		base.UpdateEvents ();
		curSwitchIndex = (AudioManager.audioEnabled) ? 0 : 1;

		Action[] actions = {
			delegate {
				AudioManager.EnableAudio (true);
			}, delegate {
				AudioManager.EnableAudio (false);
			}
		};

		for (int i = 0; i < switchInfos.Length; i++) {
			switchInfos [i].Event = actions [i];
		}
	}

}

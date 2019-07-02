using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputSwitchButton : SwitchButton {
	public override void UpdateEvents ()
	{
		base.UpdateEvents ();
		curSwitchIndex = (InputMobileController.curInputType == InputMobileController.InputType.Touch) ? 0 : 1;

		Action[] actions = {
			delegate {
				InputMobileController.ChangeInputType (InputMobileController.InputType.Touch);
			}, delegate {
				InputMobileController.ChangeInputType (InputMobileController.InputType.InverseTouch);
			}
		};

		for (int i = 0; i < switchInfos.Length; i++) {
			switchInfos [i].Event = actions [i];
		}
	}
}

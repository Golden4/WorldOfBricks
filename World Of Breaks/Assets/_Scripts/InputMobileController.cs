using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputMobileController : MonoBehaviour {

	public static InputMobileController Ins;

	public static InputType curInputType;
    
	public Button inputInfoBtn;

	public enum InputType
	{
        Touch,
        InverseTouch
    }

	void Awake ()
	{
		Ins = this;
	}

	public static void ChangeInputType (InputType type)
	{
		curInputType = type;
		Debug.Log ("CurInputType: " + type);
	}
}

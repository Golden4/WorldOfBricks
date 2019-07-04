using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputMobileController : MonoBehaviour {
    
	public static InputType curInputType;

	public enum InputType
	{
        Touch,
        InverseTouch
    }

	public static void ChangeInputType (InputType type)
	{
		curInputType = type;
		Debug.Log ("CurInputType: " + type);
	}
}

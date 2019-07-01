using UnityEngine;
using System.Collections;

public class MoveSample : MonoBehaviour {
	
	void Start ()
	{
		//iTween.MoveBy (gameObject, iTween.Hash ("x", 5, "easeType", "Linear", "loopType", "pingPong", "delay", .1));

		Vector3[] vectores = new Vector3[3];
		vectores [0] = Vector3.up * 2;
		vectores [1] = Vector3.forward * 2;
		vectores [2] = Vector3.zero;
		//iTween.DrawLine (vectores, Color.white);
		//iTween.MoveTo (gameObject, iTween.Hash ("y", 5, "easeType", "Linear", "loopType", "pingPong", "delay", .1));

		iTween.ValueTo (gameObject, iTween.Hash (
			"from", 15,
			"to", 10,
			"time", 5
		));

	}
}


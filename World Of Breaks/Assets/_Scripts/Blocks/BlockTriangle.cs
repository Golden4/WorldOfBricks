using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockTriangle : BlockWithText {


	protected override void Start ()
	{
		base.Start ();

		//int randomRotate = Random.Range (0, 5);
		//transform.GetChild (0).eulerAngles = new Vector3 (0, 0, randomRotate * 90);

		textMesh.transform.rotation = Quaternion.Euler (Vector3.zero);

		/*spriteRenderer = GetComponentInChildren<SpriteRenderer> ();
		textMesh = GetComponentInChildren<TextMesh> ();*/
	}
}

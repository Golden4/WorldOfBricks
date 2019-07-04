using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPlusBall : BlockWithText {
    protected override void Start ()
	{
		
	}

	public override void Hit ()
	{
		BallController.Instance.BallCountPlus ();
		Die ();
	}

    protected override void OnDead ()
	{
        BlocksController.Instance.blockMap [coordsY] [coordsX].blockLife = 0;

		Transform text = transform.GetChild (0);
		text.SetParent (null, false);
		text.position = transform.position + (Vector3.up - Vector3.left) * .5f;
		text.gameObject.SetActive (true);

		iTween.MoveTo (text.gameObject, text.position + Vector3.up * .3f, 1f);
		iTween.FadeTo (text.gameObject, 0, .5f);

		Destroy (text.gameObject, .5f);

		Destroy (gameObject);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockObstacle : BlockWithText {

	bool needDestroy  = false;

	protected override void Start ()
	{
		BlocksController.Instance.OnChangeTopLine += Dead;
	}

	public override void Hit (Ball ball)
	{
		needDestroy = true;
		Vector3 dir = Vector3.up + Vector3.left * Random.Range (-2f, 2f);
		ball.ChangeDirection (dir.normalized);
	}

	void OnDestroy ()
	{
		BlocksController.Instance.OnChangeTopLine -= Dead;
	}

	public override void Dead ()
	{
		if (needDestroy)
			Destroy (gameObject);
	}
}

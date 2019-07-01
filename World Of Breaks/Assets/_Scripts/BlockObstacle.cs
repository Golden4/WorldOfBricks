using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockObstacle : BlockWithText {

	protected override void Start ()
	{
		StartCoroutine (BlocksController.Instance.DestroyBlockWhenLevelEnd (gameObject, Random.Range (2, 10)));
	}

	public override void Hit (Ball ball)
	{
		base.Hit (ball);

		ball.ChangeDirection (Vector3.up);
	}

	public override void Dead ()
	{
		
	}
}

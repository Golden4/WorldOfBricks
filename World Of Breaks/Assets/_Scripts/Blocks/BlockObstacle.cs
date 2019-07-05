using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockObstacle : BlockWithText {

	bool needDestroy = false;

    protected override void Start()
    {
        BlocksController.Instance.OnChangeTopLine += TryDie;
    }

    public override void Hit (Ball ball)
	{
        t = 1;
        needDestroy = true;
		Vector3 dir = Vector3.up + Vector3.left * Random.Range (-2f, 2f);
		ball.ChangeDirection (dir.normalized);
	}

    protected override void TimerStart()
    {
        transform.GetChild(0).localScale = new Vector3(1 + t / 3f, 1 + t / 3f, 1);
    }

    protected override void TimerEnd()
    {
        transform.GetChild(0).localScale = Vector3.one;
    }

    void OnDestroy ()
	{
		BlocksController.Instance.OnChangeTopLine -= TryDie;
	}

    void TryDie()
    {
        if (needDestroy)
            Die();
    }

    protected override void OnDead ()
	{
		Destroy (gameObject);
	}
}

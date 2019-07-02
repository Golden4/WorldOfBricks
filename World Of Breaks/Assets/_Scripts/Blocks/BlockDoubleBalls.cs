using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDoubleBalls : BlockWithText
{
    bool needDestroy;

    protected override void Start()
    {
        BlocksController.Instance.OnChangeTopLine += Dead;
    }

    public override void Hit(Ball ball)
    {
        if (ball.isClone)
            return;

        needDestroy = true;
        Transform ballTmp = Instantiate<GameObject>(ball.gameObject).transform;
        ballTmp.position = ball.transform.position;
        Ball ballClone = ballTmp.GetComponent<Ball>();
        ballClone.isClone = true;
        ballClone.GetComponent<SpriteRenderer>().color = Color.blue;
        BallController.Instance.ballsList.Add(ballClone);


        Vector3 dir = ball.transform.up + Vector3.left * -1;
        ball.ChangeDirection(dir.normalized);
        dir = ball.transform.up + Vector3.left * 1;
        ballClone.ChangeDirection(dir.normalized);
    }

    void OnDestroy()
    {
        BlocksController.Instance.OnChangeTopLine -= Dead;
    }

    public override void Dead()
    {
        if (needDestroy)
            Destroy(gameObject);
    }
}

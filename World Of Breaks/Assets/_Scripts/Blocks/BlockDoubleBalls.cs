using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDoubleBalls : BlockWithText
{
    bool needDestroy;

    protected override void Start()
    {
        BlocksController.Instance.OnChangeTopLine += TryDie;
    }

    float t;
    void Update()
    {
        if (t > 0)
        {
            t -= Time.deltaTime * 6;


            transform.GetChild(0).localScale = new Vector3(1 + t / 3f, 1 + t / 3f, 1);


        }
        else if (t <= -0.01f)
        {
            t = 0;
            transform.GetChild(0).localScale = Vector3.one;
        }

    }

    public override void Hit(Ball ball)
    {
        if (ball.isClone)
            return;
        t = 1;
        needDestroy = true;
        Transform ballTmp = Instantiate<GameObject>(ball.gameObject).transform;
        ballTmp.position = ball.transform.position;
        Ball ballClone = ballTmp.GetComponent<Ball>();
        ballClone.ChangeToClone();
        BallController.Instance.ballsList.Add(ballClone);
        
        Vector3 dir = ball.transform.up + Vector3.left * -1;
        ball.ChangeDirection(dir.normalized);
        dir = ball.transform.up + Vector3.left * 1;
        ballClone.ChangeDirection(dir.normalized);
    }

    void TryDie()
    {
        if (needDestroy)
            Die();
    }

    void OnDestroy()
    {
        BlocksController.Instance.OnChangeTopLine -= TryDie;
    }

    protected override void OnDead()
    {
        Destroy(gameObject);
    }
}

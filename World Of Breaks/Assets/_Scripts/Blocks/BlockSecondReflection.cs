using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSecondReflection : BlockWithText
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

        if (!ball.isSecondReflection)
            ball.ChangeToSecondReflection();
        
    }

    void OnDestroy()
    {
        BlocksController.Instance.OnChangeTopLine -= TryDie;
    }


    void TryDie()
    {
        if (needDestroy)
            Die();
    }

    protected override void OnDead()
    {
        Destroy(gameObject);
    }
}

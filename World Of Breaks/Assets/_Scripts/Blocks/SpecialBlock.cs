using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SpecialBlock : BlockWithText
{
    protected bool needDestroy = false;

    protected override void Start()
    {
        BlocksController.Instance.OnChangeTopLine += TryDie;
    }

    protected override void OnDestroy()
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
        transform.GetChild(0).transform.DOScale(Vector3.zero, .2f);
        Destroy(gameObject, .2f);
    }

    protected override void TimerStart()
    {
        transform.GetChild(0).localScale = new Vector3(1 + t / 3f, 1 + t / 3f, 1);
    }

    protected override void TimerEnd()
    {
        transform.GetChild(0).localScale = Vector3.one;
    }
}
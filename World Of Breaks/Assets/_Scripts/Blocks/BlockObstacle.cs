using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BlockObstacle : BlockWithText
{

    bool needDestroy = false;

    protected override void Start()
    {
        BlocksController.Instance.OnChangeTopLine += TryDie;
    }

    static float lastAudioPlayTime;

    public override void Hit(Ball ball)
    {
        t = 1;
        needDestroy = true;
        BlocksController.Instance.blockMap[coordsY][coordsX].blockIndex = -1;

        Vector3 dir = Vector3.up + Vector3.left * Random.Range(-2f, 2f);
        ball.ChangeDirection(dir.normalized);

        if (lastAudioPlayTime + .05f < Time.time)
        {
            lastAudioPlayTime = Time.time;
            AudioManager.PlaySoundFromLibrary("Obstacle");
        }

    }

    protected override void TimerStart()
    {
        transform.GetChild(0).localScale = new Vector3(1 + t / 3f, 1 + t / 3f, 1);
    }

    protected override void TimerEnd()
    {
        transform.GetChild(0).localScale = Vector3.one;
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
        transform.GetChild(0).transform.DOScale(Vector3.zero, .2f);
        // iTween.ScaleTo(transform.GetChild(0).gameObject, Vector3.zero, .2f);
        Destroy(gameObject, .2f);
    }
}

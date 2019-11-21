using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BlockObstacle : SpecialBlock
{

    static float lastAudioPlayTime;

    public bool allDirections;

    public override void Hit(Ball ball)
    {
        t = 1;
        needDestroy = true;
        BlocksController.Instance.blockMap[coordsY][coordsX].blockIndex = -1;
        Vector3 dir;

        if (!allDirections)
        {
            dir = Vector3.up + Vector3.left * Random.Range(-2f, 2f);
        }
        else
        {
            dir = Random.insideUnitCircle;
        }

        ball.ChangeDirection(dir.normalized);

        if (lastAudioPlayTime + .05f < Time.time)
        {
            lastAudioPlayTime = Time.time;
            AudioManager.PlaySoundFromLibrary("Obstacle");
        }

    }
}

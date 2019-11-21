using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BlockSkull : SpecialBlock
{

    static float lastAudioPlayTime;

    public override void Hit(Ball ball)
    {
        if (Ball.HaveAblity(Ball.Ability.SkullDestroy))
        {
            Die();
            return;
        }

        needDestroy = true;
        BlocksController.Instance.blockMap[coordsY][coordsX].blockIndex = -1;
        t = 1;

        ball.ReturnBall(false);

        if (lastAudioPlayTime + .05f < Time.time)
        {
            lastAudioPlayTime = Time.time;
            AudioManager.PlaySoundFromLibrary("Skull");
        }

    }
}
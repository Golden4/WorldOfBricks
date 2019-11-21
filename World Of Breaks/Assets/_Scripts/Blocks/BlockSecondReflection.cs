using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BlockSecondReflection : SpecialBlock
{
    static float lastAudioPlayTime;

    public override void Hit(Ball ball)
    {
        if (ball.isClone)
            return;
        t = 1;
        needDestroy = true;
        BlocksController.Instance.blockMap[coordsY][coordsX].blockIndex = -1;

        if (ball.canReflectionCount == 0)
            ball.ChangeToSecondReflection();

        if (lastAudioPlayTime + .05f < Time.time)
        {
            lastAudioPlayTime = Time.time;
            AudioManager.PlaySoundFromLibrary("Reflection");
        }

    }
}

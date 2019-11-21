using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGhost : SpecialBlock
{
    static float lastAudioPlayTime;

    public override void Hit(Ball ball)
    {
        t = 1;

        needDestroy = true;

        BlocksController.Instance.blockMap[coordsY][coordsX].blockIndex = -1;
        ball.ChangeToGhost();

        if (lastAudioPlayTime + .05f < Time.time)
        {
            lastAudioPlayTime = Time.time;
            AudioManager.PlaySoundFromLibrary("Ghost");
        }

    }
}

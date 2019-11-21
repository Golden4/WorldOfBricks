using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BlockDoubleBalls : SpecialBlock
{
    static float lastAudioPlayTime;

    public override void Hit(Ball ball)
    {
        if (ball.isClone)
            return;

        t = 1;
        needDestroy = true;
        BlocksController.Instance.blockMap[coordsY][coordsX].blockIndex = -1;

        float angle = ball.transform.localEulerAngles.z;

        int cloneCount = 1;

        if (Ball.HaveAblity(Ball.Ability.Clone2Times))
            cloneCount = 2;

        for (int i = 0; i < cloneCount; i++)
        {
            Transform ballTmp = Instantiate<GameObject>(ball.gameObject).transform;
            ballTmp.position = ball.transform.position;
            Ball ballClone = ballTmp.GetComponent<Ball>();
            ballClone.ChangeToClone();
            BallController.Instance.ballsList.Add(ballClone);

            if (i == 1)
            {
                ballClone.ChangeDirection(angle);
            }
            else
            {
                ballClone.ChangeDirection(angle + 30);
            }

        }

        ball.ChangeDirection(angle - 30);


        if (lastAudioPlayTime + .05f < Time.time)
        {
            lastAudioPlayTime = Time.time;
            AudioManager.PlaySoundFromLibrary("DoubleBall");
        }

    }
}

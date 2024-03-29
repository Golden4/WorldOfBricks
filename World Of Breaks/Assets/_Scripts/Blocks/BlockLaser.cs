﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BlockLaser : SpecialBlock
{

    public enum LaserType
    {
        Horizontal,
        Vertical,
        HorizontalAndVerical
    }

    public LaserType typeOfLaserBlock;

    public SpriteRenderer laserVer;
    public SpriteRenderer laserHor;

    protected override void Start()
    {
        base.Start();
        t = 0;
        timerSpeed = 3;
    }

    protected override void TimerStart()
    {
        transform.GetChild(0).localScale = new Vector3(1 + t / 3f, 1 + t / 3f, 1);
        if (laserVer != null && (typeOfLaserBlock == LaserType.Vertical || typeOfLaserBlock == LaserType.HorizontalAndVerical))
        {
            if (!laserVer.gameObject.activeInHierarchy)
                laserVer.gameObject.SetActive(true);

            laserVer.transform.localScale = new Vector3(t, 1, 1);

        }

        if (laserHor != null && (typeOfLaserBlock == LaserType.Horizontal || typeOfLaserBlock == LaserType.HorizontalAndVerical))
        {
            if (!laserHor.gameObject.activeInHierarchy)
                laserHor.gameObject.SetActive(true);

            laserHor.transform.localScale = new Vector3(1, t, 1);
        }
    }

    protected override void TimerEnd()
    {
        transform.GetChild(0).localScale = new Vector3(1, 1, 1);
        if (laserHor != null)
            if (laserHor.gameObject.activeInHierarchy)
                laserHor.gameObject.SetActive(false);

        if (laserVer != null)
            if (laserVer.gameObject.activeInHierarchy)
                laserVer.gameObject.SetActive(false);
    }

    static float lastAudioPlayTime;

    public override void Hit()
    {
        needDestroy = true;
        BlocksController.Instance.blockMap[coordsY][coordsX].blockIndex = -1;
        t = 1;
        if (typeOfLaserBlock == LaserType.Horizontal || typeOfLaserBlock == LaserType.HorizontalAndVerical)
        {

            for (int i = 0; i < BlocksController.Instance.blockMap[0].Length; i++)
            {

                BlockWithText block = BlocksController.Instance.blockMap[coordsY][i].blockComp;

                if (block != null && block.GetType() != typeof(BlockLaser) && block.canLooseDown)
                    block.Hit();

            }
        }
        if (lastAudioPlayTime + .05f < Time.time)
        {
            lastAudioPlayTime = Time.time;
            AudioManager.PlaySoundFromLibrary("Laser");
        }

        if (typeOfLaserBlock == LaserType.Vertical || typeOfLaserBlock == LaserType.HorizontalAndVerical)
        {

            for (int i = 0; i < BlocksController.Instance.blockMap.Length; i++)
            {

                BlockWithText block = BlocksController.Instance.blockMap[i][coordsX].blockComp;

                if (block != null && block.GetType() != typeof(BlockLaser) && block.canLooseDown)
                    block.Hit();

            }
        }

    }
}

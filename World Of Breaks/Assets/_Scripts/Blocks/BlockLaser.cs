using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockLaser : BlockWithText {

	public enum LaserType
	{
		Horizontal,
		Vertical,
		HorizontalAndVerical
	}

	public LaserType typeOfLaserBlock;
	bool needDestroy;

	protected override void Start ()
	{
		BlocksController.Instance.OnChangeTopLine += Dead;
	}

	public override void Hit ()
	{
		needDestroy = true;
		if (typeOfLaserBlock == LaserType.Horizontal) {
			
			for (int i = 0; i < BlocksController.Instance.blockMap [0].Length; i++) {

				BlockWithText block = BlocksController.Instance.blockMap [coordsY] [i].blockComp;

				if (block != null && block.GetType () != typeof(BlockLaser))
					block.Hit ();
				
			}
		}

		//if(typeOfLaserBlock == la)
	}

	void OnDestroy ()
	{
		BlocksController.Instance.OnChangeTopLine -= Dead;
	}

	public override void Dead ()
	{
		if (needDestroy)
			Destroy (gameObject);
	}




}

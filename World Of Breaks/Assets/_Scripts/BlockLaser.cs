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

	protected override void Start ()
	{

	}

	public override void Hit ()
	{

		if (typeOfLaserBlock == LaserType.Horizontal) {
			
			for (int i = 0; i < BlocksController.Instance.blockMap [0].Length; i++) {

				BlockWithText block = BlocksController.Instance.blockMap [coordsY] [i].blockComp;

				if (block != null && block.GetType () != typeof(BlockLaser))
					block.Hit ();
				
			}
		}

		print ("dsadad");

		//if(typeOfLaserBlock == la)
	}

	public override void Dead ()
	{
		Destroy (gameObject);
	}




}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockExplosion : BlockWithText
{
    protected override void Start()
    {
        base.Start();
        if (!isLoadingBlock)
        {
            int blockLife = BlocksController.Instance.blockMap[coordsY][coordsX].blockLife;
            blockLife = Mathf.Clamp(blockLife + Random.Range(0, blockLife), 1, blockLife * 2);
            BlocksController.Instance.blockMap[coordsY][coordsX].blockLife = blockLife;
            UpdateText();
        }
    }

    protected override void OnDead()
    {
        bool[,] pattentToDestroy = new bool[,] {
           { false, false, true, false, false },
           { false, true, true, true, false },
           { true, true, true, true, true },
           { false, true, true, true, false },
           { false, false, true, false, false }
       };

        for (int i = 0; i < pattentToDestroy.GetLength(0); i++)
        {
            for (int j = 0; j < pattentToDestroy.GetLength(1); j++)
            {
                if (pattentToDestroy[i, j])
                {
                    int x = coordsX - 2 + i;
                    int y = coordsY - 2 + j;

                    if (x >= 0 && x < BlocksController.Instance.blockMap[0].Length && y >= 0 && y < BlocksController.Instance.blockMap.Length)
                    {

                        Vector3 pos = BlocksController.Instance.ConvertCoordToPos(y-1, x);

                        Destroy(Instantiate<GameObject>(destroyParticle.gameObject, pos, Quaternion.identity), 2);

                        BlockWithText block = BlocksController.Instance.blockMap[y][x].blockComp;

                        if (block != null && block.canLooseBeforeDown)
                        {  
                            block.Die();
                        }
                    }
                }
            }
        }

        BlocksController.Instance.blockMap[coordsY][coordsX].blockLife = 0;
        BlocksController.Instance.CalculateBlockLife();

        
        Destroy(gameObject);

    }
}

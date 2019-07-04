using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRandom : BlockWithText
{
    protected override void Start()
    {
        base.Start();
        if (!isLoadingBlock)
        {
            int blockLife = BlocksController.Instance.blockMap[coordsY][coordsX].blockLife;
            blockLife = Mathf.Clamp(blockLife + Random.Range(-blockLife / 5, blockLife * 2), 1, blockLife * 2);
            BlocksController.Instance.blockMap[coordsY][coordsX].blockLife = blockLife;
        }
    }

    public override void UpdateText()
    {
        textMesh.text = "?";
    }
}

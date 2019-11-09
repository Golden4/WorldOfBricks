using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRandom : BlockWithText
{

    public Gradient g;
    public Gradient g2;

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

    protected override void ChangeSpriteColor(float value)
    {
        spriteRenderer.material.SetColor("_TopColor", g.Evaluate(value));
        spriteRenderer.material.SetColor("_BottomColor", g2.Evaluate(value));

    }

    public override void UpdateText()
    {
        textMesh.text = "?";
    }
}

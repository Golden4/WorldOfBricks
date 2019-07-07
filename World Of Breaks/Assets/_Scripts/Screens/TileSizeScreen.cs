using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileSizeScreen : ScreenBase
{

    public Button[] tileSizeBtns;

    public enum TileSize
    {
        Small,
        Medium,
        Big
    }

    public static TileSize tileSize;

    public void SetTileSize(TileSize size)
    {
        tileSize = size;
        MenuScreen.Ins.StartGame(true, false);
    }

    private void Start()
    {
        for (int i = 0; i < tileSizeBtns.Length; i++)
        {
            int index = i;
            tileSizeBtns[i].onClick.AddListener(delegate
            {
                SetTileSize((TileSize)index);
            });
        }

       

    }
    public override void OnActivate()
    {
        base.OnActivate();
        MenuScreen.Ins.ShowGameTitle(true, false);
    }

}

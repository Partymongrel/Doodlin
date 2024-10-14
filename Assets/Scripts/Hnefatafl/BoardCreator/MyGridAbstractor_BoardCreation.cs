using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGridAbstractor_BoardCreation
{
    public MyGrid<HFBoardTile> grid { get; private set; }

    public MyGridAbstractor_BoardCreation (int width, int height, float tileSize, Vector3 gridStartPos)
    {
        grid = new MyGrid<HFBoardTile>(width, height, tileSize, gridStartPos, (MyGrid<HFBoardTile> grider, int x, int y) => new HFBoardTile(grider, x, y));
    }

    public void SetTileSprite(Vector3 worldPos, int sptIndex)
    {
        HFBoardTile tileToDraw = grid.GetGridObject(worldPos);
        if (tileToDraw != null)
        {
            tileToDraw.SetSpriteIndex(sptIndex);
        }

    }

    public void SetTile(HFBoardTile tile)
    {
        tile.SetGrid(grid);
        grid.SetGridObject(tile.x, tile.y, tile);
        grid.TriggerGridObjectChanged(tile.x, tile.y);
    }

    public void SetTilemapVisual(TilemapVisual tileVisual)
    {
        tileVisual.SetGrid(grid); 
    }

    public void GetXY(Vector3 worldPos, out int x, out int y)
    {
        grid.GetXY(worldPos, out x, out y);
    }

}

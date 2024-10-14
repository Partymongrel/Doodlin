using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer))]
public class MovePreviewer : MonoBehaviour
{
    private MyGrid<HFBoardTile> grid;
    private Mesh mesh;

    private bool updateMesh;

    Vector3[] vertices;
    Vector2[] uvs;
    int[] triangles;

    private HFBoardTile[] currentlyPreviewingMoves;

    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void OnEnable()
    {
        GameManager.Instance.ResetGameEvent += HideAllMovePreviews;
    }

    private void OnDisable()
    {
        if (grid != null) grid.OnGridValueChangedEvent -= Grid_UpdateBoardVisual;
        GameManager.Instance.ResetGameEvent -= HideAllMovePreviews;
    }

    public void SetGrid(MyGrid<HFBoardTile> grid)
    {
        this.grid = grid;
        //DrawTiles();

        grid.OnGridValueChangedEvent += Grid_UpdateBoardVisual;
        DrawTiles();

    }

    private void Grid_UpdateBoardVisual(Vector2 e)
    {
        //UpdateHeatmapVisual();
        updateMesh = true;
        //print("Event fired on grid " + e);
    }

    private void LateUpdate()
    {
        if (updateMesh) DrawTiles();
    }

    private void DrawTiles()
    {
        UtilsClass.CreateEmptyMeshArrays(grid.GetWidth() * grid.GetHeight(), out vertices, out uvs, out triangles);

        Texture tileTexture = GetComponent<MeshRenderer>().material.mainTexture;
        float textureWidth = tileTexture.width; 

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                int index = x * grid.GetHeight() + y;
                Vector3 quadSize = Vector2.zero;

                HFBoardTile tile = grid.GetGridObject(x, y);
                Vector2 gridValueUV00 = Vector2.zero;
                Vector2 gridValueUV11 = Vector2.zero;

                if (tile.movePreviewIndex != 0)
                {
                    gridValueUV00 = new Vector2(64 * (tile.movePreviewIndex - 1) / textureWidth, 0);
                    gridValueUV11 = new Vector2(64 * (tile.movePreviewIndex) / textureWidth, 1);

                    quadSize = new Vector3(1, 1) * grid.GetCellSize();
                }

                MeshUtils.AddToMeshArrays(vertices, uvs, triangles, index, grid.GetWorldPosition(x, y) + quadSize * .5f, 0f, quadSize, gridValueUV00, gridValueUV11);

            }
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        updateMesh = false;

    }

    public void HideAllMovePreviews()
    {

        if (currentlyPreviewingMoves == null) return;

        foreach (HFBoardTile tile in currentlyPreviewingMoves)
        {
            tile.RemoveTileFromMoveDisplay();
        }

        currentlyPreviewingMoves = null;

    }

    public void PreviewMovesForPiece(PieceBase selectedPiece, out HFBoardTile[] possibleMoves)
    {
        if (currentlyPreviewingMoves != null) HideAllMovePreviews();

        Vector2 selectedPieceCoords = new Vector2(selectedPiece.tileThisPieceIsOn.x, selectedPiece.tileThisPieceIsOn.y);

        List<HFBoardTile> tilesCurrentlyDisplaying = new List<HFBoardTile>();
        List<HFBoardTile> impassableTiles = new List<HFBoardTile>();

        for (int i = 0; i < grid.width; i++)
        {
            tilesCurrentlyDisplaying.Add(grid.GetGridObject((int)selectedPieceCoords.x, i));
            tilesCurrentlyDisplaying.Add(grid.GetGridObject(i, (int)selectedPieceCoords.y));
        }
        
        foreach (HFBoardTile tile in tilesCurrentlyDisplaying)
        {
            tile.CalculateMovePreviewIndex(selectedPiece);

            if(tile.movePreviewIndex == 0) impassableTiles.Add(tile);

        }

        ClearOutAvailableMoves();

        List<HFBoardTile> newlyImpassableTiles = new List<HFBoardTile>();

        for (int i = 0; i < tilesCurrentlyDisplaying.Count; i++)
        {
            HFBoardTile tile = tilesCurrentlyDisplaying[i];
            if (tile == PieceBase.selectedPiece.tileThisPieceIsOn) continue;

            foreach (HFBoardTile impassableTile in impassableTiles)
            {

                if (newlyImpassableTiles.Contains(tile)) continue;

                TileDirection directionOfTile = CalculateTileDirection(tile, selectedPiece.tileThisPieceIsOn);
                TileDirection directionOfImpassableTile = CalculateTileDirection(impassableTile, selectedPiece.tileThisPieceIsOn);
                if (directionOfTile == directionOfImpassableTile && directionOfTile != TileDirection.ERROR && directionOfImpassableTile != TileDirection.ERROR)
                {
                    bool isBehind = false;

                    switch (directionOfTile)
                    {

                        case TileDirection.UP:
                            if (tile.y > impassableTile.y)
                            {
                                isBehind = true;
                            }
                            break;

                        case TileDirection.DOWN:
                            if (tile.y < impassableTile.y)
                            {
                                isBehind = true;
                            }
                            break;

                        case TileDirection.LEFT:
                            if (tile.x < impassableTile.x)
                            {
                                isBehind = true;
                            }
                            break;

                        case TileDirection.RIGHT:
                            if (tile.x > impassableTile.x)
                            {
                                isBehind = true;
                            }
                            break;

                    }

                    if (isBehind)
                    {
                        //print(tile.ToString() + " is " + directionOfTile + " of "+ impassableTile.ToString());
                        newlyImpassableTiles.Add(tile);
                    }
                }
            }
        }

        foreach (HFBoardTile tileToRemove in newlyImpassableTiles)
        {
            tileToRemove.SetPreviewIndex(0);
        }

        ClearOutAvailableMoves();

        currentlyPreviewingMoves = tilesCurrentlyDisplaying.ToArray();

        possibleMoves = CreateMoveList();

        #region internal methods
        HFBoardTile[] CreateMoveList()
        {
            List<HFBoardTile> tilesToReturn = new List<HFBoardTile>(); 

            foreach (HFBoardTile tile in currentlyPreviewingMoves)
            {
                if (tile.PieceCanOccupy(PieceBase.selectedPiece.pieceType)) tilesToReturn.Add(tile);
            }

            return tilesToReturn.ToArray();

        }

        void ClearOutAvailableMoves()
        {

            List<HFBoardTile> tilesToRemove = new List<HFBoardTile>();

            foreach (HFBoardTile tile in tilesCurrentlyDisplaying)
            {
                if (tile.movePreviewIndex == 0)
                {
                    tilesToRemove.Add(tile);
                }
            }

            foreach (HFBoardTile tile in tilesToRemove)
            {
                tilesCurrentlyDisplaying.Remove(tile);
            }

        }

        TileDirection CalculateTileDirection(HFBoardTile tileToCheck, HFBoardTile tileToCheckAgainst)
        {

            TileDirection direction = TileDirection.ERROR;

            if(tileToCheck == PieceBase.selectedPiece.tileThisPieceIsOn || tileToCheck == tileToCheckAgainst) { return direction; }

            if (tileToCheck.x == tileToCheckAgainst.x)
            {
                if (tileToCheck.y > tileToCheckAgainst.y) direction = TileDirection.UP;
                else direction = TileDirection.DOWN;
            }
            else if (tileToCheck.y == tileToCheckAgainst.y) 
            {
                if (tileToCheck.x < tileToCheckAgainst.x) direction = TileDirection.LEFT;
                else direction = TileDirection.RIGHT;
            }

            //print(tileToCheck.ToString() + " is " + direction + " from " +  tileToCheckAgainst.ToString());

            return direction;

        }
        #endregion
    }
}

//public struct TileDirectionStruct
//{

//    public TileDirectionStruct(HFBoardTile tile, HFBoardTile tileToCompare)
//    {
//        this.tile = tile;

//        if (tile.x == tileToCompare.x)
//        {
//            if (tile.y > tileToCompare.y) direction = TileDirection.UP;
//            else direction = TileDirection.DOWN;
//        }
//        else
//        {
//            if (tile.x < tileToCompare.x) direction = TileDirection.LEFT;
//            else direction = TileDirection.RIGHT;
//        }

//    }

//    public HFBoardTile tile;
//    public TileDirection direction;
//}

public enum TileDirection
{
    UP = 0, 
    DOWN = 1, 
    LEFT = 2, 
    RIGHT = 3,
    ERROR = 4,
}


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoardGrid : MonoBehaviour
{

    [SerializeField] private InputActionReference mouseLeftClickAction, mouseRightClickAction;
    private MyGrid<HFBoardTile> boardGrid;
    [SerializeField] private float tileSize;
    [SerializeField] private int gridSize = 9;
    //[SerializeField] private Vector2[] attackerPlaces, defenderPlaces;
    [SerializeField] private GameObject[] piecePrefabs;
    [SerializeField] private TilemapVisual tilemapVisual;
    [SerializeField] private MovePreviewer movePreviewer;
    [SerializeField] private HFBoardTile[] defaultBoard;
    private HFBoardTile[] possibleMoves = new HFBoardTile[0];

    private void OnEnable()
    {
        mouseLeftClickAction.action.started += MouseLeftClick;
        mouseRightClickAction.action.started += MouseRightClick;
    }

    void Start()
    {
        GameManager.Instance.ResetGameEvent += ResetBoard;

        if (GameManager.Instance.boardToPlayOn != null)
        {
            boardGrid = GameManager.Instance.boardToPlayOn;

        }
        else
        {

            float gridOffset = -((tileSize * gridSize) / 2);
            Vector3 gridStartPos = new Vector3(gridOffset, gridOffset, 0);
            boardGrid = new MyGrid<HFBoardTile>(gridSize, gridSize, tileSize, gridStartPos, (MyGrid<HFBoardTile> grider, int x, int y) => new HFBoardTile(grider, x, y));

            //set default board
            foreach (HFBoardTile tile in defaultBoard)
            {
                tile.SetGrid(boardGrid);
                boardGrid.SetGridObject(tile.x, tile.y, tile);
                boardGrid.TriggerGridObjectChanged(tile.x, tile.y);
            }
        }

        tilemapVisual.SetGrid(boardGrid);
        movePreviewer.SetGrid(boardGrid);
        ResetBoard();

    }

    private void OnDisable()
    {
        mouseLeftClickAction.action.started -= MouseLeftClick;
        mouseRightClickAction.action.started -= MouseRightClick;
        GameManager.Instance.ResetGameEvent -= ResetBoard;
    }

    private void MouseLeftClick(InputAction.CallbackContext context)
    {
        Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        boardGrid.GetXY(position, out int x, out int y);

        //HFBoardTile bt = boardGrid.GetGridObject(x, y);
        //bt.shouldAddTileToMoveDisplay = true;
        //boardGrid.TriggerGridObjectChanged(x, y);
        //return;

        //Vector2 selectedPieceCoords;
        HFBoardTile selectedTile = boardGrid.GetGridObject(x, y);
        //bool shouldMove = false;

        PieceBase pieceOnTile = selectedTile.GetPieceOnTile();

        if (pieceOnTile != null && PieceBase.selectedPiece != pieceOnTile && pieceOnTile.team == GameManager.Instance.currentTeamTurn)
        {
            pieceOnTile.SelectPiece();
            movePreviewer.PreviewMovesForPiece(PieceBase.selectedPiece, out possibleMoves);
        }
        else //if there is no piece on that square
        {

            if (PieceBase.selectedPiece == null || possibleMoves.Length == 0) return;

            if (possibleMoves.Contains(selectedTile))
            {
                MovePieceTo(selectedTile);
                movePreviewer.HideAllMovePreviews();
                possibleMoves = new HFBoardTile[0];
            }
            else 
            {
                PieceBase.selectedPiece.DeselectPiece();
                movePreviewer.HideAllMovePreviews();
                possibleMoves = new HFBoardTile[0];
            }

            //if (selectedTile == PieceBase.selectedPiece.tileThisPieceIsOn || !selectedTile.PieceCanOccupy(PieceBase.selectedPiece.pieceType))
            //{
            //    PieceBase.selectedPiece.DeselectPiece();
            //    movePreviewer.HideAllMovePreviews();
            //    return;
            //}

            //selectedPieceCoords = new Vector2(PieceBase.selectedPiece.tileThisPieceIsOn.x, PieceBase.selectedPiece.tileThisPieceIsOn.y);

            //if (selectedPieceCoords.x == x) //if we can move horizontally
            //{

            //    if(selectedPieceCoords.y > y) //if we are moving to a square above us
            //    {

            //        for (int i = (int)selectedPieceCoords.y - 1; i >= y; i--)
            //        {
            //            HFBoardTile t = boardGrid.GetGridObject(x, i);
            //            if (!t.PieceCanMoveThrough(PieceBase.selectedPiece.pieceType))
            //            {
            //                //print("movement blocked!");
            //                PieceBase.selectedPiece.DeselectPiece();
            //                movePreviewer.HideAllMovePreviews();
            //                return;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        for (int i = (int)selectedPieceCoords.y +1 ; i <= y; i++) //if we are moving to a square below us
            //        {
            //            HFBoardTile t = boardGrid.GetGridObject(x, i);
            //            if (!t.PieceCanMoveThrough(PieceBase.selectedPiece.pieceType))
            //            {
            //                //print("movement blocked!");
            //                PieceBase.selectedPiece.DeselectPiece();
            //                movePreviewer.HideAllMovePreviews();
            //                return;
            //            }
            //        }
            //    }

            //    shouldMove = true;

            //} else if (selectedPieceCoords.y == y) //if we are moving vertically
            //{

            //    if (selectedPieceCoords.x > x) //if we are moving right
            //    {

            //        for (int i = (int)selectedPieceCoords.x - 1; i >= x; i--)
            //        {
            //            HFBoardTile t = boardGrid.GetGridObject(i, y);
            //            if (!t.PieceCanMoveThrough(PieceBase.selectedPiece.pieceType))
            //            {
            //                //print("movement blocked!");
            //                PieceBase.selectedPiece.DeselectPiece();
            //                movePreviewer.HideAllMovePreviews();
            //                return;
            //            }
            //        }

            //    }
            //    else
            //    {
            //        for (int i = (int)selectedPieceCoords.x + 1; i <= x; i++) //if we are moving left
            //        {
            //            HFBoardTile t = boardGrid.GetGridObject(i, y);
            //            if (!t.PieceCanMoveThrough(PieceBase.selectedPiece.pieceType))
            //            {
            //                //print("movement blocked!");
            //                PieceBase.selectedPiece.DeselectPiece();
            //                movePreviewer.HideAllMovePreviews();
            //                return;
            //            }
            //        }
            //    }

            //    shouldMove = true;
            //}

        }

        //if (shouldMove)
        //{

        //}
        //else if (PieceBase.selectedPiece != null)
        //{
        //    if (boardGrid.GetGridObject(x, y) != PieceBase.selectedPiece.tileThisPieceIsOn)
        //    {
        //    }
        //}

    }

    private void MovePieceTo(HFBoardTile boardTileToMovePieceTo)
    {
        PieceBase.selectedPiece.RevertColor();
        PieceBase.selectedPiece.tileThisPieceIsOn.DestroyPieceOnTile();

        boardTileToMovePieceTo.PlacePieceOnTile(PieceBase.selectedPiece.gameObject);

        ResolveCaptures(boardTileToMovePieceTo);
        boardTileToMovePieceTo.GetPieceOnTile().ResolveSpecialEffects(out bool shouldWin);
        PieceBase.selectedPiece.DeselectPiece();

        if (!shouldWin && !GameManager.Instance.winningSet)
        {
            GameManager.Instance.NextTurn();
        }

    }

    private void ResolveCaptures(HFBoardTile tileToCheckNeighbors)
    {
        //check if any enemies are next to me
        Vector2[] coordsToCheck = tileToCheckNeighbors.GetSurroundingTileCoords();
        PieceBase pieceJustMoved = tileToCheckNeighbors.GetPieceOnTile();

        //print("The piece on this square is a " + tileToCheckNeighbors.GetPieceOnTile().team);

        foreach (Vector2 v in coordsToCheck)
        {
            
            HFBoardTile neighboringTile = boardGrid.GetGridObject((int)v.x, (int)v.y);
            PieceBase neighborPiece = neighboringTile.GetPieceOnTile();

            //string printString = neighborPiece == null ? "No adjacent piece on tile " : neighborPiece.team.ToString() + " found on tile ";
            //print(printString + v);

            if (neighborPiece != null)
            {
                if (neighborPiece.team != pieceJustMoved.team)
                {
                    //print("moved next to an enemy!");

                    if (tileToCheckNeighbors.x == neighboringTile.x)
                    {

                        //print("We have the same x coords!");

                        int yCoordToCheck = neighboringTile.y + (neighboringTile.y - tileToCheckNeighbors.y);
                        if (yCoordToCheck >= gridSize) continue;

                        HFBoardTile tileToCheck = boardGrid.GetGridObject(tileToCheckNeighbors.x, yCoordToCheck);

                        //print("Checking tile on (" + tileToCheckNeighbors.x + ", " + yCoordToCheck + ")");

                        if (tileToCheck.CountsAsAlly(pieceJustMoved.team))
                        {
                            //print("The piece on " + v + " should be captured!");
                            boardGrid.GetGridObject(neighboringTile.x, neighboringTile.y).CapturePieceOnTile();
                        }

                    }

                    else if (tileToCheckNeighbors.y == neighboringTile.y)
                    {

                        //print("We have the same y coords!");

                        int xCoordToCheck = neighboringTile.x + (neighboringTile.x - tileToCheckNeighbors.x);
                        if (xCoordToCheck >= gridSize) continue;

                        HFBoardTile tileToCheck = boardGrid.GetGridObject(xCoordToCheck, tileToCheckNeighbors.y);

                        //print("Checking for friendly piece on (" + xCoordToCheck + ", " + neighboringTile.y + ")");

                        if (tileToCheck.CountsAsAlly(pieceJustMoved.team))
                        {
                            //print("The piece on " + v + " should be captured!");
                            boardGrid.GetGridObject(neighboringTile.x, neighboringTile.y).CapturePieceOnTile();
                        }

                    }

                }
            }
        }

        PieceBase.DeselectAll();
    }

    private void MouseRightClick(InputAction.CallbackContext context)
    {
        PieceBase.selectedPiece.DeselectPiece();
    }

    private void ResetBoard()
    {
        PieceBase.selectedPiece = null;

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                HFBoardTile tile = boardGrid.GetGridObject(x, y);
                tile.DestroyPieceOnTile();

                if (tile.pieceToSpawn != PieceType.None)
                {
                    tile.PlacePieceOnTile(piecePrefabs[(int)tile.pieceToSpawn - 1]);
                }

            }
        }

        //void PlaceAttackers()
        //{
        //    foreach(Vector2 v in attackerPlaces)
        //    {
        //        boardGrid.GetGridObject((int)v.x, (int)v.y).PlacePieceOnTile(attackerPiece);
        //    }
        //}

        //void PlaceDefenders()
        //{
        //    foreach (Vector2 v in defenderPlaces)
        //    {
        //        boardGrid.GetGridObject((int)v.x, (int)v.y).PlacePieceOnTile(defenderPiece);
        //    }

        //    boardGrid.GetGridObject(4, 4).PlacePieceOnTile(kingPiece);

        //}

    }

}

[Serializable]
public class HFBoardTile
{
    private MyGrid<HFBoardTile> grid;
    public int x, y;
    private PieceBase pieceOnTile;
    private GameObject objectOnTile;
    [SerializeField] private int spriteIndex;

    public PieceType pieceToSpawn = PieceType.None;
    public bool isWinningSquare = false;
    public int movePreviewIndex = 0;

    [SerializeField] private bool defenderCanMoveThrough = true, attackerCanMoveThrough = true, kingCanMoveThrough = true;
    [SerializeField] private bool defenderCanOccupy = true, attackerCanOccupy = true, kingCanOccupy = true;
    [SerializeField] private bool countsAsAttacker = false, countsAsDefender = false;

    public HFBoardTile(MyGrid<HFBoardTile> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;

        System.Random rand = new System.Random();
        int calculatedSpriteIndex = rand.Next(0, 7);

        this.spriteIndex = calculatedSpriteIndex;

    }

    public bool ShouldPreviewSpecialColour(PieceBase piece)
    {
        if (piece.pieceType == PieceType.King && isWinningSquare) return true;
        else return false;
    }

    public void CalculateMovePreviewIndex(PieceBase piece)
    {
        if (piece == null) return;

        if (PieceCanMoveThrough(piece.pieceType)) movePreviewIndex = 1;
        if (!PieceCanOccupy(piece.pieceType)) movePreviewIndex = 2;
        if (piece.pieceType == PieceType.King && isWinningSquare) movePreviewIndex = 3;
        if (pieceOnTile != null) movePreviewIndex = 0;
        
        grid.TriggerGridObjectChanged(x, y);

    }

    public void SetPreviewIndex(int i)
    {
        movePreviewIndex = i;
    }

    public void SetGrid(MyGrid<HFBoardTile> g)
    {
        grid = g;
    }

    public void SetPieceToSpawn(PieceType pieceType)
    {
        pieceToSpawn = pieceType;
    }

    public bool CheckIfWinningSquare()
    {
        return isWinningSquare;
    }

    public void SetShouldBeWinningSquare(bool b)
    {
        isWinningSquare = b;
    }

    public void SetSpriteIndex(int index)
    {
        this.spriteIndex = index;
    }

    public override string ToString()
    {
        return x + ", " + y;
    }

    public void PlacePieceOnTile(GameObject pieceToPlace) 
    {
        if (pieceToPlace != null)
        {
            objectOnTile = GameObject.Instantiate(pieceToPlace, GetTileCenter(), Quaternion.identity);
            pieceOnTile = objectOnTile.GetComponent<PieceBase>();
            pieceOnTile.InitializePiece(this);
            pieceOnTile.enabled = true;
        }
    }

    public void DestroyPieceOnTile()
    {
        if (pieceOnTile == null) return;

        GameObject.Destroy(objectOnTile.gameObject);
        pieceOnTile = null;
        objectOnTile = null;
    }

    public Vector3 GetTileCenter()
    {

        float mod = grid.GetCellSize() / 2;
        float centerX = grid.GetWorldPosition(x, y).x + mod;
        float centerY = grid.GetWorldPosition(x, y).y + mod;

        return new Vector3(centerX, centerY, grid.GetWorldPosition(x, y).z);
    }

    public PieceBase GetPieceOnTile()
    {
        //Debug.Log("Getting piece on tile " + x + ", " + y);
        return pieceOnTile;
    }

    public Vector2[] GetSurroundingTileCoords()
    {
        List<Vector2> results = new List<Vector2>();

        if (x + 1 < grid.height) results.Add(new Vector2(x + 1, y));
        if (x - 1 > 0) results.Add(new Vector2(x - 1, y));
        if (y + 1 < grid.width) results.Add(new Vector2(x, y + 1));
        if (y - 1 > 0) results.Add(new Vector2(x, y - 1));

        return results.ToArray();

    }

    public HFBoardTile[] GetSurroundingTiles()
    {
        Vector2[] coordsToCheck = GetSurroundingTileCoords();
        List<HFBoardTile> result = new List<HFBoardTile>();

        foreach (Vector2 coords in coordsToCheck)
        {
            result.Add(grid.GetGridObject((int)coords.x, (int)coords.y));
        }

        return result.ToArray();

    }

    public void CapturePieceOnTile()
    {

        if (pieceOnTile == null) return;

        if (pieceOnTile.TryCapturePiece())
        {
            pieceOnTile = null;
            objectOnTile = null;
        }
    }

    public Vector2 GetGridSize()
    {
        return grid.GetWidthAndHeight();
    }

    public int GetSpriteIndex() 
    { 
        return spriteIndex; 
    }

    public bool PieceCanMoveThrough(PieceType pieceType)
    {

        bool returnBool = true;

        if (pieceType == PieceType.Attacker && !attackerCanMoveThrough)
        {
            returnBool = false;
        }
        else if (pieceType == PieceType.Defender && !defenderCanMoveThrough)
        {
            returnBool = false;
        }
        else if (pieceType == PieceType.King && !kingCanMoveThrough)
        {
            returnBool = false;
        }
        else if (pieceOnTile != null)
        {
            returnBool = false;
        }

         return returnBool;

    }

    public bool PieceCanOccupy(PieceType pieceType)
    {

        bool returnBool = true;

        if (pieceType == PieceType.Attacker && !attackerCanOccupy)
        {
            returnBool = false;
        }
        else if (pieceType == PieceType.Defender && !defenderCanOccupy)
        {
            returnBool = false;
        }
        else if (pieceType == PieceType.King && !kingCanOccupy)
        {
            returnBool = false;
        }
        else if (pieceOnTile != null)
        {
            returnBool = false;
        }

        return returnBool;

    }

    public void SetMoveThroughPermissions(bool attackerCanMoveThrough, bool defenderCanMoveThrough, bool kingCanMoveThrough)
    {
        this.attackerCanMoveThrough = attackerCanMoveThrough;
        this.defenderCanMoveThrough = defenderCanMoveThrough;
        this.kingCanMoveThrough = kingCanMoveThrough;
    }

    public void SetCanOccupyPermissions(bool attackerCanOccupy, bool defenderCanOccupy, bool kingCanOccupy)
    {
        this.attackerCanOccupy = attackerCanOccupy;
        this.defenderCanOccupy = defenderCanOccupy;
        this.kingCanOccupy = kingCanOccupy;
    }

    public void SetCountsAsPermissions(bool attacker, bool defender)
    {
        countsAsAttacker = attacker;
        countsAsDefender = defender;
    }

    public bool CountsAsAlly(PieceTeam team)
    {
        bool returnBool = false;

        if (pieceOnTile != null)
        {
            if (pieceOnTile.team == team)
            {
                returnBool = true;
            }
        }

        if (team == PieceTeam.Attacker && countsAsAttacker)
        {
            returnBool = true;

        } else if (team == PieceTeam.Defender && countsAsDefender)
        {
            returnBool = true;
        }

        return returnBool;

    }

    public void RemovePieceOnTile()
    {

        pieceOnTile = null;
        objectOnTile = null;
    }

    public void RemoveTileFromMoveDisplay()
    {
        movePreviewIndex = 0;
        grid.TriggerGridObjectChanged(x, y);
    }
}

public enum PieceType
{
    None = 0,
    Attacker = 1,
    Defender = 2,
    King = 3
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileCreator : MonoBehaviour
{

    public static TileCreator Instance { get; private set; }

    [SerializeField] Sprite[] spriteDisplayOptions;
    [SerializeField] Image spriteToDisplay;

    private int spriteIndex;
    private PieceType pieceToSpawn;
    private bool winningSquare = false;
    private bool attackerCanMoveThrough = true, defenderCanMoveThrough = true, kingCanMoveThrough = true;
    private bool defenderCanOccupy = true, attackerCanOccupy = true, kingCanOccupy = true;
    private bool countsAsAttacker = false, countsAsDefender = false;

    private void Awake()
    {
        Instance = this;
    }

    public void ChangeSpriteIndex(int amountToChangeBy)
    {
        spriteIndex += amountToChangeBy;
        spriteIndex = Mathf.Clamp(spriteIndex, 0, spriteDisplayOptions.Length - 1);
        spriteToDisplay.sprite = spriteDisplayOptions[spriteIndex];
    }

    public void SetSpriteIndex(int index)
    {
        spriteIndex = index;
        spriteIndex = Mathf.Clamp(spriteIndex, 0, spriteDisplayOptions.Length - 1);
        spriteToDisplay.sprite = spriteDisplayOptions[spriteIndex];
    }

    public void SetPieceToSpawn(int index)
    {
        pieceToSpawn = (PieceType)index;
    }

    public void SetWinningSquare(bool b)
    {
        winningSquare = b;
    }
    
    public void SetAttackerMoveThrough(bool b)
    {
        attackerCanMoveThrough = b;
    }

    public void SetDefenderMoveThrough(bool b)
    {
        defenderCanMoveThrough = b;
    }

    public void SetKingMoveThrough(bool b)
    {
        kingCanMoveThrough = b;
    }

    public void SetAttackerCanOccupy(bool b)
    {
        attackerCanOccupy = b;
    }

    public void SetDefenderCanOccupy(bool b)
    {
        defenderCanOccupy = b;
    }

    public void SetKingCanOccupy(bool b)
    {
        kingCanOccupy = b;
    }

    public void SetCountsAsAttacker(bool b)
    {
        countsAsAttacker = b;
    }

    public void SetCountsAsDefender(bool b)
    {
        countsAsDefender = b;
    }

    public HFBoardTile GetNewTile(int x, int y)
    {
        HFBoardTile tileToReturn = new HFBoardTile(null, x, y);
        tileToReturn.SetSpriteIndex(spriteIndex);
        tileToReturn.SetPieceToSpawn(pieceToSpawn);
        tileToReturn.SetShouldBeWinningSquare(winningSquare);
        tileToReturn.SetMoveThroughPermissions(attackerCanMoveThrough, defenderCanMoveThrough, kingCanMoveThrough);
        tileToReturn.SetCanOccupyPermissions(attackerCanOccupy, defenderCanOccupy, kingCanOccupy);
        tileToReturn.SetCountsAsPermissions(countsAsAttacker, countsAsDefender);

        return tileToReturn;
    }

}

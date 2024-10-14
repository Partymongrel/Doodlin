using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackerPiece : PieceBase
{
    public override void InitializePiece(HFBoardTile tileThisPieceIsPlacedOn)
    {
        base.InitializePiece(tileThisPieceIsPlacedOn);
        team = PieceTeam.Attacker;
        pieceType = PieceType.Attacker;

        gameObject.name = $"Attacker ({tileThisPieceIsOn.x}, {tileThisPieceIsOn.y})";
    }
}

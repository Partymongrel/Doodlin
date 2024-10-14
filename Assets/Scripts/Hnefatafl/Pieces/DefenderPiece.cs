using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenderPiece : PieceBase
{

    public override void InitializePiece(HFBoardTile tileThisPieceIsPlacedOn)
    {
        base.InitializePiece(tileThisPieceIsPlacedOn);
        team = PieceTeam.Defender;
        pieceType = PieceType.Defender;

        gameObject.name = $"Defender ({tileThisPieceIsOn.x}, {tileThisPieceIsOn.y})";
    }

}

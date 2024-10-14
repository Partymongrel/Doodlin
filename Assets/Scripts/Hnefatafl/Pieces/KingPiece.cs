using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingPiece : PieceBase
{

    public override void InitializePiece(HFBoardTile tileThisPieceIsPlacedOn)
    {
        base.InitializePiece(tileThisPieceIsPlacedOn);
        team = PieceTeam.Defender;
        pieceType = PieceType.King;

        pieceStrength = GameManager.Instance.Ruleset.strongKing ? PieceStrength.Strong : PieceStrength.Normal;

        gameObject.name = $"King ({tileThisPieceIsOn.x}, {tileThisPieceIsOn.y})";


    }

    public override void ResolveSpecialEffects(out bool w)
    {
        //print("You moved the king!");


        CheckIfOnWinningSquare(out bool gameEnded);

        if (!gameEnded)
        {
            w = false;
            base.ResolveSpecialEffects(out bool b);
        }
        else
        {
            w = true;
        }



    }

    public override void CapturePiece()
    {
        //print("You captured the king! Attackers Win!");
        GameManager.Instance.WinGame(PieceTeam.Attacker);
        base.CapturePiece();
    }

    private void CheckIfOnWinningSquare(out bool gameShouldEnd)
    {

        if (tileThisPieceIsOn.isWinningSquare)
        {
            gameShouldEnd = true;
            GameManager.Instance.WinGame(PieceTeam.Defender);
        }
        else
        {
            gameShouldEnd = false;
        }


    }

}

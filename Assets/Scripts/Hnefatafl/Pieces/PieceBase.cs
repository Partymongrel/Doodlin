using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PieceBase : MonoBehaviour
{

    public static PieceBase selectedPiece;
    public Sprite[] spriteOptions;
    public Color selectedColor;
    protected Color originalColor;
    protected SpriteRenderer sptRend;
    public HFBoardTile tileThisPieceIsOn;
    public PieceTeam team;
    public PieceType pieceType;
    public PieceStrength pieceStrength = PieceStrength.Normal;
    protected bool shouldFollowMouse = false;
    protected Vector3 startingPos;

    private void Update()
    {
        if (shouldFollowMouse) {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePos.x, mousePos.y, -1f); 
        }
    }

    private void OnMouseEnter()
    {
        if (GameManager.Instance.currentTeamTurn != this.team) { return; }

        if (sptRend != null && selectedPiece != this)
        {
            sptRend.color = Color.white;
        }
    }

    private void OnMouseExit()
    {
        if (GameManager.Instance.currentTeamTurn != this.team) { return; }

        if (sptRend != null && selectedPiece != this)
        {
            sptRend.color = originalColor;
        }
    }

    public virtual void InitializePiece(HFBoardTile tileThisPieceIsPlacedOn)
    {
        sptRend = gameObject.GetComponent<SpriteRenderer>();
        originalColor = sptRend.color;
        this.tileThisPieceIsOn = tileThisPieceIsPlacedOn;

        System.Random rnd = new System.Random();
        sptRend.sprite = spriteOptions[rnd.Next(spriteOptions.Length)];

        CircleCollider2D cc = GetComponent<CircleCollider2D>();
        cc.enabled = true;
    }

    public virtual bool TryCapturePiece()
    {

        if (ShouldPieceBeCaptured())
        {
            CapturePiece();
            return true;
        }
        else 
        { 
            return false;
        }

        bool ShouldPieceBeCaptured()
        {

            if (pieceStrength == PieceStrength.Normal) return true;

            HFBoardTile[] surroundingTiles = tileThisPieceIsOn.GetSurroundingTiles();
            if (surroundingTiles.Count() != 4) return false;

            int surroundingEnemyCount = 0;

            foreach (HFBoardTile tile in surroundingTiles)
            {
                PieceBase pieceOnSurroundingSquare = tile.GetPieceOnTile();
                if (pieceOnSurroundingSquare != null)
                {
                    if (pieceOnSurroundingSquare.team != this.team)
                    {
                        surroundingEnemyCount++;
                    }
                }
            }

            return surroundingEnemyCount == 4;

        }

    }

    public virtual void CapturePiece()
    {
        tileThisPieceIsOn.RemovePieceOnTile();
        Destroy(gameObject);
    }

    public virtual void ResolveSpecialEffects(out bool shouldWin)
    {

        shouldWin = false;

    }

    public void SelectPiece()
    {

        if (GameManager.Instance.currentTeamTurn != this.team) { return; }

        if (selectedPiece != this)
        {
            if (selectedPiece != null) selectedPiece.DeselectPiece();

            selectedPiece = this;
            this.sptRend.color = selectedColor;
            //sptRend.color = Color.white;
            startingPos = gameObject.transform.position;
            shouldFollowMouse = true;

        }

        //string pieceName = selectedPiece == null ? "none selected!" : selectedPiece.name;
        //print("Selected piece! Selected piece = " + pieceName);
    }

    public void RevertColor()
    {
        sptRend.color = originalColor;
    }

    public void DeselectPiece()
    {
        selectedPiece = null;
        sptRend.color = originalColor;
        shouldFollowMouse = false;
        transform.position = startingPos;
    }

    public static void DeselectAll()
    {
        if (selectedPiece == null) return;

        selectedPiece.SelectPiece();

    }

}

public enum PieceTeam
{
    Attacker,
    Defender
}

public enum PieceStrength
{
    Normal,
    Strong
}

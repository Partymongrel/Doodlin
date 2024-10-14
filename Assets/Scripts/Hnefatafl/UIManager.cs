using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Canvas))]
public class UIManager : MonoBehaviour
{

    [SerializeField] private float menuInSpeed, menuOutSpeed;
    [SerializeField] private float topPosY, middlePosY;

    [SerializeField] private RectTransform winningPanelRectT;
    [SerializeField] private TextMeshProUGUI winningText, currentTeamTurnText;

    private void Awake()
    {
    }

    private void OnEnable()
    {
        GameManager.Instance.GameWonEvent += DisplayGameWonUI;
        GameManager.Instance.ResetGameEvent += HideGameWonUI;
        GameManager.Instance.NextTurnEvent += ChangeCurrentTeamTurnUI;
    }

    private void OnDisable()
    {
        GameManager.Instance.GameWonEvent -= DisplayGameWonUI;
        GameManager.Instance.ResetGameEvent -= HideGameWonUI;
        GameManager.Instance.NextTurnEvent -= ChangeCurrentTeamTurnUI;
    }

    private void Start()
    {
        HideGameWonUI();
    }

    public void ChangeCurrentTeamTurnUI()
    {
        currentTeamTurnText.text = "Current turn: " + GameManager.Instance.currentTeamTurn;
    }

    public void DisplayGameWonUI(PieceTeam teamThatWon)
    {
        //print("Winner is: " + teamThatWon);
        string changingText = teamThatWon == PieceTeam.Defender ? "escaped!\nDefenders " : "been captured!\nAttackers ";
        string winningDisplayText = "The king has " + changingText + "win!";

        winningText.text = winningDisplayText;
        winningPanelRectT.DOAnchorPosY(middlePosY, menuInSpeed).SetEase(Ease.InOutBack);

    }

    public void HideGameWonUI()
    {
        winningPanelRectT.DOAnchorPosY(topPosY, menuOutSpeed);
        ChangeCurrentTeamTurnUI();
    }
}

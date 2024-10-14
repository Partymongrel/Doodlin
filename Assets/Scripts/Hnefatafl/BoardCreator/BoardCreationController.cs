using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(PlayerInput))]
public class BoardCreationController : MonoBehaviour
{
    [SerializeField] private int gridXByY;
    [SerializeField] private float tileSize;
    private TilemapVisual tmapVisual;

    [SerializeField] private InputActionReference mouseLeftClickAction, mouseRightClickAction;
    private MyGridAbstractor_BoardCreation board;
    public static event Action<MyGrid<HFBoardTile>> BoardCreatedEvent;

    [Space(10)]
    [SerializeField] private HFBoardTile defaultTile;
    [SerializeField] private HFBoardTile[] defaultBoard;

    private void Awake()
    {
        tmapVisual = GetComponentInChildren<TilemapVisual>();
    }

    private void OnEnable()
    {
        mouseLeftClickAction.action.started += MouseLeftClick;
        mouseRightClickAction.action.started += MouseRightClick;
        GameManager.Instance.MoveToPlaySceneEvent += GoToPlayScene;
    }

    private void OnDisable()
    {
        mouseLeftClickAction.action.started -= MouseLeftClick;
        mouseRightClickAction.action.started -= MouseRightClick;
        GameManager.Instance.MoveToPlaySceneEvent -= GoToPlayScene;

    }

    private void Start()
    {
        float gridOffset = -((tileSize * gridXByY) / 2);
        Vector3 gridStartPos = new Vector3(gridOffset, gridOffset, 0);
        board = new MyGridAbstractor_BoardCreation(9, 9, 6, gridStartPos);

        //set default board
        foreach (HFBoardTile tile in defaultBoard)
        {
            board.SetTile(tile);
        }

        board.SetTilemapVisual(tmapVisual);
        
    }

    private void MouseRightClick(InputAction.CallbackContext context)
    {
        if (BoardCreationUIManager.Instance.additionalRulesPanel.activeSelf) return;

        Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        board.GetXY(position, out int x, out int y);
        defaultTile.x = x;
        defaultTile.y = y;
        board.SetTile(defaultTile);
    }

    private void MouseLeftClick(InputAction.CallbackContext context)
    {

        if (BoardCreationUIManager.Instance.additionalRulesPanel.activeSelf)
        {
            //print("Panel is active");
            return;
        }

        Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        board.GetXY(position, out int x, out int y);
        board.SetTile(TileCreator.Instance.GetNewTile(x, y));
    }

    private void GoToPlayScene()
    {
        GameManager.Instance.boardToPlayOn = board.grid;
    }

}

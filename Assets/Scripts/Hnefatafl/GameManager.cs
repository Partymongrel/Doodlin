using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    //loads things
    //controls actions when the game is won/lost
    //controls the players experience: loading and setting menus/scenes
    //controls what turn it is

    public static GameManager Instance;
    public event Action<PieceTeam> GameWonEvent;
    public event Action NextTurnEvent;
    public event Action ResetGameEvent;
    public event Action MoveToPlaySceneEvent;
    [HideInInspector] public PieceTeam currentTeamTurn;

    public MyGrid<HFBoardTile> boardToPlayOn;
    public Ruleset Ruleset = new Ruleset();
    public bool winningSet = false;



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        currentTeamTurn = PieceTeam.Attacker;
    }

    public void WinGame(PieceTeam teamThatWon)
    {
        if (GameWonEvent != null)
        {
            currentTeamTurn = PieceTeam.Attacker;
            GameWonEvent(teamThatWon);
        }

        winningSet = true;

    }

    public void ResetGame()
    {
        if (ResetGameEvent != null)
        {
            ResetGameEvent();
        }

        winningSet = false;

    }

    public void NextTurn()
    {
        if (NextTurnEvent != null)
        {
            currentTeamTurn = currentTeamTurn == PieceTeam.Attacker ? PieceTeam.Defender : PieceTeam.Attacker;
            NextTurnEvent();
        }
    }

    public void Play()
    {
        if (MoveToPlaySceneEvent != null) { 
            
            MoveToPlaySceneEvent();
            SceneManager.LoadScene("Hnefatafl_PlayScene");
            //if (boardToPlayOn != null)
            //{
            //    boardToPlayOn.UpdateDebugVisuals();
            //}

        }
    }

}

public class Ruleset
{

    public bool strongKing = true;

}

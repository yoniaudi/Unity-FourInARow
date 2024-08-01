using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private HumanPlayerModel m_PlayerHuman = null;
    [SerializeField] private ComputerPlayerAIModel m_PlayerComputerAIPrefab = null;
    [SerializeField] private ScoreHandlerModel m_ScoreHandler = null;
    [SerializeField] private BoardGameModel m_BoardGamePrefab = null;
    [SerializeField] private TextMeshProUGUI m_ScoreText = null;
    public static GameManager s_Instance = null;
    public bool IsGameRunning { get; private set; } = false;
    public int BoardMinLength { get; } = 4;
    public int BoardMaxLength { get; } = 8;
    public eTurnState m_TurnState;
    private eTurnState m_ChosenRival;
    private ComputerPlayerAIModel m_PlayerComputerAI = null;
    private BoardGameModel m_BoardGame = null;
    private List<HumanPlayerModel> m_Players = null;
    private bool m_IsTie = true;
    private bool m_HasWinner = true;
    public event Action ShowPlayAgainCanvas = null;

    private void Awake()
    {
        if (s_Instance == null)
        {
            s_Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InitializeBoardGame(int i_RowLength, int i_ColumnLength)
    {
        if (i_RowLength < BoardMinLength || i_RowLength > BoardMaxLength ||
            i_ColumnLength < BoardMinLength || i_ColumnLength > BoardMaxLength)
        {
            string exceptionMessage = string.Format("Row length or column length must be within the specified range {0}-{1}.",
                BoardMinLength, BoardMaxLength);

            throw new ArgumentOutOfRangeException("i_RowLength or i_ColumnLength", exceptionMessage);
        }

        m_BoardGame = Instantiate(m_BoardGamePrefab);
        m_BoardGame.Initialize(i_RowLength, i_ColumnLength);
    }

    public void SetGameMode(string i_GameMode)
    {
        System.Random random = new System.Random();
        string singlePlayer = "SinglePlayer";
        string multiPlayer = "Multiplayer";

        m_Players = new List<HumanPlayerModel>() { Instantiate(m_PlayerHuman) };
        m_Players[0].name = "Player 1";

        if (i_GameMode == singlePlayer)
        {
            m_PlayerComputerAI = Instantiate(m_PlayerComputerAIPrefab);
            m_PlayerComputerAI.name = "Computer AI";
            m_PlayerComputerAI.Board = m_BoardGame.GetBoardGame();
            m_ScoreHandler.InitializeScoreHandler(m_Players[0].name, m_PlayerComputerAI.name);
            m_ChosenRival = eTurnState.computerAI;
        }
        else if (i_GameMode == multiPlayer)
        {
            //IsMultiplayer = true;
            //m_CurrentPlayerIndx = random.Next(2);
            //m_Player2 = Instantiate(m_Player2);
            m_Players.Add(Instantiate(m_PlayerHuman));
            m_Players[1].name = "Player 2";
            m_ScoreHandler.InitializeScoreHandler(m_Players[0].name, m_Players[1].name);
            m_ChosenRival = eTurnState.player2;
        }

        IsGameRunning = true;
        m_BoardGame.Show();
    }

    public void PlayTurn(int i_UserNextMoveIndx)
    {
        int playerRowInsertPosition = 0;
        bool isNextMoveInserted = false;

        UnityEngine.Color gamePieceColor = m_TurnState == eTurnState.player1 ? UnityEngine.Color.green : UnityEngine.Color.red;
        isNextMoveInserted = m_BoardGame.InsertUserNextMoveToBoard(gamePieceColor, i_UserNextMoveIndx, ref playerRowInsertPosition);

        if (isNextMoveInserted)
        {
            if (m_TurnState == eTurnState.player1)
            {
                m_Players[0].RowMoveInput = playerRowInsertPosition;
                m_Players[0].ColumnMoveInput = i_UserNextMoveIndx;
            }
            else if (m_TurnState == eTurnState.player2)
            {
                m_Players[1].RowMoveInput = playerRowInsertPosition;
                m_Players[1].ColumnMoveInput = i_UserNextMoveIndx;
            }

            m_HasWinner = CheckForWinner();
            m_IsTie = CheckForTie();

            if (m_HasWinner == true)
            {
                string winnerMessage = GetWinnerMessage();

                Debug.Log(winnerMessage);
                m_ScoreText.text = winnerMessage;
                setAnotherRoundOrEndGame();
            }
            else if (m_IsTie == true)
            {
                string tieMessage = GetTieMessage();

                Debug.Log(tieMessage);
                m_ScoreText.text = tieMessage;
                setAnotherRoundOrEndGame();
            }
            else
            {
                UpdateNextPlayerIndex();

                if (m_TurnState == eTurnState.computerAI)
                {
                    playComputerAINextMove();
                }
            }
        }
    }

    private void setAnotherRoundOrEndGame()
    {
        m_BoardGame.SetColumnCollidersActive(false);
        ShowPlayAgainCanvas?.Invoke();
    }

    public bool CheckForTie()
    {
        return m_BoardGame.IsTie();
    }

    public bool CheckForWinner()
    {
        UnityEngine.Color gamePieceColor = UnityEngine.Color.red;
        int lastRowMoveInput = 0;
        int lastColumnMoveInput = 0;
        string winnerName = null;
        bool isWinner = false;

        if (m_TurnState == eTurnState.player1)
        {
            winnerName = m_Players[0].name;
            lastRowMoveInput = m_Players[0].RowMoveInput;
            lastColumnMoveInput = m_Players[0].ColumnMoveInput;
            gamePieceColor = UnityEngine.Color.green;
        }
        else if (m_TurnState == eTurnState.player2)
        {
            winnerName = m_Players[1].name;
            lastRowMoveInput = m_Players[1].RowMoveInput;
            lastColumnMoveInput = m_Players[1].ColumnMoveInput;
        }
        else
        {
            winnerName = ComputerPlayerAIModel.k_Name;
            lastRowMoveInput = m_PlayerComputerAI.RowMoveInput;
            lastColumnMoveInput = m_PlayerComputerAI.ColumnMoveInput;
        }

        isWinner = m_BoardGame.CheckIfWinner(lastRowMoveInput, lastColumnMoveInput, gamePieceColor);

        if (isWinner == true)
        {
            m_ScoreHandler.UpdateWinner(winnerName);
        }

        return isWinner;
    }

    public void UpdateNextPlayerIndex()
    {
        m_TurnState = (int)m_ChosenRival - m_TurnState;
    }

    public string GetWinnerMessage()
    {
        return m_ScoreHandler.WinnerMessage;
    }

    public string GetTieMessage()
    {
        return m_ScoreHandler.TieMessage;
    }

    public void btnPlayAgain_Clicked()
    {
        int boardRowLength = m_BoardGame.GetRowLength();
        int boardColumnLength = m_BoardGame.GetColumnLength();

        m_BoardGame.ResetBoard();

        if (m_ChosenRival == eTurnState.player2)
        {
            UpdateNextPlayerIndex();
        }
        else
        {
            Destroy(m_PlayerComputerAI.gameObject);
            m_PlayerComputerAI = Instantiate(m_PlayerComputerAIPrefab);
            m_PlayerComputerAI.name = "Computer AI";
            m_PlayerComputerAI.Board = m_BoardGame.GetBoardGame();
            m_TurnState = eTurnState.player1;
        }
    }

    public void Reset()
    {
        foreach (HumanPlayerModel player in m_Players)
        {
            Destroy(player.gameObject);
        }

        if (m_PlayerComputerAI != null)
        {
            Destroy(m_PlayerComputerAI.gameObject);
        }

        Destroy(m_BoardGame.gameObject);
        m_BoardGame.Reset();
        m_ScoreHandler.Reset();
        m_TurnState = eTurnState.player1;
    }

    public void Surrender()
    {
        string winnerName = null;

        if (m_TurnState != 0)
        {
            winnerName = m_Players[0].name;
        }
        else if (m_TurnState == eTurnState.player2)
        {
            winnerName = m_Players[1].name;
        }
        else
        {
            winnerName = ComputerPlayerAIModel.k_Name;
        }

        m_ScoreHandler.UpdateWinner(winnerName);
    }

    public bool IsColumnFull(int i_UserNextMoveIndx)
    {
        return m_BoardGame.IsColumnFull(i_UserNextMoveIndx);
    }

    private void playComputerAINextMove()
    {
        int bestAIMove = m_PlayerComputerAI.GetBestMove();

        PlayTurn(bestAIMove);
    }

    public void btnQuit_clicked()
    {
        Reset();
    }
}

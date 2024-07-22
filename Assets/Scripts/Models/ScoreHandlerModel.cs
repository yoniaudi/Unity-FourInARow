using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreHandlerModel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_PlayerTurnText = null;
    [SerializeField] private TextMeshProUGUI m_ScoreText = null;
    [SerializeField] private TextMeshProUGUI m_ScoreText2 = null;
    public string WinnerMessage { get; private set; } = null;
    public string TieMessage { get; private set; } = null;
    private int m_PlayerOneScore = 0;
    private int m_PlayerTwoScore = 0;
    private string m_PlayerOneName = null;
    private string m_PlayerTwoName = null;
    private string m_RoundWinner = null;
    private string m_ScoreStatusMessage = null;

    private void Update()
    {
        m_ScoreText.text = m_ScoreStatusMessage;
    }

    public void InitializeScoreHandler(string i_PlayerOneName, string i_PlayerTwoName)
    {
        Reset();
        m_PlayerOneName = i_PlayerOneName;
        m_PlayerTwoName = i_PlayerTwoName;
        updateScoreStatusMessage();
    }

    public void UpdateWinner(string i_PlayerName)
    {
        if (i_PlayerName == m_PlayerOneName)
        {
            m_PlayerOneScore++;
        }
        else
        {
            m_PlayerTwoScore++;
        }

        m_RoundWinner = i_PlayerName;
        updateScoreStatusMessage();
    }

    private void updateWinnerMessage()
    {
        string winnerMessage = $"{m_RoundWinner} Won!{Environment.NewLine}{m_ScoreStatusMessage}";

        WinnerMessage = winnerMessage;
    }

    private void updateTieMessage()
    {
        string tieMessage = $"It's a tie!{Environment.NewLine}{m_ScoreStatusMessage}";

        TieMessage = tieMessage;
    }

    private void updateScoreStatusMessage()
    {
        string singularOrPluralPlayerOneScore = m_PlayerOneScore == 1 ? "point" : "points";
        string singularOrPluralPlayerTwoScore = m_PlayerTwoScore == 1 ? "point" : "points";
        string scoreSatusMessage = $"{m_PlayerOneName} has {m_PlayerOneScore} {singularOrPluralPlayerOneScore}." +
            $"{Environment.NewLine}{m_PlayerTwoName} has {m_PlayerTwoScore} {singularOrPluralPlayerTwoScore}.";

        m_ScoreStatusMessage = scoreSatusMessage;
        updateWinnerMessage();
        updateTieMessage();
    }

    public void Reset()
    {
        m_PlayerTurnText = null;
        m_ScoreText = null;
        m_ScoreText2 = null;
        WinnerMessage = null;
        TieMessage = null;
        m_PlayerOneScore = 0;
        m_PlayerTwoScore = 0;
        m_PlayerOneName = null;
        m_PlayerTwoName = null;
        m_RoundWinner = null;
        m_ScoreStatusMessage = null;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameBoardSizeCanvas : MonoBehaviour
{
    [SerializeField] private GameManager m_GameLogic = null;
    [SerializeField] private TextMeshProUGUI m_BoardGameSizeText = null;
    [SerializeField] private Slider m_BoardGameSizeSlider = null;
    private string[] m_BoardSizeMsgs = null;

    void Start()
    {
        createBoardSizeOptionsText();
    }

    public void sliderChooseBoardSize_slide()
    {
        int boardSizeIndx = (int)m_BoardGameSizeSlider.value;

        m_BoardGameSizeText.text = m_BoardSizeMsgs[boardSizeIndx];
    }

    private void createBoardSizeOptionsText()
    {
        int msgIndx = 0;
        int boardMinLength = m_GameLogic.BoardMinLength;
        int optionsLength = m_GameLogic.BoardMaxLength - m_GameLogic.BoardMinLength + 1;
        int numberOfBoardSizeMsgs = optionsLength * optionsLength;

        m_BoardSizeMsgs = new string[numberOfBoardSizeMsgs];
        m_BoardGameSizeSlider.maxValue = numberOfBoardSizeMsgs - 1;

        for (int i = 0; i < optionsLength; i++)
        {
            for (int j = 0; j < optionsLength; j++)
            {
                m_BoardSizeMsgs[msgIndx++] = $"{i + boardMinLength} X {j + boardMinLength}";
            }
        }
    }

    public void btnSetBoardSize_click()
    {
        int boardMsgIndx = (int)m_BoardGameSizeSlider.value;
        string boardSizeMsg = m_BoardSizeMsgs[boardMsgIndx];
        int rowLength = boardSizeMsg[0] - '0';
        int columnLength = boardSizeMsg[boardSizeMsg.Length - 1] - '0';
        
        try
        {
            m_GameLogic.InitializeBoardGame(rowLength, columnLength);
        }
        catch (ArgumentException ex)
        {
            string exceptionMessage = string.Format("Invalid input: ", ex.Message);

            Console.WriteLine(exceptionMessage);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private List<Canvas> m_CanvasMenuItems = null;

    void Start()
    {
        GameManager.m_Instance.ShowPlayAgainCanvas += playAnotherRound_Action;
        changeState(eUIState.MainMenu);
    }

    private void changeState(eUIState i_NewState)
    {
        disableAllCanvases();
        m_CanvasMenuItems[(int)i_NewState].enabled = true;
    }

    private void disableAllCanvases()
    {
        foreach(Canvas canvas in m_CanvasMenuItems)
        {
            canvas.enabled = false;
        }
    }

    public void btnStartGame_clicked()
    {
        changeState(eUIState.ChooseGameBoardSize);
    }

    public void btnSetBoardSize_clicked()
    {
        changeState(eUIState.ChooseGameMode);
    }

    public void btnSetGameMode_clicked()
    {
        changeState(eUIState.GameScore);
    }

    public void btnPlayAgain_clicked()
    {
        changeState(eUIState.GameScore);
    }

    public void btnQuit_clicked()
    {
        changeState(eUIState.MainMenu);
    }

    public void btnExitGame_clicked()
    {
        Application.Quit();
    }

    public void playAnotherRound_Action()
    {
        changeState(eUIState.PlayAgain);
    }
}

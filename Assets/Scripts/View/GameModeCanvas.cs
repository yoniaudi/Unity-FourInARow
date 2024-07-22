using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeCanvas : MonoBehaviour
{
    [SerializeField] private GameManager m_GameLogic = null;

    public void btnSinglePlayerAI_click()
    {
        initializeGameMode("SinglePlayer");
    }

    public void btnMultiplayer_click()
    {
        initializeGameMode("Multiplayer");
    }

    private void initializeGameMode(string i_GameMode)
    {
        m_GameLogic.SetGameMode(i_GameMode);
    }
}

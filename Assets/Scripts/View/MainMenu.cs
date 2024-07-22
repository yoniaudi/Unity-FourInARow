using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private List<Button> m_MainMenuButtons = null;

    public void btnStartGame_click()
    {
        //initializeGameMode("Multiplayer");
    }

    public void btnOptions_click()
    {
        //initializeGameMode("Multiplayer");
    }

    public void btnExitGame_click()
    {
        Application.Quit();
    }
}

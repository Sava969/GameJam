using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class UIControls : MonoBehaviour
{
    public GameObject MainMenuPanel;
    public GameObject PauseMenuUI;
    public GameObject LoseScreen;
    //public GameObject creditsMenuUI;
    

    private bool mainmenuOn = false;

    public static bool GameIsPaused = false;

    public string newScene;

    public void StartGame()
    {
        // Hide the main menu
        MainMenuPanel.SetActive(false);

        // Unpause the game
        Time.timeScale = 1f;
        GameIsPaused = false;
    }


    void Update()
    {
        if (EscapePressedThisFrame())
        {
            if (mainmenuOn == false)
            {
                if (GameIsPaused)
                    Resume();
                else
                    Pause();
            }
        }
    }

    public void Resume()
    {
        if (PauseMenuUI != null)
            PauseMenuUI.SetActive(false);

        Time.timeScale = 1f;
        GameIsPaused = false;
        
    }

    public void Pause()
    {
        if (PauseMenuUI != null)
            PauseMenuUI.SetActive(true);

        Time.timeScale = 0f;
        GameIsPaused = true;
        
    }

    public void LoadScene()
    {
        Time.timeScale = 1f;
        Debug.Log("loading menu");
        SceneManager.LoadScene("Level01");
    }

    public void QuitGame()
    {
        Debug.Log("Quiting Game");
        Application.Quit();
    }

    public void mainmenuToggle()
    {
        if (mainmenuOn == false)
        {
            if (MainMenuPanel != null)
                MainMenuPanel.SetActive(true);

            Time.timeScale = 0f;
            GameIsPaused = true;
        }
        else
        {
            if (MainMenuPanel != null)
                MainMenuPanel.SetActive(false);

            Time.timeScale = 1f;
            GameIsPaused = false;
        }

        mainmenuOn = !mainmenuOn;
    }

    // Prefer the new Input System; fall back to legacy Input.GetKeyDown safely.
    private bool EscapePressedThisFrame()
    {
        // New Input System
        if (Keyboard.current != null && Keyboard.current.escapeKey != null)
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame) return true;
        }

        // Safe legacy fallback (will not throw if legacy input is disabled)
        try
        {
            return Input.GetKeyDown(KeyCode.Escape);
        }
        catch (System.InvalidOperationException)
        {
            return false;
        }
    }

    public void RestartGame()
    {
        LoseScreen.SetActive(false);
        Time.timeScale = 1f; // make sure the game isn't paused
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    public void OpenCredits()
    {
        MainMenuPanel.SetActive(false);
        //creditsMenuUI.SetActive(true);
    }

    public void CloseCredits()
    {
        //creditsMenuUI.SetActive(false);
        MainMenuPanel.SetActive(true);
    }

    public void ShowLoseScreen()
    {
        Time.timeScale = 0f;
        LoseScreen.SetActive(true);
    }


}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] LevelLoader levelLoader;
    [SerializeField] GameObject pauseMenuUI;
    // [SerializeField] GameManager gameManager;
    public static bool isPaused;
    // * did not work:
    // bool isPlayerSwinging; //temp method to ensure player's isSwinging state is set after unpause, preventing immediate falling

    bool CheckPauseInput()
    {
        return Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P);
    }

    void Update()
    {
        if (CheckPauseInput())
        {
            if (isPaused)
            {
                UnpauseGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        // isPlayerSwinging = gameManager.playerRef.GetComponent<SwingingController>().isSwinging;
        // Debug.Log("Paused: isSwinging: " + isPlayerSwinging);
        Cursor.visible = true;
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void UnpauseGame()
    {
        Cursor.visible = false;
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        // gameManager.playerRef.GetComponent<SwingingController>().isSwinging = isPlayerSwinging;
        // Debug.Log("UnPaused: isSwinging: " + isPlayerSwinging);
        isPaused = false;
    }

    public void LoadStartMenu()
    {
        UnpauseGame();
        levelLoader.LoadStartMenu();
    }

    public void QuitGame()
    {
#if UNITY_WEBGL
        LoadStartMenu();
        return;
#endif
        Application.Quit();
    }
}

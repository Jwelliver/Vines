using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] LevelLoader levelLoader;
    [SerializeField] GameObject pauseMenuUI;
    public static bool isPaused;

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
            LoadMenu();
            return;
#endif
        Application.Quit();
    }
}

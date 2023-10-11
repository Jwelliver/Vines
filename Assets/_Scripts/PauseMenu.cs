
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenuUI;
    // [SerializeField] GameManager gameManager;
    public static bool isPaused;


    // * did not work:
    // bool isPlayerSwinging; //temp method to ensure player's isSwinging state is set after unpause, preventing immediate falling

    public void OnPauseButtonPressed()
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

    public void PauseGame()
    {
        // isPlayerSwinging = gameManager.playerRef.GetComponent<SwingingController>().isSwinging;
        // Debug.Log("Paused: isSwinging: " + isPlayerSwinging);
        Cursor.visible = true;
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        GameContext.SetGameState(GameState.PauseMenu);
    }

    public void UnpauseGame()
    {
        Cursor.visible = false;
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        // gameManager.playerRef.GetComponent<SwingingController>().isSwinging = isPlayerSwinging;
        // Debug.Log("UnPaused: isSwinging: " + isPlayerSwinging);
        isPaused = false;
        GameContext.SetGameState(GameState.InGame);
    }

    public void LoadStartMenu()
    {
        UnpauseGame();
        SceneLoader.FadeToScene(SceneRef.MainMenu);
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

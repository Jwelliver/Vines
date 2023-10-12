
using UnityEngine;

public class PauseMenu : MonoBehaviour
{

    [SerializeField] GameObject pauseMenu;
    public static PauseMenu Instance;
    public bool isPaused;

    void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this);
        }

    }

    public void OnPauseButtonPressed()
    {
        if (isPaused) { UnpauseGame(); }
        else { PauseGame(); }
    }

    public void PauseGame()
    {
        Cursor.visible = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        GameContext.SetGameState(GameState.PauseMenu);
    }

    public void UnpauseGame()
    {
        Cursor.visible = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
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

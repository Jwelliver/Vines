using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] GameObject gameOverCanvasContainer;
    [SerializeField] GameObject keyboardPanel;
    [SerializeField] GameObject mobilePanel;
    [SerializeField] StatsTextDisplay statsTextDisplay;
    [SerializeField] Animator fadeInAnimator;
    [SerializeField] LevelLoader levelLoader;

    // private ControlTextByPlatform restartText = new ControlTextByPlatform
    // {
    //     keyboard = "Press 'R' to restart",
    //     mobile = "Tap here to restart"
    // };

    void Awake()
    {
        gameOverCanvasContainer.SetActive(false);
    }

    void ActivatePlatformUI()
    {
        if (GameContext.PlayerSettings.useTouchScreenControls)
        {
            mobilePanel.SetActive(true);
        }
        else
        {
            keyboardPanel.SetActive(true);
        }
    }

    public void ShowGameOverUI()
    {
        Cursor.visible = true;
        // Activate the relevant UI for the platform
        ActivatePlatformUI();
        // Refresh Stats Text
        statsTextDisplay.RefreshStatsText();
        // Activate the primary container
        gameOverCanvasContainer.SetActive(true);
        // Begin fade in
        fadeInAnimator.SetTrigger("StartDeathText");
    }

    public void OnRestart() //triggered by triggering restart from gameover screen.
    {
        // play some sound.
        //
        levelLoader.reloadCurrentLevel();
    }
}

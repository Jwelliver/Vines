using UnityEngine;


public enum GameState
{
    MainMenu,
    SettingsMenu,
    GameLoading,
    InGame,
    PauseMenu,
    GameOver
}

public struct PlayerSettings
{

    public bool useTouchScreenControls;
    public int vsyncCount;
    public int targetFrameRate;
    public PlayerSettings(bool loadOnInit = true)
    {
        //TODO: if loadOnInit, try to  Load existing settings; otherwise use defaults
        useTouchScreenControls = GameContext.IsMobilePlatform();
        vsyncCount = QualitySettings.vSyncCount;
        targetFrameRate = Application.targetFrameRate;

    }

}


[CreateAssetMenu(menuName = "MyAssets/ScriptableObjects/GameContext")]
public class GameContext : ScriptableObject
{
    public static GameState CurrentGameState = new GameState();
    public static PlayerSettings ActiveSettings = new PlayerSettings(true);

    void OnEnable()
    {
        CurrentGameState = GameState.MainMenu;
    }

    public static void SetGameState(GameState newGameState)
    {
        CurrentGameState = newGameState;
    }

    public static bool IsMobilePlatform() //TODO: should move to dedicated util class (platformutil or something)
    {
        //TODO: figure out how to ID ipad on webgl with JS.
        return Application.isMobilePlatform;
    }



}

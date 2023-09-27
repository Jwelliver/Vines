using UnityEngine;


public enum GameState
{
    MainMenu,
    GameLoading,
    InGame,
    PauseMenu,
    GameOver
}

public class PlayerSettings
{
    public bool useTouchScreenControls = GameContext.IsMobilePlatform();
}


[CreateAssetMenu(menuName = "MyAssets/ScriptableObjects/GameContext")]
public class GameContext : ScriptableObject
{
    public static GameState CurrentGameState = new GameState();
    public static PlayerSettings PlayerSettings = new PlayerSettings();

    void OnEnable()
    {
        CurrentGameState = GameState.MainMenu;
    }

    public static void SetGameState(GameState newGameState)
    {
        CurrentGameState = newGameState;
    }

    public static bool IsMobilePlatform()
    {
        //TODO: figure out how to ID ipad on webgl with JS.
        return Application.isMobilePlatform;
    }



}

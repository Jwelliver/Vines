using System;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum SceneRef
{
    Current = -1,
    Next = -2,
    MainMenu = 0,
    SettingsMenu = 1,
    IntroText = 2, //i.e. cutScene
    GameMain = 3,
    Win = 4, // after amulet og win scene
    Credits = 5,


}

public enum SceneTransitionType
{
    None = 0,
    Fade = 1,
    FadeOutAndWait = 2
}

// [CreateAssetMenu(menuName = "MyAssets/ScriptableObjects/SceneLoader")]
public class SceneLoader : MonoBehaviour
{
    // [SerializeField] float transitionTime = 2f;
    // [SerializeField] bool allowNextLevelKey; //TODO: find references and refactor
    // [SerializeField] KeyCode nextLevelKeyCode = KeyCode.Space; //TODO: move this out of here and trigger on event; 


    static SceneLoader Instance;
    public static bool isLoadingNewScene;

    private static int CurrentSceneIndex => SceneManager.GetActiveScene().buildIndex;
    private static int NextSceneIndex => CurrentSceneIndex + 1;

    void Awake()
    {
        //Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        SceneManager.sceneLoaded += CustomOnSceneLoad.OnSceneLoad;
        SceneManager.sceneUnloaded += CustomSceneUnload.OnSceneUnload;
    }


    void OnApplicationQuit()
    {
        Instance = null;
        SceneManager.sceneLoaded -= CustomOnSceneLoad.OnSceneLoad;
        SceneManager.sceneUnloaded -= CustomSceneUnload.OnSceneUnload;
    }

    public static void LoadNextScene()
    {
        FadeToScene(SceneRef.Next);
    }

    public static void ReloadCurrentScene()
    {
        FadeToScene(SceneRef.Current);
    }

    private static int GetBuildIndexFromSceneRef(SceneRef sceneRef)
    {
        switch (sceneRef)
        {
            case SceneRef.Current:
                {
                    return CurrentSceneIndex;
                }
            case SceneRef.Next:
                {
                    return NextSceneIndex;
                }
            default:
                {
                    return (int)sceneRef;
                }
        }
    }

    public static void LoadSceneNoTransition(SceneRef sceneRef)
    {
        // Return if already loading scene;
        if (isLoadingNewScene) { return; }
        isLoadingNewScene = true;
        SceneManager.LoadScene((int)sceneRef);
    }

    public static void FadeToScene(SceneRef sceneRef, float duration = 2f, bool autoFadeInAfterLoad = true, Action OnFadeOutComplete = null, Action OnFadeInComplete = null)
    {
        Debug.Log("FadeToScene() entry > isLoadingNewScene: " + isLoadingNewScene);
        // Return if already loading scene;
        if (isLoadingNewScene) { return; }
        isLoadingNewScene = true;
        int buildIndex = GetBuildIndexFromSceneRef(sceneRef);
        CrossFadeCanvas.FadeToOpaque(duration, () =>
        {
            if (OnFadeOutComplete != null) OnFadeOutComplete.Invoke();
            SceneManager.LoadScene(buildIndex);
            if (autoFadeInAfterLoad)
            {
                CrossFadeCanvas.FadeToTransparent(duration, OnFadeInComplete);
            }
        });
    }
}


public static class CustomSceneUnload
{
    //CustomSceneUnload holds Logic to run right before unloading a scene

    // Primary entry point OnSceneUnload 
    public static void OnSceneUnload(Scene scene)
    {
        SceneRef sceneRef = (SceneRef)scene.buildIndex;
        switch (sceneRef)
        {
            case SceneRef.GameMain:
                {
                    UnloadGameMain();
                    break;
                }
        }
    }

    static void UnloadGameMain()
    {
        // remove persistant audio
        GameObject[] audioPlayers = GameObject.FindGameObjectsWithTag("PersistantAudio");
        // Debug.Log("Number of audio players: " + audioPlayers.Length);
        foreach (GameObject audio in audioPlayers)
        {
            GameObject.Destroy(audio);
        }
        Resources.UnloadUnusedAssets();//added 090623 in attempt to address mem issue and crash
    }
}

public static class CustomOnSceneLoad
{
    //CustomSceneUnload holds Logic to run right after loading a scene

    // Primary entry point OnSceneLoad 
    public static void OnSceneLoad(Scene scene, LoadSceneMode loadSceneMode)
    {
        // Reset isLoadingNewScene flag;
        Debug.Log("OnSceneLoad() > entry; Setting isLoadingNewScene False");
        SceneLoader.isLoadingNewScene = false;

        SceneRef sceneRef = (SceneRef)scene.buildIndex;
        switch (sceneRef)
        {
            case SceneRef.MainMenu:
                {
                    GameContext.SetGameState(GameState.MainMenu);
                    Cursor.visible = true;
                    break;
                }
            case SceneRef.SettingsMenu:
                {
                    GameContext.SetGameState(GameState.SettingsMenu);
                    break;
                }
            case SceneRef.GameMain:
                {
                    GameContext.SetGameState(GameState.InGame);
                    Cursor.visible = false;
                    break;
                }
        }
    }
}
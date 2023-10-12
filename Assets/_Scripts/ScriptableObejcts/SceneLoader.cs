using System;
using System.Collections;
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

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;
    public static bool isLoadingNewScene;
    public static float sceneLoadingProgress;

    private static int CurrentSceneIndex => SceneManager.GetActiveScene().buildIndex;
    private static int NextSceneIndex => CurrentSceneIndex + 1;

    private static WaitForSeconds asyncSceneLoadProgressCheckInterval;

    void Awake()
    {
        //Singleton
        if (Instance == null)
        {
            Instance = this;
            Init();
        }
        else if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Init()
    {
        asyncSceneLoadProgressCheckInterval = new WaitForSeconds(0.1f);
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
        FadeToScene(SceneRef.Current, 1.75f, false);
    }

    public static void LoadSceneNoTransition(SceneRef sceneRef)
    {
        // Return if already loading scene;
        if (isLoadingNewScene) { return; }
        isLoadingNewScene = true;
        SceneManager.LoadScene((int)sceneRef);
    }

    public void LoadSceneAsync(SceneRef sceneRef, bool loadSceneOnComplete, Action<AsyncOperation> OnSceneLoadComplete = null)
    {
        StartCoroutine(_LoadSceneAsync(sceneRef, loadSceneOnComplete, OnSceneLoadComplete));
    }

    private static IEnumerator _LoadSceneAsync(SceneRef sceneRef, bool loadSceneOnComplete = true, Action<AsyncOperation> OnSceneLoadComplete = null)
    {
        // Handle case where loadSceneOnComplete is falde and no callback was provided.
        if (!loadSceneOnComplete && OnSceneLoadComplete == null)
        {
            Debug.LogError("Error: Either loadSceneOnComplete must be true or the AsyncSceneOperation must be handled in a provided OnSceneLoadComplete callback, otherwise the Scene will never load. Aborting.");
            yield break;
        }

        sceneLoadingProgress = 0f;
        isLoadingNewScene = true;

        AsyncOperation scene = SceneManager.LoadSceneAsync(GetBuildIndexFromSceneRef(sceneRef));
        scene.allowSceneActivation = false;

        // Wait while the scene is loaded;
        while (!scene.isDone) //Notes were that Scene is Fully loaded at 0.9f instead of 1f?
        {
            sceneLoadingProgress = scene.progress;
            yield return asyncSceneLoadProgressCheckInterval;
        }

        // Call the callback if one exists;
        if (OnSceneLoadComplete != null)
        {
            OnSceneLoadComplete.Invoke(scene);
        }

        //load scene if requested
        if (loadSceneOnComplete) scene.allowSceneActivation = true;
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
            OnFadeOutComplete?.Invoke();
            SceneManager.LoadScene(buildIndex);
            if (autoFadeInAfterLoad)
            {
                CrossFadeCanvas.FadeToTransparent(duration, OnFadeInComplete);
            }
        });
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
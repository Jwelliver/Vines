using System;
using UnityEngine;

public class CrossFadeCanvas : MonoBehaviour
{
    public bool fadeInOnStart = false;
    public float defaultFadeTime = 2f;
    static Canvas canvasMain; // this is what will be enabled/disabled
    static CanvasGroup targetCanvasGroup;
    static CrossFadeCanvas Instance;
    public static bool isOpaque;
    public static bool isFading;

    void Awake()
    {
        //Singleton
        if (Instance == null) { Instance = this; Init(); }
        else if (Instance != this) { Debug.Log("CrossFadeCanvas Singleton; OtherInstanceFound: Destroying Self"); Destroy(gameObject); }
    }

    void Start()
    {
        if (fadeInOnStart)
        {
            FadeToTransparent(defaultFadeTime);
        }
    }

    void OnApplicationQuit()
    {
        //Cleanup
        targetCanvasGroup = null;
        canvasMain = null;
        Instance = null;
    }

    void Init()
    {
        // Init self
        DontDestroyOnLoad(gameObject);
        targetCanvasGroup = GetComponentInChildren<CanvasGroup>();
        canvasMain = transform.Find("CanvasMain").GetComponent<Canvas>();

        // If target Canvas not found; destroy self
        if (targetCanvasGroup == null)
        {
            Debug.LogError("targetCanvasGroup not found in children. Destroying Self");
            Destroy(this);
        }
        // Initialize targetCanvasGroup
        else if (fadeInOnStart)
        {
            canvasMain.enabled = false;
            targetCanvasGroup.alpha = 1f;
            return;
        }
        else
        {
            canvasMain.enabled = false;
            targetCanvasGroup.alpha = 0f;
        }
    }

    public static void FadeToTransparent(float duration, Action OnFadeComplete = null)
    {  //Used by callers to Fade in manually
        if (targetCanvasGroup == null) { OnNotInitialized(); return; }
        //Ensure alpha starts opaque so we can fade in
        targetCanvasGroup.alpha = 1f;
        //Ensure canvas in enabled
        if (!canvasMain.enabled) canvasMain.enabled = true;

        isFading = true;
        //Perform FadeIn
        FadeCanvas.FadeToTransparent(targetCanvasGroup, duration, () =>
        {
            //Disable canvas after fade in is complete
            canvasMain.enabled = false;
            isFading = false;
            isOpaque = false;
            // Call OnFadeComplete callback if provided
            if (OnFadeComplete != null) OnFadeComplete.Invoke();
        });
    }

    public static void FadeToOpaque(float duration, Action OnFadeComplete = null)
    {
        if (targetCanvasGroup == null) { OnNotInitialized(); return; }
        //Ensure alpha starts at transparent so we can fade out
        targetCanvasGroup.alpha = 0f;
        //Ensure canvas is enabled
        if (!canvasMain.enabled) canvasMain.enabled = true;
        isFading = true;
        //Perform FadeOut
        FadeCanvas.FadeToOpaque(targetCanvasGroup, duration, () =>
        {
            isFading = false;
            isOpaque = true;
            if (OnFadeComplete != null) OnFadeComplete.Invoke();
        });
    }

    private static void OnNotInitialized()
    {
        Debug.LogError("Error: CrossFadeCanvas Not initialized; Static method called but no targetCanvasGroup was found.");
    }

    // void Update() //TODO: Testing only; Remove later
    // {
    //     // if (Input.GetKeyDown(KeyCode.T))
    //     // {
    //     //     if (isFading)
    //     //     {
    //     //         Debug.Log("Is Fading; cannot transition");
    //     //         return;
    //     //     }
    //     //     if (isOpaque)
    //     //     {
    //     //         Debug.Log("Fading To Transparent...");
    //     //         FadeToTransparent(2f, () => Debug.Log("Fade to Transparent Complete"));
    //     //     }
    //     //     else
    //     //     {
    //     //         Debug.Log("Fading To Opaque...");
    //     //         FadeToOpaque(2f, () => Debug.Log("Fade to Opaque Complete"));
    //     //     }
    //     // }

    //     // if (Input.GetKeyDown(KeyCode.T))
    //     // {
    //     //     SceneLoader.FadeToScene(SceneRef.MainMenu, 3f, true, () => Debug.Log("FadeOutComplete"), () => Debug.Log("FadeInComplete"));
    //     // }
    // }

}

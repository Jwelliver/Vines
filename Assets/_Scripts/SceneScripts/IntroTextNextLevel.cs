using System.Collections;
using UnityEngine;

public class IntroTextNextLevel : MonoBehaviour
{
    PersistentAudio music;
    [SerializeField] AudioSource jungleSound;
    // [SerializeField] TMPro.TextMeshProUGUI mainText; //this is the text that displays on the screen
    [SerializeField] FadeText mainTextFadeController;

    bool transitionStarted = false;

    void Awake()
    {
        try
        {
            music = GameObject.Find("Music").GetComponent<PersistentAudio>();
        }
        catch
        {
            Debug.LogError("Music Object not found");
        }
    }

    void Start()
    {
        //Append ControlText to mainText;
        bool isMobilePlatform = GameContext.IsMobilePlatform();
        string controlText = isMobilePlatform ? "Tap to continue..." : "Press spacebar to continue...";
        mainTextFadeController.textMeshGUI.text += "\n\n" + controlText;
        mainTextFadeController.FadeIn(StartTransition);
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: update this input 
        if (Input.GetKeyDown(KeyCode.Space) && !transitionStarted)
        {
            StartTransition();
        }
    }

    public void StartTransition()
    {
        if (transitionStarted) return;
        StartCoroutine(Transition());
    }

    IEnumerator Transition()
    {
        if (transitionStarted) { yield break; }
        transitionStarted = true;
        // musicFadeOut.SetTrigger("FadeOut");
        if (music != null) music.fadeOutAndStop(1.5f);
        jungleSound.Play();
        yield return new WaitForSeconds(0.5f);
        // musicFadeOut.transform.GetComponent<AudioSource>().Stop();
        SceneLoader.FadeToScene(SceneRef.GameMain, 2f, false); //TODO: Set autoFadeIn on start to false; We're going to call it manually
    }
}

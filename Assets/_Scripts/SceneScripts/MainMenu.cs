using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Transform PersistantAudioPrefab;
    [SerializeField] AudioClip mainMenuMusic;

    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.FindObjectsOfType<PersistentAudio>().Length == 0)
        {
            Transform menuMusic = GameObject.Instantiate(PersistantAudioPrefab);
            menuMusic.name = "Music";
            menuMusic.GetComponent<PersistentAudio>().playClip(mainMenuMusic);
        }
    }

    public void OnSettingsButtonClicked()
    {
        SceneLoader.LoadSceneNoTransition(SceneRef.SettingsMenu);
    }

    public void OnPlayButtonClicked()
    {
        // Goto intro text/cutscene
        // CrossFadeCanvas.FadeToOpaque(1.5f, () => { SceneLoader.LoadSceneNoTransition(SceneRef.IntroText); });
        SceneLoader.FadeToScene(SceneRef.IntroText);
    }
}

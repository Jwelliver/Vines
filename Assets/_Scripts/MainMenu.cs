using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Transform PersistantAudioPrefab;
    [SerializeField] AudioClip mainMenuMusic;

    // Start is called before the first frame update
    void Start()
    {
        GameContext.SetGameState(GameState.MainMenu);
        if (GameObject.FindObjectsOfType<PersistentAudio>().Length == 0)
        {
            Transform menuMusic = GameObject.Instantiate(PersistantAudioPrefab);
            menuMusic.name = "Music";
            menuMusic.GetComponent<PersistentAudio>().playClip(mainMenuMusic);
        }
    }
}

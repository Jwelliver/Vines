using System;
using System.Collections;
using UnityEngine;


//TODO: Refactor (092123);
/*
    - Extract audio clips and sources;
    - Extract amuletFoundText and Animator (build into own module, rename for generic use)
    - 

*/
public class GameManager : MonoBehaviour
{
    public LevelGenerator levelGen;
    [SerializeField] LevelLoader levelLoader;
    [SerializeField] AudioClip winSound;
    [SerializeField] AudioClip winMusic;
    [SerializeField] AudioClip amuletPickupSound;
    [SerializeField] AudioClip amuletFoundTribalDrums;
    [SerializeField] AudioClip tribeChaseMusic;

    [SerializeField] Animator amuletFoundTextAnimator;
    TMPro.TextMeshProUGUI amuletFoundText;
    [SerializeField] ArrowGenerator arrowGenerator;
    [SerializeField] SpriteRenderer playerAmulet;
    public Transform playerRef;
    public Camera cameraRef;
    AudioSource audioSource;
    PersistentAudio music;
    public bool playerHasAmulet;

    void Awake()
    {
        amuletFoundText = GameObject.Find("AmuletFoundText").GetComponent<TMPro.TextMeshProUGUI>();
        try
        {
            music = GameObject.Find("Music").GetComponent<PersistentAudio>();
        }
        catch
        {
            music = null;
        }

        audioSource = GetComponent<AudioSource>();
        // Cursor.visible = false; //TODO reimplement
        levelGen.InitLevel();
        GameContext.SetGameState(GameState.InGame);
    }

    void HandleNewSectionGeneration()
    {
        // *TODO TEMP code to test endless mode; todo: move
        Section currentSection = levelGen.GetCurrentSection();
        float endX = currentSection.startPos.x + currentSection.length;
        if (Mathf.Abs(playerRef.position.x - endX) < 30f)
        {
            levelGen.ExtendCurrentSection();
            Debug.Log("Appending NewSection");
        }
    }

    public void win()
    {
        // Debug.Log("You Won!");
        playWinMusic();
        levelLoader.loadNextLevel();
    }

    public void playerFoundAmulet()
    {
        // Debug.Log("AmuletFound!");
        playerHasAmulet = true;
        playerAmulet.enabled = true;
        audioSource.PlayOneShot(amuletPickupSound);
        StartCoroutine(showAmuletFoundText());
    }

    IEnumerator showAmuletFoundText()
    {
        // Debug.Log("AmuletFound!");
        amuletFoundTextAnimator.SetTrigger("FadeIn");
        yield return new WaitForSeconds(3f);
        amuletFoundTextAnimator.SetTrigger("FadeOut");
        // Debug.Log("Playing tribal war drums!");
        audioSource.PlayOneShot(amuletFoundTribalDrums);
        yield return new WaitForSeconds(2f);
        // Debug.Log("Displaying next text");
        amuletFoundText.text = "Are those war drums? I better get out of here quick!";
        amuletFoundTextAnimator.SetTrigger("FadeIn");
        yield return new WaitForSeconds(2f);
        audioSource.PlayOneShot(tribeChaseMusic);
        amuletFoundTextAnimator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(4f);
        arrowGenerator.enabled = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            levelLoader.reloadCurrentLevel();
        }
        //TODO: Move this out of here; Come up with a better way of handling the endless generatino
        if (levelGen.levelSettings.levelType == LevelType.ENDLESS)
        {
            HandleNewSectionGeneration();
        }

    }

    void playWinMusic()
    {
        if (!music) { Debug.Log("Music Component Not Found."); return; }
        music.reset();
        music.playClip(winSound);

        // audioSource.PlayOneShot(winSound);
        // music.volume = 1;
        // music.Play();
    }


}

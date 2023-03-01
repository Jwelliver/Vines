using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
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

  

    AudioSource audioSource;
    

    PersistentAudio music;


    public bool playerHasAmulet;

    void Start() {
        amuletFoundText = GameObject.Find("AmuletFoundText").GetComponent<TMPro.TextMeshProUGUI>();
        music = GameObject.Find("Music").GetComponent<PersistentAudio>();
        audioSource = GetComponent<AudioSource>();
    }
    public void win() {
        // Debug.Log("You Won!");
        playWinMusic();
        levelLoader.loadNextLevel();
    }

    public void playerFoundAmulet() {
        // Debug.Log("AmuletFound!");
        playerHasAmulet=true;
        playerAmulet.enabled=true;
        audioSource.PlayOneShot(amuletPickupSound);
        StartCoroutine(showAmuletFoundText());
    }

    IEnumerator showAmuletFoundText() {
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

    void Update() {
        if(Input.GetKeyDown(KeyCode.R)) {
            levelLoader.reloadCurrentLevel();
        }
    }

    void playWinMusic() {
        music.reset();
        music.playClip(winSound);
        
        // audioSource.PlayOneShot(winSound);
        // music.volume = 1;
        // music.Play();
    }
}

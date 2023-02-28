using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroTextNextLevel : MonoBehaviour
{
    [SerializeField] LevelLoader levelLoader;
    PersistentAudio music;
    [SerializeField] AudioSource jungleSound;
    


    void Start() {
        // musicFadeOut = GameObject.Find("Music").GetComponent<Animator>();
        music = GameObject.Find("Music").GetComponent<PersistentAudio>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) {
            StartCoroutine(transition());
        }
    }

    IEnumerator transition() {

        // musicFadeOut.SetTrigger("FadeOut");
        music.fadeOutAndStop();
        jungleSound.Play();
        yield return new WaitForSeconds(2f);
        // musicFadeOut.transform.GetComponent<AudioSource>().Stop();
        levelLoader.loadNextLevel();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroTextNextLevel : MonoBehaviour
{
    [SerializeField] LevelLoader levelLoader;
    PersistentAudio music;
    [SerializeField] AudioSource jungleSound;

    bool transitionStarted;

    void Awake()
    {
        // musicFadeOut = GameObject.Find("Music").GetComponent<Animator>();
        music = GameObject.Find("Music").GetComponent<PersistentAudio>();
        // music = GameObject.FindObjectOfType<PersistentAudio>();
        // if (music == null)
        // {
        //     Debug.LogError("Error: Music Not Found");
        // }
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: update this input 
        if (Input.GetKeyDown(KeyCode.Space) && !transitionStarted)
        {
            // startTransition();
            LoadNext();

        }
    }

    public void LoadNext()
    {
        StartCoroutine(transition());
    }

    // void startTransition()
    // {
    //     transitionStarted = true;
    //     // music.onFadeComplete += finishTransition;
    //     music.fadeOutAndStop(1.5f);
    //     levelLoader.loadNextLevel();
    //     jungleSound.Play();
    // }

    // void finishTransition()
    // {
    //     levelLoader.loadNextLevel();
    // }


    IEnumerator transition()
    {
        // musicFadeOut.SetTrigger("FadeOut");
        music.fadeOutAndStop(2f);
        jungleSound.Play();
        yield return new WaitForSeconds(2f);
        // musicFadeOut.transform.GetComponent<AudioSource>().Stop();
        levelLoader.loadNextLevel();
    }
}

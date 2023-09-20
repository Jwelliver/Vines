using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentAudio : MonoBehaviour
{
    public Action onFadeComplete;
    Animator animator;
    AudioSource audioSource;

    private float fadeTime;
    private bool isFading;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        // if (isFading)
        // {
        //     continueFade();
        // }
    }

    public void play()
    {
        audioSource.Play();
    }

    public void playClip(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void reset()
    {
        audioSource.Stop();
        animator.SetTrigger("Reset");
    }

    public void fadeOutAndStop(float time)
    {
        // fadeTime = time;
        // isFading = true;
        animator.SetTrigger("FadeOut");
    }

    // void continueFade()
    // {
    //     if (audioSource != null && audioSource.isPlaying && audioSource.volume > 0 && fadeTime > 0)
    //     {
    //         // Decrease volume
    //         audioSource.volume -= Time.deltaTime / fadeTime;

    //         if (audioSource.volume < 0)
    //         {
    //             audioSource.volume = 0;
    //             isFading = false;
    //             return;
    //         }

    //         // Decrease fade out time
    //         fadeTime -= Time.deltaTime;

    //         if (fadeTime <= 0)
    //         {
    //             fadeTime = 0;
    //             isFading = false;
    //             if (onFadeComplete != null)
    //             {
    //                 onFadeComplete.Invoke();
    //             }

    //         }
    //     }
    //     else
    //     {
    //         isFading = false;
    //     }
    // }


    void OnDestroy()
    {
        audioSource.Stop();
        audioSource.clip = null;
    }
}

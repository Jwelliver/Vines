using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentAudio : MonoBehaviour
{
    Animator animator;
    AudioSource audioSource;
    void Start() {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        DontDestroyOnLoad(gameObject);
    }

    public void play() {
        audioSource.Play();
    }

    public void playClip(AudioClip clip) {
        audioSource.PlayOneShot(clip);
    }

    public void reset() {
        audioSource.Stop();
        animator.SetTrigger("Reset");
    }

    public void fadeOutAndStop() {
        animator.SetTrigger("FadeOut");
    }

    void OnDestroy() {
        audioSource.Stop();
        audioSource.clip=null;
    }
}

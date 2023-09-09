using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineSFX : MonoBehaviour
{

    [Header("AudioSources")]
    [SerializeField] AudioSource vineAudio;
    
    [Header("Clips")]
    [SerializeField] AudioClip vineSnap;
    [SerializeField] List<AudioClip> vineStretchAudioClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> vineImpactAudioClips = new List<AudioClip>();


    public void playVineStretchSound() {
        // Debug.Log("VineStretchSound");
        if(vineAudio.isPlaying) return;
        if(Random.Range(0f,1f)>0.05f) return;
        AudioClip rndSound = vineStretchAudioClips[Random.Range(0,vineStretchAudioClips.Count)];
        vineAudio.PlayOneShot(rndSound);
    }

    public void playVineImpactSound() {
        // if(Random.Range(0f,1f)>0.05f) return;
        AudioClip rndSound = vineImpactAudioClips[Random.Range(0,vineImpactAudioClips.Count)];
        vineAudio.PlayOneShot(rndSound);
    }

    public void playVineSnapSound() {
        vineAudio.Stop();
        vineAudio.PlayOneShot(vineSnap);
    }

}

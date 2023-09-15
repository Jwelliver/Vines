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


    public void playVineStretchSound()
    {
        // Debug.Log("VineStretchSound");
        if (vineAudio.isPlaying) return;
        if (RNG.RandomBool()) return;
        AudioClip rndSound = RNG.RandomChoice(vineStretchAudioClips);
        vineAudio.PlayOneShot(rndSound);
    }

    public void playVineImpactSound()
    {
        // if(Random.Range(0f,1f)>0.05f) return;
        AudioClip rndSound = RNG.RandomChoice(vineImpactAudioClips);
        vineAudio.PlayOneShot(rndSound);
    }

    public void playVineSnapSound()
    {
        vineAudio.Stop();
        vineAudio.PlayOneShot(vineSnap);
    }

}

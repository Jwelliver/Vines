using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineSFX : MonoBehaviour
{

    [Header("AudioSources")]
    public AudioSource vineAudio;

    [Header("Clips")]
    [SerializeField] AudioClip vineSnap;
    [SerializeField] List<AudioClip> vineStretchAudioClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> vineImpactAudioClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> vineStressAudioClips = new List<AudioClip>();


    public void playVineStretchSound(float volume = -1)
    {
        // Debug.Log("VineStretchSound");
        if (vineAudio.isPlaying) return;
        if (RNG.RandomBool()) return;
        AudioClip rndSound = RNG.RandomChoice(vineStretchAudioClips);
        if (volume == -1)
        {
            vineAudio.PlayOneShot(rndSound);
        }
        else
        {
            vineAudio.PlayOneShot(rndSound, volume);
        }
    }

    public void playVineStressSound(float volume = -1)
    {
        AudioClip rndSound = RNG.RandomChoice(vineStressAudioClips);
        if (volume == -1)
        {
            vineAudio.PlayOneShot(rndSound);
        }
        else
        {
            vineAudio.PlayOneShot(rndSound, volume);
        }
    }

    public void playVineImpactSound(float volume = -1)
    {
        // if(Random.Range(0f,1f)>0.05f) return;
        AudioClip rndSound = RNG.RandomChoice(vineImpactAudioClips);
        if (volume == -1)
        {
            vineAudio.PlayOneShot(rndSound);
        }
        else
        {
            vineAudio.PlayOneShot(rndSound, volume);
        }
    }

    public void playVineSnapSound(float volume = -1)
    {
        vineAudio.Stop();
        if (volume == -1)
        {
            vineAudio.PlayOneShot(vineSnap);
        }
        else
        {
            vineAudio.PlayOneShot(vineSnap, volume);
        }
    }

}

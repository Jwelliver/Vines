using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SwingWindSound : MonoBehaviour
{
    [SerializeField] Rigidbody2D playerRb;
    [SerializeField] float minPitch = 0.5f;
    [SerializeField] float maxPitch = 1.5f;
    [SerializeField] float velocityAtMaxPitch = 10f;

    [SerializeField] float cutoffVelocity = 5f; // player must be greater than this velocity before wind sound can be heard.
    [SerializeField] float maxVolume = 50f;
    [SerializeField] float velocityAtMaxVolume = 10f;

    AudioSource audioSource;

    bool isPlaying;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // void StartPlaying()
    // {
    //     isPlaying = true;
    // }

    // void StopPlaying()
    // {
    //     isPlaying = false;
    //     audioSource.volume = 0;
    // }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Mathf.Abs(playerRb.velocity.magnitude) > cutoffVelocity)
        {
            if (!isPlaying) isPlaying = true;
        }
        else if (isPlaying)
        {
            isPlaying = false;
            audioSource.volume = 0;
        }

        if (isPlaying)
        {
            handleAudioVolume();
            handleAudioPitch();
        }


    }

    void handleAudioPitch()
    {
        float velocity = playerRb.velocity.magnitude;
        float pctOfMaxVelocity = velocity / velocityAtMaxPitch;
        float newPitch = ((maxPitch - minPitch) * pctOfMaxVelocity) + minPitch;
        audioSource.pitch = newPitch;
    }

    void handleAudioVolume()
    {
        float playerVelocity = Mathf.Abs(playerRb.velocity.magnitude);
        // if (playerVelocity < cutoffVelocity)
        // {
        //     audioSource.volume = 0;
        //     return;
        // }
        float pctOfMaxVelocity = playerVelocity / velocityAtMaxVolume;
        float newVolume = maxVolume * pctOfMaxVelocity;
        audioSource.volume = newVolume;
    }

}

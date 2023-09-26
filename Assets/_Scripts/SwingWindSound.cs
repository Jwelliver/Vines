using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingWindSound : MonoBehaviour
{
    [SerializeField] Rigidbody2D playerRb;
    [SerializeField] SwingingController swingingController;
    [SerializeField] float minPitch = 0.5f;
    [SerializeField] float maxPitch = 1.5f;
    [SerializeField] float velocityAtMaxPitch = 10f;

    [SerializeField] float minVelocityVolume = 5f; // player must be greater than this velocity before wind sound can be heard.
    [SerializeField] float maxVolume = 50f;
    [SerializeField] float velocityAtMaxVolume = 10f;

    AudioSource audioSource;
    bool isSwinging;
    // Start is called before the first frame update
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // handleAudioStartStop();
        // if(isSwinging) {
        handleAudioVolume();
        handleAudioPitch();
        // }
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
        float playerVelocity = playerRb.velocity.magnitude;
        if (playerVelocity < minVelocityVolume)
        {
            audioSource.volume = 0;
            return;
        };
        float pctOfMaxVelocity = playerVelocity / velocityAtMaxVolume;
        float newVolume = ((maxVolume) * pctOfMaxVelocity);
        audioSource.volume = newVolume;
    }

    void handleAudioStartStop()
    {
        if (!isSwinging && SwingingController.isSwinging)
        {
            audioSource.Play();
            isSwinging = true;
        }
        else if (isSwinging && !SwingingController.isSwinging)
        {
            audioSource.Stop();
            isSwinging = false;
        }
    }
}

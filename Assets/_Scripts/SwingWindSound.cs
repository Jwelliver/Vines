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

    AudioSource audioSource;
    bool isSwinging;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        handleAudioStartStop();
        if(isSwinging) {
            handleAudioPitch();
        }
    }

    void handleAudioPitch() {
        float velocity = playerRb.velocity.magnitude;
        float pctOfMaxVelocity = velocity/velocityAtMaxPitch;
        float newPitch = ((maxPitch-minPitch)*pctOfMaxVelocity)+minPitch;
        audioSource.pitch = newPitch;
    }

    void handleAudioStartStop() {
        if(!isSwinging && swingingController.isSwinging) {
            audioSource.Play();
            isSwinging=true;
        } else if(isSwinging && !swingingController.isSwinging) {
            audioSource.Stop();
            isSwinging=false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowSFX : MonoBehaviour
{

    [SerializeField] AudioSource arrowShootAudioSource;
    [SerializeField] AudioSource arrowHitAudioSource;
    // Start is called before the first frame update

    public void PlayArrowHitSound()
    {
        arrowShootAudioSource.Play();
    }

    public void PlayArrowShootSound()
    {
        arrowShootAudioSource.Play();
    }
}

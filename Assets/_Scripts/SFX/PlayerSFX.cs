using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSFX : MonoBehaviour
{
    [Header("AudioSources")]
    [SerializeField] AudioSource playerAudio;

    [Header("Clips")]
    [SerializeField] AudioClip jumpStart;
    [SerializeField] AudioClip jumpStop;
    [SerializeField] AudioClip playerDamage;
    [SerializeField] List<AudioClip> whoas = new List<AudioClip>();


    public void playJumpStartSound() {
        // Debug.Log("JumpStartSound");
        playerAudio.PlayOneShot(jumpStart);
    }

    public void playJumpStopSound() {
        // Debug.Log("JumpStopSound");
        playerAudio.Stop();
        playerAudio.PlayOneShot(jumpStop);
    }

    public void playWhoaSound() {
        // Debug.Log("whoaSound");
        if(playerAudio.isPlaying) return;
        AudioClip rndWhoaSound = whoas[Random.Range(0,whoas.Count)];
        playerAudio.PlayOneShot(rndWhoaSound);
    }

    public void playPlayerDamageSfx() {
        playerAudio.Stop();
        playerAudio.PlayOneShot(playerDamage);
    }

}

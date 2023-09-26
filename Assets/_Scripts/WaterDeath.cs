using System.Collections;
using UnityEngine;

public class WaterDeath : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip waterSplash;
    [SerializeField] AudioClip deadMusic;

    [SerializeField] Animator deathTextAnimator;

    [SerializeField] Rigidbody2D playerRb;
    [SerializeField] GameObject playerBody;
    [SerializeField] Transform deathHat;
    [SerializeField] Transform deathHatBuoyancyObj;
    [SerializeField] ParticleSystem waterSplashParticles;
    bool hasDied;
    // [SerializeField] Cinemachine.CinemachineVirtualCamera cam;


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && !hasDied)
        {
            handleWaterSplash();
            StartCoroutine(onDeath());
        }
    }

    IEnumerator onDeath()
    {

        // set hasDied flag; (prevents accidental multiple calls if multiple colliders hit trigger)
        hasDied = true;
        // stop player motion
        disablePlayer();

        yield return new WaitForSeconds(0.5f);
        //spawn deathHat
        spawnDeathHat();

        yield return new WaitForSeconds(1f);

        // play deathText animation
        deathTextAnimator.SetTrigger("StartDeathText");
        // play deadMusic
        audioSource.PlayOneShot(deadMusic);


    }

    void spawnDeathHat()
    {
        Vector2 spawnPos = (Vector2)deathHatBuoyancyObj.position - new Vector2(0, 3);
        GameObject.Instantiate(deathHat, spawnPos, Quaternion.identity);
    }

    void handleWaterSplash()
    {
        //play splash anim
        waterSplashParticles.Play();
        // play splash sound
        audioSource.PlayOneShot(waterSplash);
    }

    void disablePlayer()
    {
        foreach (AudioSource a in playerBody.transform.root.GetComponents<AudioSource>())
        {
            a.Stop();
        }
        // Rigidbody2D playerRb = other.transform.root.gameObject.GetComponent<Rigidbody2D>();
        playerRb.velocity = Vector2.zero;
        playerRb.gravityScale = 0;
        playerRb.bodyType = RigidbodyType2D.Static;
        // disable player sprite
        // playerBody.SetActive(false);
        playerRb.transform.root.gameObject.SetActive(false);
    }
}

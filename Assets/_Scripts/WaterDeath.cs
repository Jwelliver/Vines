using System.Collections;
using UnityEngine;

public class WaterDeath : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip waterSplash;
    [SerializeField] AudioClip deadMusic;

    // [SerializeField] Animator deathTextAnimator;

    // [SerializeField] Rigidbody2D playerRb;
    // [SerializeField] GameObject playerBody;
    [SerializeField] Transform deathHat;
    [SerializeField] Transform deathHatBuoyancyObj;
    [SerializeField] ParticleSystem waterSplashParticles;
    [SerializeField] CharacterController2D playerController;
    bool hasDied;
    // [SerializeField] Cinemachine.CinemachineVirtualCamera cam;


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && !hasDied)
        {
            // Only Trigger death if player is not swinging or is "swinging" on a detached vine.
            if (!SwingingController.isSwinging || !SwingingController.currentVineSegmentRef.vineRoot.isRootAnchored)
            {
                handleWaterSplash();
                StartCoroutine(onDeath());
            }

        }
    }

    IEnumerator onDeath()
    {

        // set hasDied flag; (prevents accidental multiple calls if multiple colliders hit trigger)
        hasDied = true;
        // stop player motion
        playerController.OnDeath();

        yield return new WaitForSeconds(0.5f);
        //spawn deathHat
        spawnDeathHat();

        yield return new WaitForSeconds(1f);

        // play deathText animation
        // deathTextAnimator.SetTrigger("StartDeathText");
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


}

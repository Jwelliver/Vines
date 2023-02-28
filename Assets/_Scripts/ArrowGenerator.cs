using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowGenerator : MonoBehaviour
{

    [SerializeField] Transform arrowProjectile;
    [SerializeField] float minTimeBetweenShots;
    [SerializeField] float maxTimeBetweenShots;
    // [SerializeField] float pctChance;

    AudioSource arrowShotAudioSource;

    void Start() {
        StartCoroutine(wait());
        arrowShotAudioSource = GetComponent<AudioSource>();
    }

    void fireArrow() {
        GameObject.Instantiate(arrowProjectile,transform.position, Quaternion.identity);
        arrowShotAudioSource.Play();
    }

    IEnumerator wait() {
        yield return new WaitForSeconds(Random.Range(minTimeBetweenShots, maxTimeBetweenShots));
        fireArrow();
        StartCoroutine(wait());
    }

    void OnDestroy() {
        StopAllCoroutines();
    }
}

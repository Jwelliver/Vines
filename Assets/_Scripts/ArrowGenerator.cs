using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowGenerator : MonoBehaviour
{

    [SerializeField] Transform arrowProjectile;
    [SerializeField] float minTimeBetweenShots;
    [SerializeField] float maxTimeBetweenShots;
    // [SerializeField] float pctChance;
    ArrowSFX sfx;


    void Start()
    {
        sfx = SfxHandler.arrowSFX;
        StartCoroutine(wait());
    }

    void fireArrow()
    {
        ArrowProjectile newArrow = GameObject.Instantiate(arrowProjectile, transform.position, Quaternion.identity).GetComponent<ArrowProjectile>();
        sfx.PlayArrowShootSound();
    }

    IEnumerator wait()
    {
        yield return new WaitForSeconds(RNG.RandomRange(minTimeBetweenShots, maxTimeBetweenShots));
        fireArrow();
        StartCoroutine(wait());
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }
}

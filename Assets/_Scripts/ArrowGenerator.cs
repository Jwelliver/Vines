using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowGenerator : MonoBehaviour
{

    [SerializeField] Transform arrowProjectile;
    [SerializeField] float minTimeBetweenShots;
    [SerializeField] float maxTimeBetweenShots;
    // [SerializeField] float pctChance;
    [SerializeField] ArrowSFX sfx;
    Transform playerRef;

    void Awake()
    {
        playerRef = GameObject.Find("GameManager").GetComponent<GameManager>().playerRef;
    }


    void Start()
    {
        StartCoroutine(wait());
    }

    void fireArrow()
    {
        ArrowProjectile newArrow = GameObject.Instantiate(arrowProjectile, transform.position, Quaternion.identity).GetComponent<ArrowProjectile>();
        newArrow.sfx = sfx;
        newArrow.playerRef = playerRef;
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

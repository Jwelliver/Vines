using UnityEngine;

public class ArrowGenerator : MonoBehaviour
{
    //TODO: This needs to utilize an object pool
    [SerializeField] Transform arrowProjectile;
    [SerializeField] MinMax<float> timeBetweenShots = new MinMax<float>(0.5f, 5f);
    [SerializeField] bool fireOnStart = false;
    private bool isFiring = false;

    void Start()
    {
        if (fireOnStart)
        {
            StartFiring(0f);
        }
    }

    public void StartFiring(float initialDelay = 5f)
    {
        isFiring = true;
        Invoke("WaitBetweenShots", initialDelay);
    }

    public void StopFiring()
    {
        isFiring = false;
        CancelInvoke("WaitBetweenShots");
    }

    void FireArrow()
    {
        Instantiate(arrowProjectile, transform.position, Quaternion.identity);
        SfxHandler.arrowSFX.PlayArrowShootSound();
    }

    void WaitBetweenShots()
    {
        if (!isFiring)
        {
            CancelInvoke("WaitBetweenShots");
            return;
        }
        FireArrow();
        Invoke("WaitBetweenShots", RNG.RandomRange(timeBetweenShots));
    }

    void OnDestroy()
    {
        CancelInvoke("WaitBetweenShots");
    }
}

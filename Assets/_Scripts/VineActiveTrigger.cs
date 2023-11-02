using UnityEngine;

public class VineActiveTrigger : MonoBehaviour
{
    public int secondsBeforeResuspendAfterExit = 5;
    VineSuspenseManager tempVineSuspenseMgr;
    Rigidbody2D playerRb;

    void Start()
    {
        playerRb = GameManager.GetPlayerRef().GetComponent<Rigidbody2D>();
    }


    void FixedUpdate()
    {
        //TODO: This is a temp fix to ensure the ActiveTrigger collider does not rotate with the player due to parenting; Would prefer to attach use a method outside of an update method
        transform.position = playerRb.position;
    }


    void OnTriggerEnter2D(Collider2D otherCol)
    {
        // Debug.Log("VineActiveTrigger Enter");
        // otherCol.attachedRigidbody.WakeUp();
        if (otherCol.TryGetComponent<VineSuspenseManager>(out tempVineSuspenseMgr))
        {
            tempVineSuspenseMgr.UnsuspendAfterSeconds(0f);
            // Debug.Log("VineActiveTrigger: Unsuspend: " + otherCol.name);
        }
    }

    void OnTriggerExit2D(Collider2D otherCol)
    {
        // Debug.Log("VineActiveTrigger Exit");
        // otherCol.attachedRigidbody.Sleep();
        if (otherCol.TryGetComponent<VineSuspenseManager>(out tempVineSuspenseMgr))
        {
            tempVineSuspenseMgr.SuspendAfterSeconds(secondsBeforeResuspendAfterExit);
            // Debug.Log("VineActiveTrigger: Suspend: " + otherCol.name);
        }
    }
}

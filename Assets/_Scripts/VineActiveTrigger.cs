using UnityEngine;

public class VineActiveTrigger : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D otherCol)
    {
        Debug.Log("VineActiveTrigger Enter");
        otherCol.attachedRigidbody.WakeUp();
    }

    void OnTriggerExit2D(Collider2D otherCol)
    {
        otherCol.attachedRigidbody.Sleep();
    }
}

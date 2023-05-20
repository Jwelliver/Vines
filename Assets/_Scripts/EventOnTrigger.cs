using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Collider2dEvent: UnityEvent<Collider2D> {}

public class EventOnTrigger : MonoBehaviour
{
    public Collider2dEvent onTriggerEnter = new Collider2dEvent();
    
    void OnTriggerEnter2D(Collider2D col) {
        Debug.Log("OnTriggerEnter2D: "+ col.name);
        onTriggerEnter.Invoke(col);
    }
}

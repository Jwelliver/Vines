using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Net : MonoBehaviour
{
    [SerializeField] EventOnTrigger entryCollider;
    [SerializeField] EventOnTrigger capturePoint;
    [SerializeField] LayerMask capturable;

    private Collider2D objInEntry;
    // private Collider2D objInCapturePoint;
    private Transform capturedObj;


    void OnEnable() {
        reset();
    }

    void reset() {
        objInEntry = null;
        clearCapturedObjects();
        // objInCapturePoint = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        entryCollider.onTriggerEnter.AddListener(onEntryPointCollider);
        capturePoint.onTriggerEnter.AddListener(onCapturePointCollider);
    }

    void onEntryPointCollider(Collider2D col) {
        objInEntry = col;
    }

    void onCapturePointCollider(Collider2D col) {
        if(col==objInEntry) {
            captureObj(col);
        }
    }

    void captureObj(Collider2D col) {
        //TODO: Call bug script.onCaptured();
        capturedObj = col.transform;
        Transform capturePointTransform = capturePoint.transform;
        capturedObj.position = capturePointTransform.position;
        capturedObj.SetParent(capturePointTransform);
    }

    void clearCapturedObjects() {
        if(capturedObj!=null) {
            Destroy(capturedObj.gameObject);
            capturedObj = null;
        }
    }
}

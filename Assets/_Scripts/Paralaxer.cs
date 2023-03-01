using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
Place on a paralax backround parent to control the paralax movement
*/

public class Paralaxer : MonoBehaviour
{
    [SerializeField] Vector3 axisSpeed = new Vector3(0.5f,0,0); //speed for each axis to move paralax field
    [SerializeField] Transform referenceObj; //Object to for paralaxing against

    Vector3 prevReferencePosition;

    
    void Start() {
        prevReferencePosition = referenceObj.position;
    }

    void LateUpdate()
    {
        performParalax();
    }

    void performParalax() {
        Vector3 refObjCurPos = referenceObj.position;
        Vector3 refObjPosDiff = prevReferencePosition-refObjCurPos;
        float newX = refObjPosDiff.x * axisSpeed.x;
        float newY = refObjPosDiff.y * axisSpeed.y;
        float newZ = refObjPosDiff.z * axisSpeed.z;
        Vector3 newMovement = new Vector3(newX,newY,newZ) * Time.deltaTime;
        transform.position += newMovement;
        prevReferencePosition = refObjCurPos;
    }
}

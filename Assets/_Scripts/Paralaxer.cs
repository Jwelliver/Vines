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

    Transform myTransform;

    Vector3 prevReferencePosition;
    Vector3 refObjCurPos;
    Vector3 refObjPosDiff;
    Vector3 newMovement;

    void Awake() {
        myTransform = transform;
    }

    void Start() {
        prevReferencePosition = referenceObj.position;
    }

    void LateUpdate()
    {
        performParalax();
    }

    void performParalax() {
        refObjCurPos = referenceObj.position;
        refObjPosDiff = prevReferencePosition-refObjCurPos;
        float newX = refObjPosDiff.x * axisSpeed.x;
        float newY = refObjPosDiff.y * axisSpeed.y;
        float newZ = refObjPosDiff.z * axisSpeed.z;
        newMovement = new Vector3(newX,newY,newZ) * Time.deltaTime;
        myTransform.position += newMovement;
        prevReferencePosition = refObjCurPos;
    }
}

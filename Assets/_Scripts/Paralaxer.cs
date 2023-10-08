using System;
using UnityEngine;


/*
Place on a paralax backround parent to control the paralax movement
*/


public class Paralaxer : MonoBehaviour
{
    // [SerializeField] Transform cam;
    // [SerializeField] Transform referenceObj; //Object to for paralaxing against

    Camera cam;
    Transform referenceObj; //Object to paralax against ()
    Vector2 startPos;
    float clippingPlane;
    float parallaxFactor;

    Transform myTransform;
    Transform camTransform;

    Vector2 travel => (Vector2)camTransform.position - startPos;

    void Start()
    {
        myTransform = transform;
        cam = GameManager.cameraRef;
        camTransform = cam.transform;
        referenceObj = GameManager.playerRef;
        // prevReferencePosition = referenceObj.position;
        startPos = myTransform.position;
        float zDistanceFromReferenceObj = myTransform.position.z - referenceObj.position.z;
        clippingPlane = camTransform.position.z + (zDistanceFromReferenceObj > 0 ? cam.farClipPlane : cam.nearClipPlane);
        parallaxFactor = MathF.Abs(zDistanceFromReferenceObj) / clippingPlane;
    }

    void FixedUpdate() //TODO: test if this works fine compared to update
    {
        Vector2 newPos = travel * parallaxFactor + startPos;
        myTransform.position = new Vector3(newPos.x, myTransform.position.y, myTransform.position.z);
    }
}


//=========091623 OLD method:

// public class Paralaxer : MonoBehaviour
// {
//     [SerializeField] Vector3 axisSpeed = new Vector3(0.5f, 0, 0); //speed for each axis to move paralax field
//     [SerializeField] Transform referenceObj; //Object to for paralaxing against

//     Transform myTransform;

//     Vector3 prevReferencePosition;
//     Vector3 refObjCurPos;
//     Vector3 refObjPosDiff;
//     Vector3 newMovement;

//     void Awake()
//     {
//         myTransform = transform;
//     }

//     void Start()
//     {
//         prevReferencePosition = referenceObj.position;
//         // Debug.Log("Paralaxer: " + name);
//     }

//     void LateUpdate()
//     {
//         performParalax();
//     }

//     void performParalax()
//     {
//         refObjCurPos = referenceObj.position;
//         refObjPosDiff = prevReferencePosition - refObjCurPos;
//         float newX = refObjPosDiff.x * axisSpeed.x;
//         float newY = axisSpeed.y == 0 ? 0 : refObjPosDiff.y * axisSpeed.y;
//         float newZ = axisSpeed.z == 0 ? 0 : refObjPosDiff.z * axisSpeed.z;
//         newMovement = new Vector3(newX, newY, newZ) * Time.deltaTime;
//         myTransform.position += newMovement;
//         prevReferencePosition = refObjCurPos;
//     }
// }

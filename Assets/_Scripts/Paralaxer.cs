using System;
using UnityEngine;


/*
Place on a paralax backround parent to control the paralax movement
*/


public class Paralaxer : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] Transform referenceObj; //Object to for paralaxing against

    Vector2 startPos;
    float clippingPlane;
    float parallaxFactor;

    Vector2 travel => (Vector2)cam.transform.position - startPos;

    void Start()
    {
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        cam = gameManager.cameraRef;
        referenceObj = gameManager.playerRef;
        // prevReferencePosition = referenceObj.position;
        startPos = transform.position;
        float zDistanceFromReferenceObj = transform.position.z - referenceObj.position.z;
        clippingPlane = cam.transform.position.z + (zDistanceFromReferenceObj > 0 ? cam.farClipPlane : cam.nearClipPlane);
        parallaxFactor = MathF.Abs(zDistanceFromReferenceObj) / clippingPlane;
    }

    void Update()
    {
        Vector2 newPos = startPos + travel * parallaxFactor;
        transform.position = new Vector3(newPos.x, transform.position.y, transform.position.z);
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

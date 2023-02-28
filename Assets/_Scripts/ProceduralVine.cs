using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralVine : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] Transform vineAnchor;
    
    [SerializeField] List<Transform> vineSegements; //list of possible vine segments
    [SerializeField] List<Transform> adornments;
    // private Transform vineSegment;

    [Header("Length")]
    [SerializeField] int minLength = 5;
    [SerializeField] int maxLength = 20;
    [SerializeField] float segmentOffset = 0f;

    [Header("Break Forces")]
    [SerializeField] float minNormalBreakForce = 1000f;
    [SerializeField] float maxNormalBreakForce = 5000f;
    
    [SerializeField] float minWeakBreakForce = 200f;
    [SerializeField] float maxWeakBreakForce = 500f;

    [SerializeField] float pctChanceWeak = 0.03f;
    [SerializeField] int weakStartSegment = 4; // weak segments will only appear after this number of segments has been created.

    [Header("Appearance")]
    [SerializeField] float pctChanceCurl = 0.3f;
    [SerializeField] float maxCurlForce = 10f;
    [SerializeField] float curlSpeed = 10f;
    [SerializeField] float pctChanceAdornment = 0.1f;
    [SerializeField] float minAdornmentScale = 0.5f;
    [SerializeField] float maxAdornmentScale = 1f;

    [Header("Colors")]
    [SerializeField] Color minWeakColor = new Color(125,110,88);
    [SerializeField] Color maxWeakColor = new Color(125,145,88);

    [Header("Audio")] //Note: vine stretch audio is currently handled in Play CharacterController script
    [SerializeField] AudioClip vineSnap;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        // vineSegment = vineSegments[Random.Range(0,vineSegments.Count)];
        createVine();
    }


    /*
        TODO: new vine weak
            - determine if vine is weak at start
            - if it is weak, set all segments and adornments to weak color
            - after generating segments, pick a random segment and override breakforce to random weak value
    */

void createVine() {
        int length = Random.Range(minLength,maxLength);
        Transform prevSegment;
        //instantiate anchor at 0,0
        prevSegment = GameObject.Instantiate(vineAnchor,transform.position,transform.rotation);
        prevSegment.SetParent(transform);
        bool isWeak = Random.Range(0f,1f)<pctChanceWeak;
        //for i in length:
        for(int i=0; i<length;i++) {
            // -  instantiate vineSegment at prev segment position - segmentLength*2;
            Transform rndSegment = vineSegements[Random.Range(0,vineSegements.Count)];
            Vector2 newPosition = (Vector2)prevSegment.position - new Vector2(0,Mathf.Abs(rndSegment.localScale.y*2)+segmentOffset);
            Transform newSegment = GameObject.Instantiate(rndSegment, newPosition, Quaternion.identity);
            newSegment.SetParent(transform);
            // newSegment.GetComponent<SpriteRenderer>().sprite = vineSprites[Random.Range(0,vineSprites.Count)];
            //  - set hinge connected rb to previous segment; 
            HingeJoint2D newHinge = newSegment.GetComponent<HingeJoint2D>();
            newHinge.connectedBody = prevSegment.GetComponent<Rigidbody2D>();
            //  - set curl
            bool hasCurl = Random.Range(0f,1f) < pctChanceCurl;
            if(hasCurl) {
                JointMotor2D motor = new JointMotor2D();
                float motorForce = Random.Range(0,maxCurlForce);
                float motorSpeed =Random.Range(-curlSpeed,curlSpeed);
                motor.motorSpeed = motorSpeed;
                motor.maxMotorTorque = motorForce;
                newHinge.motor = motor;
                newHinge.useMotor = true;
            }

            if(isWeak)
                newSegment.GetComponentInChildren<SpriteRenderer>().color = maxWeakColor;//Color.Lerp(minWeakColor, maxWeakColor, howWeak);
   
            newHinge.breakForce = Random.Range(minNormalBreakForce,maxNormalBreakForce);

            bool hasAdornment = Random.Range(0f,1f)<pctChanceAdornment;
            if(hasAdornment) {
                Transform rndAdornment = adornments[Random.Range(0,adornments.Count)];
                Transform newAdornment = GameObject.Instantiate(rndAdornment, newSegment.position, Quaternion.identity);
                float rndScale = Random.Range(minAdornmentScale,maxAdornmentScale);
                newAdornment.localScale = new Vector2(rndScale,rndScale);
                newAdornment.SetParent(newSegment);
                if(isWeak)
                    newAdornment.GetComponentInChildren<SpriteRenderer>().color = maxWeakColor;
            }

            // - update prevSegment
            prevSegment = newSegment;
        }

        if(isWeak) {
            //pick one segment at random and reset breakforce to random weak breakforce
            int numWeakSegments = (int)length/4;
            for(int i=0;i<numWeakSegments;i++) {
                HingeJoint2D[] allSegments = gameObject.GetComponentsInChildren<HingeJoint2D>();
                HingeJoint2D rndSegment = allSegments[Random.Range(0,allSegments.Length)];
                float rndWeakBreakForce = Random.Range(minWeakBreakForce,maxWeakBreakForce);
                rndSegment.breakForce = rndWeakBreakForce;
            }
        }
    }



    // ========== ORIG =============
    // void createVine() {
    //     int length = Random.Range(minLength,maxLength);
    //     Transform prevSegment;
    //     //instantiate anchor at 0,0
    //     prevSegment = GameObject.Instantiate(vineAnchor,transform.position,transform.rotation);
    //     prevSegment.SetParent(transform);
    //     // bool isWeak = Random.Range(0f,1f)<pctChanceWeak;
    //     //for i in length:
    //     for(int i=0; i<length;i++) {
    //         // -  instantiate vineSegment at prev segment position - segmentLength*2;
    //         Transform rndSegment = vineSegements[Random.Range(0,vineSegements.Count)];
    //         Vector2 newPosition = (Vector2)prevSegment.position - new Vector2(0,Mathf.Abs(rndSegment.localScale.y*2)+segmentOffset);
    //         Transform newSegment = GameObject.Instantiate(rndSegment, newPosition, Quaternion.identity);
    //         newSegment.SetParent(transform);
    //         // newSegment.GetComponent<SpriteRenderer>().sprite = vineSprites[Random.Range(0,vineSprites.Count)];
    //         //  - set hinge connected rb to previous segment; 
    //         HingeJoint2D newHinge = newSegment.GetComponent<HingeJoint2D>();
    //         newHinge.connectedBody = prevSegment.GetComponent<Rigidbody2D>();
    //         //  - set curl
    //         bool hasCurl = Random.Range(0f,1f) < pctChanceCurl;
    //         if(hasCurl) {
    //             JointMotor2D motor = new JointMotor2D();
    //             float motorForce = Random.Range(0,maxCurlForce);
    //             float motorSpeed =Random.Range(-curlSpeed,curlSpeed);
    //             motor.motorSpeed = motorSpeed;
    //             motor.maxMotorTorque = motorForce;
    //             newHinge.motor = motor;
    //             newHinge.useMotor = true;
    //         }
   
    //         if(i>weakStartSegment) {
    //             //  - set hinge breakforce
    //             bool isWeak = Random.Range(0f,1f)<pctChanceWeak;
    //             float breakForce = (isWeak) ? Random.Range(minWeakBreakForce,maxWeakBreakForce) : Random.Range(minNormalBreakForce,maxNormalBreakForce);
    //             newHinge.breakForce = breakForce;
    //             if(isWeak) {
    //                 float howWeak = minWeakBreakForce - breakForce; // diff between weakest possible breakforce and actual breakforce; Used for lerping weakness color below.
    //                 newSegment.GetComponentInChildren<SpriteRenderer>().color = Color.Lerp(minWeakColor, maxWeakColor, howWeak);
    //             }
    //         } else {
    //             newHinge.breakForce = Random.Range(minNormalBreakForce,maxNormalBreakForce);
    //         }
    //         // newHinge.breakForce = Random.Range(minNormalBreakForce,maxNormalBreakForce);

    //         bool hasAdornment = Random.Range(0f,1f)<pctChanceAdornment;
    //         if(hasAdornment) {
    //             Transform rndAdornment = adornments[Random.Range(0,adornments.Count)];
    //             Transform newAdornment = GameObject.Instantiate(rndAdornment, newSegment.position, Quaternion.identity);
    //             float rndScale = Random.Range(minAdornmentScale,maxAdornmentScale);
    //             newAdornment.localScale = new Vector2(rndScale,rndScale);
    //             newAdornment.SetParent(newSegment);
    //         }

    //         // - update prevSegment
    //         prevSegment = newSegment;
    //     }

    //     // if(isWeak) {
    //     //     //pick one segment at random and reset breakforce to random weak breakforce
    //     //     float howWeak = minWeakBreakForce - breakForce; // diff between weakest possible breakforce and actual breakforce; Used for lerping weakness color below.
    //     //     newSegment.GetComponentInChildren<SpriteRenderer>().color = Color.Lerp(minWeakColor, maxWeakColor, howWeak);
    //     // }

    // }

    public void playVineSnapSound() {
        Debug.Log("VINE SNAP");
        audioSource.PlayOneShot(vineSnap);
    }

}

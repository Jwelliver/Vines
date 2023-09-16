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

    [Header("Appearance")]
    [SerializeField] float pctChanceCurl = 0.3f;
    [SerializeField] float maxCurlForce = 10f;
    [SerializeField] float curlSpeed = 10f;
    [SerializeField] float pctChanceAdornment = 0.1f;
    [SerializeField] float minAdornmentScale = 0.5f;
    [SerializeField] float maxAdornmentScale = 1f;

    [Header("Colors")]
    [SerializeField] Color minWeakColor = new Color(125, 110, 88);
    [SerializeField] Color maxWeakColor = new Color(125, 145, 88);

    public SfxHandler sfx;

    // private LineRenderer lineRenderer;

    void Start()
    {
        try
        {
            sfx = GameObject.Find("SFX").GetComponent<SfxHandler>();
        }
        catch
        {

        }

        // vineSegment = vineSegments[Random.Range(0,vineSegments.Count)];
        createVine();
    }

    // void LateUpdate() {
    //     // lineRenderer
    // }

    void createVine()
    {
        Transform prevSegment;
        //instantiate anchor
        prevSegment = GameObject.Instantiate(vineAnchor, transform.position, transform.rotation, transform);

        bool isWeak = RNG.SampleProbability(pctChanceWeak);
        int length = RNG.RandomRange(minLength, maxLength);
        //for i in length:
        for (int i = 0; i < length; i++)
        {
            // -  instantiate vineSegment at prev segment position - segmentLength*2;
            Transform rndSegment = RNG.RandomChoice(vineSegements);//vineSegements[RNG.RandomRange(0, vineSegements.Count)];
            // Vector2 newPosition = (Vector2)prevSegment.position - new Vector2(0, Mathf.Abs(rndSegment.localScale.y * 2) + segmentOffset); //ORIG
            float colliderSizeY = rndSegment.GetComponent<CapsuleCollider2D>().size.y;
            // float connectedAnchorPosY = rndSegment.GetComponent<HingeJoint2D>().connectedAnchor.y;
            float positionYOffset = -colliderSizeY;
            Vector2 newPosition = (Vector2)prevSegment.position + new Vector2(0, positionYOffset);
            Transform newSegment = GameObject.Instantiate(rndSegment, newPosition, Quaternion.identity, transform);
            //  - set hinge connected rb to previous segment; 
            HingeJoint2D newHinge = newSegment.GetComponent<HingeJoint2D>();
            newHinge.connectedBody = prevSegment.GetComponent<Rigidbody2D>();
            //  - set curl
            bool hasCurl = RNG.SampleProbability(pctChanceCurl);
            if (hasCurl)
            {
                JointMotor2D motor = new JointMotor2D();
                float motorForce = RNG.RandomRange(0, maxCurlForce);
                float motorSpeed = RNG.RandomRange(-curlSpeed, curlSpeed);
                motor.motorSpeed = motorSpeed;
                motor.maxMotorTorque = motorForce;
                newHinge.motor = motor;
                newHinge.useMotor = true;
            }

            if (isWeak)
                newSegment.GetComponentInChildren<SpriteRenderer>().color = maxWeakColor;//Color.Lerp(minWeakColor, maxWeakColor, howWeak);

            newHinge.breakForce = RNG.RandomRange(minNormalBreakForce, maxNormalBreakForce);

            bool hasAdornment = RNG.SampleProbability(pctChanceAdornment);
            if (hasAdornment)
            {
                Transform rndAdornment = RNG.RandomChoice(adornments);
                Transform newAdornment = GameObject.Instantiate(rndAdornment, newSegment.position, Quaternion.identity);
                float rndScale = RNG.RandomRange(minAdornmentScale, maxAdornmentScale);
                newAdornment.localScale = new Vector2(rndScale, rndScale);
                newAdornment.SetParent(newSegment);
                if (isWeak)
                    newAdornment.GetComponentInChildren<SpriteRenderer>().color = maxWeakColor;
            }

            // - update prevSegment
            prevSegment = newSegment;
        }

        if (isWeak)
        {
            //pick one segment at random and reset breakforce to random weak breakforce
            int numWeakSegments = (int)length / 4;
            for (int i = 0; i < numWeakSegments; i++)
            {
                HingeJoint2D[] allSegments = gameObject.GetComponentsInChildren<HingeJoint2D>();
                HingeJoint2D rndSegment = RNG.RandomChoice(allSegments);
                float rndWeakBreakForce = RNG.RandomRange(minWeakBreakForce, maxWeakBreakForce);
                rndSegment.breakForce = rndWeakBreakForce;
            }
        }
    }
}

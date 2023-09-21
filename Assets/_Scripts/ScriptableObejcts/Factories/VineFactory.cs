using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
092123
TODO: Refactor:
    - implement VineConfig and VineSegmentConfig
    - extract/refactor generateVine
    - Add methods to place single vine of desired type
*/

[Serializable]
public class VineFactoryConfig
{

    [Header("Length")]
    public MinMax<int> length = new MinMax<int>(15, 25);
    public float segmentLength = 0.7f;

    [Header("Break Forces")]
    public MinMax<float> normalBreakForce = new MinMax<float>(1500f, 2500f);
    public MinMax<float> weakBreakForce = new MinMax<float>(250f, 500f);
    public float pctChanceWeak = 0.08f;

    [Header("Curl")]
    public float pctChanceCurl = 0.3f;
    public float maxCurlForce = 10f;
    public float curlSpeed = 10f;

    [Header("Adornments")]
    public float pctChanceAdornment = 0.7f;
    public MinMax<float> adornmentScale = new MinMax<float>(0.3f, 0.8f);

    [Header("Segment Colors")]
    public Color normalSegmentColor = new Color(1, 1, 1, 1);
    public Color weakSegmentColor = new Color(0.764151f, 0.50165635f, 0f, 1);

}

// public class VineConfig
// {
//     public int vineLength;
// }

// public class VineSegmentConfig
// {

// }



public class VineFactory : ScriptableObject
{

    [SerializeField] VineFactoryConfig defaultFactoryConfig = new VineFactoryConfig();
    [SerializeField] Transform vineRootPrefab;
    [SerializeField] List<Transform> vineSegmentPrefabs;
    [SerializeField] List<Transform> adornmentPrefabs;

    // [SerializeField] VineAdornmentFactory vineAdornmentFactory; //TODO: implement adornmentFactory with colors

    public void GenerateVine(Vector2 position, Transform containerParent, VineFactoryConfig factoryConfigOverride = null)
    {
        VineFactoryConfig factoryConfig = factoryConfigOverride ?? defaultFactoryConfig;

        // Instantiate VineRoot
        Transform vineRoot = GameObject.Instantiate(vineRootPrefab, position, Quaternion.identity, containerParent);

        //instantiate anchor
        Transform prevSegment;
        prevSegment = GameObject.Instantiate(vineSegmentPrefabs[0], position, Quaternion.identity, vineRoot);
        prevSegment.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        prevSegment.GetComponent<HingeJoint2D>().enabled = false;

        bool isWeak = RNG.SampleProbability(factoryConfig.pctChanceWeak);
        int vineLength = RNG.RandomRange(factoryConfig.length.min, factoryConfig.length.max);

        float segLength = factoryConfig.segmentLength;
        // Set up each segment
        for (int i = 0; i < vineLength; i++)
        {
            // Get Random Vine segment prefab
            Transform rndSegment = RNG.RandomChoice(vineSegmentPrefabs);
            // instantiate new segment at previous segment minus segmentLength Y;
            Vector2 newPosition = (Vector2)prevSegment.position + new Vector2(0, -segLength);
            Transform newSegment = GameObject.Instantiate(rndSegment, newPosition, Quaternion.identity, vineRoot);
            // Set up collider size
            CapsuleCollider2D segCollider = newSegment.GetComponent<CapsuleCollider2D>();
            segCollider.size = new Vector2(segCollider.size.x, segLength);
            // Setup hinge; configure anchor points and set previous segment as the connected rb; 
            HingeJoint2D newHinge = newSegment.GetComponent<HingeJoint2D>();
            float anchorYOffset = segLength * 0.4f;
            newHinge.anchor = new Vector2(0f, anchorYOffset);
            newHinge.connectedAnchor = new Vector2(0f, -anchorYOffset);
            newHinge.connectedBody = prevSegment.GetComponent<Rigidbody2D>();
            // Setup sprite to match segLength
            SpriteRenderer spriteRenderer = newSegment.GetComponent<SpriteRenderer>();
            spriteRenderer.size = new Vector2(spriteRenderer.size.x, segLength);
            //  - set curl
            bool hasCurl = RNG.SampleProbability(factoryConfig.pctChanceCurl);
            if (hasCurl)
            {
                JointMotor2D motor = new JointMotor2D();
                float motorForce = RNG.RandomRange(0, factoryConfig.maxCurlForce);
                float motorSpeed = RNG.RandomRange(-factoryConfig.curlSpeed, factoryConfig.curlSpeed);
                motor.motorSpeed = motorSpeed;
                motor.maxMotorTorque = motorForce;
                newHinge.motor = motor;
                newHinge.useMotor = true;
            }

            if (isWeak)
                newSegment.GetComponentInChildren<SpriteRenderer>().color = factoryConfig.weakSegmentColor;//Color.Lerp(minWeakColor, maxWeakColor, howWeak);
            else
                newSegment.GetComponentInChildren<SpriteRenderer>().color = factoryConfig.normalSegmentColor;

            newHinge.breakForce = RNG.RandomRange(factoryConfig.normalBreakForce.min, factoryConfig.normalBreakForce.max);

            bool hasAdornment = RNG.SampleProbability(factoryConfig.pctChanceAdornment);
            if (hasAdornment)
            {
                Transform rndAdornment = RNG.RandomChoice(adornmentPrefabs);
                float randomRotation = RNG.RandomRange(0, 359);
                Transform newAdornment = GameObject.Instantiate(rndAdornment, newSegment.position, Quaternion.Euler(0, 0, randomRotation));
                float rndScale = RNG.RandomRange(factoryConfig.adornmentScale.min, factoryConfig.adornmentScale.max);
                newAdornment.localScale = new Vector2(rndScale, rndScale);
                newAdornment.SetParent(newSegment);
                if (isWeak)
                    newAdornment.GetComponentInChildren<SpriteRenderer>().color = factoryConfig.weakSegmentColor;
            }

            // - update prevSegment
            prevSegment = newSegment;
        }

        if (isWeak)
        {
            //pick one segment at random and reset breakforce to random weak breakforce
            int numWeakSegments = (int)vineLength / 4;
            for (int i = 0; i < numWeakSegments; i++)
            {
                HingeJoint2D[] allSegments = vineRoot.gameObject.GetComponentsInChildren<HingeJoint2D>();
                HingeJoint2D rndSegment = RNG.RandomChoice(allSegments);
                float rndWeakBreakForce = RNG.RandomRange(factoryConfig.weakBreakForce.min, factoryConfig.weakBreakForce.max);
                rndSegment.breakForce = rndWeakBreakForce;
            }
        }
    }
}

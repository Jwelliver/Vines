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

    [Header("Vine Length")]
    public MinMax<int> length = new MinMax<int>(15, 25);

    [Header("Segment size")]
    public float segmentLength = 0.7f;
    public float segmentWidth = 0.35f;

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


    public VineFactoryConfig Copy()
    {
        return new VineFactoryConfig
        {
            length = length.Copy(),
            segmentLength = segmentLength,
            normalBreakForce = normalBreakForce.Copy(),
            weakBreakForce = weakBreakForce.Copy(),
            pctChanceWeak = pctChanceWeak,
            pctChanceCurl = pctChanceCurl,
            maxCurlForce = maxCurlForce,
            curlSpeed = curlSpeed,
            pctChanceAdornment = pctChanceAdornment,
            adornmentScale = adornmentScale,
            normalSegmentColor = normalSegmentColor,
            weakSegmentColor = weakSegmentColor
        };
    }
}

// public class VineConfig
// {
//     public int vineLength;
// }

// public class VineSegmentConfig
// {

// }

/*
    ? implement vine width
    ? use width to influence avg strength
    ? use health to influence avg strength and color (weakColor + (normalcolor-weakcolor)*strength as pct of max strength)
    ? set weight of segment based on width
*/

/*
TODO:
    - Add Segment sprites list; choose random sprite here;
    - Try to set the snap particle reference in the vine root; then on break, the vine root can instantiate the particles at the position of the broken joint
    - Refactor VineSegment script; Might not want to remove completely as it still may be useful for other functionality
        ... but def want to remove references to sprites;

    - On VineRoot; Refactor "ProceduralVine" Script
        ... maybe just set it to a new VineRoot script
*/

[CreateAssetMenu(menuName = "MyAssets/ScriptableObjects/Factories/VineFactory")]
public class VineFactory : ScriptableObject
{

    [SerializeField] VineFactoryConfig defaultFactoryConfig = new VineFactoryConfig();
    [SerializeField] Transform vineRootPrefab;
    [SerializeField] List<Transform> vineSegmentPrefabs;
    [SerializeField] VineAdornmentFactory vineAdornmentFactory;

    public void SetDefaultFactoryConfig(VineFactoryConfig newDefaultFactoryConfig)
    {
        defaultFactoryConfig = null;
        defaultFactoryConfig = newDefaultFactoryConfig;
    }

    VineFactoryConfig CheckIfInVineOverrideZone(Vector2 position, VineFactoryConfig currentConfig)
    {
        VineOverrideZone[] vineOverrideZones = GameObject.FindObjectsOfType<VineOverrideZone>();
        foreach (VineOverrideZone zone in vineOverrideZones)
        {
            VineFactoryConfig configOverride = zone.QueryBounds(position, currentConfig);
            if (configOverride != null)
            {
                return configOverride;
            }
        }
        return null;
    }

    public Transform GenerateVine(Vector2 position, Transform containerParent, VineFactoryConfig factoryConfigOverride = null)
    {
        // Setup FactoryConfig; First use provided override or defaultConfig; Then Check for a Zone override which has priority.
        VineFactoryConfig factoryConfig = factoryConfigOverride ?? defaultFactoryConfig;
        VineFactoryConfig zoneOverride = CheckIfInVineOverrideZone(position, factoryConfig);
        factoryConfig = zoneOverride ?? factoryConfig;

        // Init Vine properties
        bool isWeak = RNG.SampleProbability(factoryConfig.pctChanceWeak);
        int vineLength = RNG.RandomRange(factoryConfig.length.min, factoryConfig.length.max);
        float segLength = factoryConfig.segmentLength;
        Color vineColor = isWeak ? factoryConfig.weakSegmentColor : factoryConfig.normalSegmentColor;

        // Track segments to apply to vineroot/VineLineRenderer; Adding 1 to vineLength to account for anchor segment;
        VineSegment[] vineSegments = new VineSegment[vineLength + 1];

        // Instantiate VineRoot
        Transform vineRoot = GameObject.Instantiate(vineRootPrefab, position, Quaternion.identity, containerParent);
        // vineRoot.name = "VineRoot";

        //instantiate anchor segment
        Transform prevSegment;
        prevSegment = GameObject.Instantiate(vineSegmentPrefabs[0], position, Quaternion.identity, vineRoot);
        prevSegment.name = "0 (AnchorSegment)";
        prevSegment.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        prevSegment.GetComponent<HingeJoint2D>().enabled = false;
        vineSegments[0] = prevSegment.GetComponent<VineSegment>();

        // Set up anchor collider size
        CapsuleCollider2D anchorCollider = prevSegment.GetComponent<CapsuleCollider2D>();
        anchorCollider.size = new Vector2(factoryConfig.segmentWidth, segLength);

        // Get LineRenderer and setup
        LineRenderer vineLineRenderer = vineRoot.GetComponent<LineRenderer>();
        vineLineRenderer.startWidth = factoryConfig.segmentWidth;
        vineLineRenderer.endWidth = factoryConfig.segmentWidth;
        vineLineRenderer.startColor = vineColor;
        vineLineRenderer.endColor = vineColor;

        // Set up each segment
        for (int i = 0; i < vineLength; i++)
        {
            // Get Random Vine segment prefab
            Transform rndSegment = RNG.RandomChoice(vineSegmentPrefabs);
            // instantiate new segment at previous segment minus segmentLength Y;
            //TODO: try using position offset of one; and also try not parenting until after segment is setup.
            Vector2 newPosition = (Vector2)prevSegment.position + new Vector2(0, -segLength);
            Transform newSegment = GameObject.Instantiate(rndSegment, newPosition, Quaternion.identity, vineRoot);
            // Set segment name
            newSegment.name = (i + 1).ToString();
            // Set up collider size
            CapsuleCollider2D segCollider = newSegment.GetComponent<CapsuleCollider2D>();
            segCollider.size = new Vector2(factoryConfig.segmentWidth, segLength);
            // Setup hinge; configure anchor points and set previous segment as the connected rb; 
            HingeJoint2D newHinge = newSegment.GetComponent<HingeJoint2D>();
            float anchorYOffset = segLength * 0.4f;
            newHinge.anchor = new Vector2(0f, anchorYOffset);
            newHinge.connectedAnchor = new Vector2(0f, -anchorYOffset);
            newHinge.connectedBody = prevSegment.GetComponent<Rigidbody2D>();

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

            newHinge.breakForce = RNG.RandomRange(factoryConfig.normalBreakForce.min, factoryConfig.normalBreakForce.max);

            // Add Adornment at Random
            bool hasAdornment = RNG.SampleProbability(factoryConfig.pctChanceAdornment);
            if (hasAdornment)
            {
                Transform newAdornment = vineAdornmentFactory.GenerateVineAdornment(newSegment.position, newSegment, factoryConfig.adornmentScale);
                if (isWeak)
                    newAdornment.GetComponentInChildren<SpriteRenderer>().color = vineColor;
            }

            // Add Segment to segments list
            // vineSegments.Add(newSegment.GetComponent<VineSegment>());
            vineSegments[i + 1] = newSegment.GetComponent<VineSegment>();
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

        // Set segments in VineLineRenderer
        vineRoot.GetComponent<VineRoot>().Init(vineSegments, true, true);

        return vineRoot;
    }
}


//=========== ORIG GenerateVine (works with sprite approach)
//    public Transform GenerateVine(Vector2 position, Transform containerParent, VineFactoryConfig factoryConfigOverride = null)
//     {
//         // Setup FactoryConfig; First use provided override or defaultConfig; Then Check for a Zone override which has priority.
//         VineFactoryConfig factoryConfig = factoryConfigOverride ?? defaultFactoryConfig;
//         VineFactoryConfig zoneOverride = CheckIfInVineOverrideZone(position, factoryConfig);
//         factoryConfig = zoneOverride ?? factoryConfig;

//         // Instantiate VineRoot
//         Transform vineRoot = GameObject.Instantiate(vineRootPrefab, position, Quaternion.identity, containerParent);

//         //instantiate anchor
//         Transform prevSegment;
//         prevSegment = GameObject.Instantiate(vineSegmentPrefabs[0], position, Quaternion.identity, vineRoot);
//         prevSegment.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
//         prevSegment.GetComponent<HingeJoint2D>().enabled = false;

//         bool isWeak = RNG.SampleProbability(factoryConfig.pctChanceWeak);
//         int vineLength = RNG.RandomRange(factoryConfig.length.min, factoryConfig.length.max);
//         float segLength = factoryConfig.segmentLength;

//         Color vineColor = isWeak ? factoryConfig.weakSegmentColor : factoryConfig.normalSegmentColor;

//         // Track segments to apply to vineroot/VineLineRenderer
//         List<Transform> segments = new List<Transform>();

//         // Set up each segment
//         for (int i = 0; i < vineLength; i++)
//         {
//             // Get Random Vine segment prefab
//             Transform rndSegment = RNG.RandomChoice(vineSegmentPrefabs);
//             // instantiate new segment at previous segment minus segmentLength Y;
//             Vector2 newPosition = (Vector2)prevSegment.position + new Vector2(0, -segLength);
//             Transform newSegment = GameObject.Instantiate(rndSegment, newPosition, Quaternion.identity, vineRoot);
//             // Set up collider size
//             CapsuleCollider2D segCollider = newSegment.GetComponent<CapsuleCollider2D>();
//             segCollider.size = new Vector2(factoryConfig.segmentWidth, segLength);
//             // Setup hinge; configure anchor points and set previous segment as the connected rb; 
//             HingeJoint2D newHinge = newSegment.GetComponent<HingeJoint2D>();
//             float anchorYOffset = segLength * 0.4f;
//             newHinge.anchor = new Vector2(0f, anchorYOffset);
//             newHinge.connectedAnchor = new Vector2(0f, -anchorYOffset);
//             newHinge.connectedBody = prevSegment.GetComponent<Rigidbody2D>();
//             // Setup sprite to match segLength
//             SpriteRenderer spriteRenderer = newSegment.GetComponent<SpriteRenderer>();
//             spriteRenderer.size = new Vector2(factoryConfig.segmentWidth, segLength);
//             //  - set curl
//             bool hasCurl = RNG.SampleProbability(factoryConfig.pctChanceCurl);
//             if (hasCurl)
//             {
//                 JointMotor2D motor = new JointMotor2D();
//                 float motorForce = RNG.RandomRange(0, factoryConfig.maxCurlForce);
//                 float motorSpeed = RNG.RandomRange(-factoryConfig.curlSpeed, factoryConfig.curlSpeed);
//                 motor.motorSpeed = motorSpeed;
//                 motor.maxMotorTorque = motorForce;
//                 newHinge.motor = motor;
//                 newHinge.useMotor = true;
//             }

//             // Set Vine Color
//             newSegment.GetComponentInChildren<SpriteRenderer>().color = vineColor;

//             newHinge.breakForce = RNG.RandomRange(factoryConfig.normalBreakForce.min, factoryConfig.normalBreakForce.max);

//             // Add Adornment at Random
//             bool hasAdornment = RNG.SampleProbability(factoryConfig.pctChanceAdornment);
//             if (hasAdornment)
//             {
//                 Transform newAdornment = vineAdornmentFactory.GenerateVineAdornment(newSegment.position, newSegment, factoryConfig.adornmentScale);
//                 if (isWeak)
//                     newAdornment.GetComponentInChildren<SpriteRenderer>().color = vineColor;
//             }

//             // Set segmentIndex
//             newSegment.GetComponent<VineSegment>().SetSegmentIndex(i);

//             // Add Segment to segments list
//             segments.Add(newSegment);
//             // - update prevSegment
//             prevSegment = newSegment;
//         }

//         if (isWeak)
//         {
//             //pick one segment at random and reset breakforce to random weak breakforce
//             int numWeakSegments = (int)vineLength / 4;
//             for (int i = 0; i < numWeakSegments; i++)
//             {
//                 HingeJoint2D[] allSegments = vineRoot.gameObject.GetComponentsInChildren<HingeJoint2D>();
//                 HingeJoint2D rndSegment = RNG.RandomChoice(allSegments);
//                 float rndWeakBreakForce = RNG.RandomRange(factoryConfig.weakBreakForce.min, factoryConfig.weakBreakForce.max);
//                 rndSegment.breakForce = rndWeakBreakForce;
//             }
//         }

//         // Set segments in VineLineRenderer
//         vineRoot.GetComponent<VineRoot>().Init(segments, factoryConfig.segmentWidth, vineColor);

//         return vineRoot;
//     }
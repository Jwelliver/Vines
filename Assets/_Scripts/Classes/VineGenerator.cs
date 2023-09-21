using UnityEngine;
using System.Collections.Generic;
public static class VineGenerator
{
    /*
        ? implement vine width
        ? use width to influence avg strength
        ? use health to influence avg strength and color (weakColor + (normalcolor-weakcolor)*strength as pct of max strength)
        ? set weight of segment based on width
    */

    public static void GenerateVine(ProceduralVineSettings vineSettings, Vector2 position, Transform parent, List<Transform> vineSegmentPrefabs, List<Transform> adornmentPrefabs)
    {
        Transform prevSegment;
        //instantiate anchor
        prevSegment = GameObject.Instantiate(vineSegmentPrefabs[0], position, Quaternion.identity, parent);
        prevSegment.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        prevSegment.GetComponent<HingeJoint2D>().enabled = false;

        bool isWeak = RNG.SampleProbability(vineSettings.pctChanceWeak);
        int vineLength = RNG.RandomRange(vineSettings.length.min, vineSettings.length.max);

        float segLength = vineSettings.segmentLength;
        // Set up each segment
        for (int i = 0; i < vineLength; i++)
        {
            // Get Random Vine segment prefab
            Transform rndSegment = RNG.RandomChoice(vineSegmentPrefabs);
            // instantiate new segment at previous segment minus segmentLength Y;
            Vector2 newPosition = (Vector2)prevSegment.position + new Vector2(0, -segLength);
            Transform newSegment = GameObject.Instantiate(rndSegment, newPosition, Quaternion.identity, parent);
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
            bool hasCurl = RNG.SampleProbability(vineSettings.pctChanceCurl);
            if (hasCurl)
            {
                JointMotor2D motor = new JointMotor2D();
                float motorForce = RNG.RandomRange(0, vineSettings.maxCurlForce);
                float motorSpeed = RNG.RandomRange(-vineSettings.curlSpeed, vineSettings.curlSpeed);
                motor.motorSpeed = motorSpeed;
                motor.maxMotorTorque = motorForce;
                newHinge.motor = motor;
                newHinge.useMotor = true;
            }

            if (isWeak)
                newSegment.GetComponentInChildren<SpriteRenderer>().color = vineSettings.weakSegmentColor;//Color.Lerp(minWeakColor, maxWeakColor, howWeak);
            else
                newSegment.GetComponentInChildren<SpriteRenderer>().color = vineSettings.normalSegmentColor;

            newHinge.breakForce = RNG.RandomRange(vineSettings.normalBreakForce.min, vineSettings.normalBreakForce.max);

            bool hasAdornment = RNG.SampleProbability(vineSettings.pctChanceAdornment);
            if (hasAdornment)
            {
                Transform rndAdornment = RNG.RandomChoice(adornmentPrefabs);
                float randomRotation = RNG.RandomRange(0, 359);
                Transform newAdornment = GameObject.Instantiate(rndAdornment, newSegment.position, Quaternion.Euler(0, 0, randomRotation));
                float rndScale = RNG.RandomRange(vineSettings.adornmentScale.min, vineSettings.adornmentScale.max);
                newAdornment.localScale = new Vector2(rndScale, rndScale);
                newAdornment.SetParent(newSegment);
                if (isWeak)
                    newAdornment.GetComponentInChildren<SpriteRenderer>().color = vineSettings.weakSegmentColor;
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
                HingeJoint2D[] allSegments = parent.gameObject.GetComponentsInChildren<HingeJoint2D>();
                HingeJoint2D rndSegment = RNG.RandomChoice(allSegments);
                float rndWeakBreakForce = RNG.RandomRange(vineSettings.weakBreakForce.min, vineSettings.weakBreakForce.max);
                rndSegment.breakForce = rndWeakBreakForce;
            }
        }
    }


    // public GetSingleNormalVine(MinMax<int> length) {
    //     ProceduralVineSettings settings = new ProceduralVineSettings();
    //     settings.length = length;

    // }

}


//======= 092023 Version before adding Segment Length auto configure (e.g. set segment lenght in vine settings, then it auto adjusts sprite, collider size, etc.)

//    public static void GenerateVine(ProceduralVineSettings vineSettings, Vector2 position, Transform parent, List<Transform> vineSegmentPrefabs, List<Transform> adornmentPrefabs)
//     {
//         Transform prevSegment;
//         //instantiate anchor
//         prevSegment = GameObject.Instantiate(vineSegmentPrefabs[0], position, Quaternion.identity, parent);
//         prevSegment.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;

//         bool isWeak = RNG.SampleProbability(vineSettings.pctChanceWeak);
//         int length = RNG.RandomRange(vineSettings.length.min, vineSettings.length.max);
//         //for i in length:
//         for (int i = 0; i < length; i++)
//         {
//             // -  instantiate vineSegment at prev segment position - segmentLength*2;
//             Transform rndSegment = RNG.RandomChoice(vineSegmentPrefabs);//vineSegements[RNG.RandomRange(0, vineSegements.Count)];
//             // Vector2 newPosition = (Vector2)prevSegment.position - new Vector2(0, Mathf.Abs(rndSegment.localScale.y * 2) + segmentOffset); //ORIG
//             float colliderSizeY = rndSegment.GetComponent<CapsuleCollider2D>().size.y;
//             // float connectedAnchorPosY = rndSegment.GetComponent<HingeJoint2D>().connectedAnchor.y;
//             float positionYOffset = -colliderSizeY;
//             Vector2 newPosition = (Vector2)prevSegment.position + new Vector2(0, positionYOffset);
//             Transform newSegment = GameObject.Instantiate(rndSegment, newPosition, Quaternion.identity, parent);
//             //  - set hinge connected rb to previous segment; 
//             HingeJoint2D newHinge = newSegment.GetComponent<HingeJoint2D>();
//             newHinge.connectedBody = prevSegment.GetComponent<Rigidbody2D>();
//             //  - set curl
//             bool hasCurl = RNG.SampleProbability(vineSettings.pctChanceCurl);
//             if (hasCurl)
//             {
//                 JointMotor2D motor = new JointMotor2D();
//                 float motorForce = RNG.RandomRange(0, vineSettings.maxCurlForce);
//                 float motorSpeed = RNG.RandomRange(-vineSettings.curlSpeed, vineSettings.curlSpeed);
//                 motor.motorSpeed = motorSpeed;
//                 motor.maxMotorTorque = motorForce;
//                 newHinge.motor = motor;
//                 newHinge.useMotor = true;
//             }

//             if (isWeak)
//                 newSegment.GetComponentInChildren<SpriteRenderer>().color = vineSettings.weakSegmentColor;//Color.Lerp(minWeakColor, maxWeakColor, howWeak);
//             else
//                 newSegment.GetComponentInChildren<SpriteRenderer>().color = vineSettings.normalSegmentColor;

//             newHinge.breakForce = RNG.RandomRange(vineSettings.normalBreakForce.min, vineSettings.normalBreakForce.max);

//             bool hasAdornment = RNG.SampleProbability(vineSettings.pctChanceAdornment);
//             if (hasAdornment)
//             {
//                 Transform rndAdornment = RNG.RandomChoice(adornmentPrefabs);
//                 float randomRotation = RNG.RandomRange(0, 359);
//                 Transform newAdornment = GameObject.Instantiate(rndAdornment, newSegment.position, Quaternion.Euler(0, 0, randomRotation));
//                 float rndScale = RNG.RandomRange(vineSettings.adornmentScale.min, vineSettings.adornmentScale.max);
//                 newAdornment.localScale = new Vector2(rndScale, rndScale);
//                 newAdornment.SetParent(newSegment);
//                 if (isWeak)
//                     newAdornment.GetComponentInChildren<SpriteRenderer>().color = vineSettings.weakSegmentColor;
//             }

//             // - update prevSegment
//             prevSegment = newSegment;
//         }

//         if (isWeak)
//         {
//             //pick one segment at random and reset breakforce to random weak breakforce
//             int numWeakSegments = (int)length / 4;
//             for (int i = 0; i < numWeakSegments; i++)
//             {
//                 HingeJoint2D[] allSegments = parent.gameObject.GetComponentsInChildren<HingeJoint2D>();
//                 HingeJoint2D rndSegment = RNG.RandomChoice(allSegments);
//                 float rndWeakBreakForce = RNG.RandomRange(vineSettings.weakBreakForce.min, vineSettings.weakBreakForce.max);
//                 rndSegment.breakForce = rndWeakBreakForce;
//             }
//         }
//     }
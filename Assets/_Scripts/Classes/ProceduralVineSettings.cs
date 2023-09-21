using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class ProceduralVineSettings
//Describes settings and limits for a procedural vine; Provides method to return instance of VineParams derived from the settings
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


    // public VineParams GetRandomVineParams()
    // {

    // }
}


// public class VineParams
// {
//     // represents the parameters to construct a single vine
//     public int length;
// }

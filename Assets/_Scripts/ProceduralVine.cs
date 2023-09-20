using System;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralVine : MonoBehaviour
{
    [Header("Prefabs")]

    [SerializeField] List<Transform> vineSegments; //list of possible vine segments
    [SerializeField] List<Transform> adornments;

    [Header("Vine Settings")]
    public ProceduralVineSettings vineSettings = new ProceduralVineSettings();

    public SfxHandler sfx;


    void Awake()
    {
        try { sfx = GameObject.Find("SFX").GetComponent<SfxHandler>(); }
        catch { }
    }

    void Start()
    {
        VineGenerator.GenerateVine(vineSettings, transform.position, transform, vineSegments, adornments);
    }
}
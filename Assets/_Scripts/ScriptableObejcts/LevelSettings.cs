using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "MyAssets/ScriptableObjects/Level Settings")]
public class LevelSettings : ScriptableObject
{
    [Header("RNG Seed")]
    public int rngSeed = -1;

    [Header("Level Length")]
    public Vector2 startPos = new Vector2(0, 0);
    public MinMax<int> levelLength = new MinMax<int>(300, 500);
    public int globalStartOffset = 0;
    public int globalEndOffset = 0;

    [Header("Trees")]
    public MinMax<float> treeSpacing = new MinMax<float>(5f, 10f);
    public TreeFactoryConfig treeSettings = new TreeFactoryConfig();

    [Header("Vines")]
    public VineFactoryConfig vineSettings = new VineFactoryConfig();


    // public ProceduralLayoutParams treeLayoutParams; //todo: 091723 using for layout levelgen approach; if keeping, remove trees spacing vars
    // public int startOffset;
    // public int endOffset;

}

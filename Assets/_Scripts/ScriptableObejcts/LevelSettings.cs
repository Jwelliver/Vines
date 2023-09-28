using System;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(menuName = "MyAssets/ScriptableObjects/Level Settings")]
public class LevelSettings : ScriptableObject
{

    [Header("GameMode")]
    public LevelType levelType = LevelType.ENDLESS;

    [Header("RNG Seed")]
    public int rngSeed = -1;

    [Header("Level Length")]
    public Vector2 startPos = new Vector2(0, 0);
    public MinMax<int> levelLength = new MinMax<int>(300, 500);
    public LevelDirection direction = new LevelDirection();

    [Header("Trees")]
    // public MinMax<float> treeSpacing = new MinMax<float>(5f, 10f);
    // public TreeFactoryConfig treeSettings = new TreeFactoryConfig();
    public List<TreeLayer> treeLayers = new List<TreeLayer>();

    // [Header("Vines")]
    // public VineFactoryConfig vineSettings = new VineFactoryConfig();


    // public ProceduralLayoutParams treeLayoutParams; //todo: 091723 using for layout levelgen approach; if keeping, remove trees spacing vars
    // public int startOffset;
    // public int endOffset;

}

[Serializable]
public enum LevelType
{
    NORMAL,
    ENDLESS
}

[Serializable]
public enum LevelDirection
{
    LEFT = -1,
    RIGHT = 1,
    BOTH = 2
}

[Serializable]
public struct TreeLayer
{
    public MinMax<float> spacing;
    public TreeFactoryConfig treeSettings;
    public VineFactoryConfig vineSettings;
}

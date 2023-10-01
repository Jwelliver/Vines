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

    [Header("Tree Layers")]
    public List<TreeLayer> treeLayers = new List<TreeLayer>();
}

[Serializable]
public enum LevelType
{
    NORMAL = 0,
    ENDLESS = 1,
    ENDLESS_DEFINED_LAYERS = 2,
    ENDLESS_LINEAR_DIFFICULTY = 3,
    ENDLESS_WANDERING_DIFFICULTY = 4,

    // public static void GetStringList() {

    // }

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
    public string id;
    public bool enabled;
    public MinMax<float> spacing;
    public TreeFactoryConfig treeSettings;
    public VineFactoryConfig vineSettings;
    public int layerIndex; // auto set by LevelGen
}

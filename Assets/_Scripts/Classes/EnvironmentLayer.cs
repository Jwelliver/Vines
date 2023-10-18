using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public enum EnvLayerType
{
    SpriteObj,
    PrefabOnly
}

[Serializable]
public enum EnvLayerAutoSortType
{
    Disable,
    StartFromMySortOrder,
    Inherit
}

[Serializable]
public class EnvironmentLayer
{
    [Header("Layer Properties")]
    public string id;
    public bool enabled = true;
    [Header("Placement")]
    public MinMax<float> spacing;
    public float yOffset;
    public float yOffsetVariance;
    [Header("Parallax")]
    public bool enableParallax;
    public bool useAutoZDistance;
    public float zDistance;
    [Header("Object Properties")]
    public EnvLayerType type;
    public Transform prefab;
    public MinMax<float> scaleModifier = new MinMax<float>(1f, 1f);
    public float maxRotation;
    public bool enableRandomFlip;
    public ProbWeightItemList<Sprite> spritePool;
    public ProbWeightItemList<Color> colorPool;

    [Header("Sorting")]
    public string sortLayerName;
    public int sortOrder;
    public EnvLayerAutoSortType autoSort;
    public Color layerTint = new Color(1f, 1f, 1f, 1f);
    [Header("SortingGroup")]
    public bool useSortGroup;

    // [SerializeReference] public EnvironmentObjectFactoryConfig factoryConfig;


    public EnvironmentObjectFactoryConfig GetEnvironmentObjectFactoryConfig()
    {
        switch (type)
        {
            case EnvLayerType.SpriteObj:
                {
                    return new EnvironmentSpriteObjectFactoryConfig
                    {
                        scale = scaleModifier,
                        maxRotation = maxRotation,
                        enableRandomFlip = enableRandomFlip,
                        spritePool = spritePool,
                        colorPool = colorPool,
                        sortLayerName = sortLayerName,
                        sortOrder = sortOrder,
                        secondaryTint = layerTint
                    };
                }
            case EnvLayerType.PrefabOnly:
                {
                    return new EnvironmentObjectFactoryConfig
                    {
                        scale = scaleModifier,
                        maxRotation = maxRotation,
                        enableRandomFlip = enableRandomFlip,
                    };
                }
            default:
                {
                    return null;
                }
        }
    }
}

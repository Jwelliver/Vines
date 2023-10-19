using System;
using UnityEngine;

public class EnvironmentObjectFactoryConfig
{
    [Header("Transform Properties")]
    public MinMax<float> scale = new MinMax<float>(1f, 2f);
    public float maxRotation = 5f;
    public bool enableRandomFlip = true;
}

public class EnvironmentSpriteObjectFactoryConfig : EnvironmentObjectFactoryConfig
{
    [Header("Sprite Properties")]
    public ProbWeightItemList<Sprite> spritePool = null;
    public ProbWeightItemList<Color> colorPool = null;
    public string sortLayerName;
    public int sortOrder;
    public Color secondaryTint = new Color(1f, 1f, 1f, 1f); //Additional tint applied to selected color
}

public class EnvironmentObjectBlueprint
{
    public float scaleModifier;
    public float rotationAmt;
    public bool isFlipped;
}

public class EnvironmentSpriteObjectBlueprint : EnvironmentObjectBlueprint
{
    public Sprite sprite;
    public Color color;
    public string sortLayerName;
    public int sortOrder;
}


[CreateAssetMenu(menuName = "MyAssets/ScriptableObjects/Factories/EnvironmentObjFactory")]
public class EnvironmentObjectFactory : ScriptableObject
{

    [SerializeField] Transform defaultPrefab;



    EnvironmentObjectBlueprint GenerateBlueprint(EnvironmentObjectFactoryConfig config)
    {
        return new EnvironmentObjectBlueprint
        {
            scaleModifier = RNG.RandomRange(config.scale),
            rotationAmt = RNG.RandomRange(-config.maxRotation, config.maxRotation),
            isFlipped = config.enableRandomFlip ? RNG.RandomBool() : false
        };
    }

    EnvironmentSpriteObjectBlueprint GenerateBlueprint(EnvironmentSpriteObjectFactoryConfig config)
    {
        return new EnvironmentSpriteObjectBlueprint
        {
            scaleModifier = RNG.RandomRange(config.scale),
            rotationAmt = RNG.RandomRange(-config.maxRotation, config.maxRotation),
            isFlipped = config.enableRandomFlip ? RNG.RandomBool() : false,
            sprite = config.spritePool.getRandomItem(),
            color = config.colorPool != null && config.colorPool.items.Count > 0 ? config.colorPool.getRandomItem() * config.secondaryTint : config.secondaryTint,
            sortLayerName = config.sortLayerName,
            sortOrder = config.sortOrder
        };
    }

    // public Transform Instantiate<T>(Vector2 position, Transform parent, T factoryConfig, Transform prefabOverride) where T : EnvironmentObjectFactoryConfig
    // {
    //     if (factoryConfig is EnvironmentObjectFactoryConfig)
    //     {
    //         Debug.Log("Envfactory: normal obj");
    //         return InstantiateObj(position, parent, factoryConfig, prefabOverride);
    //     }
    //     else if (factoryConfig is EnvironmentSpriteObjectFactoryConfig)
    //     {
    //         Debug.Log("envFactory: Sprite OBj");
    //         return InstantiateSpriteObj(position, parent, factoryConfig as EnvironmentSpriteObjectFactoryConfig, prefabOverride);
    //     }
    //     return null;
    // }

    public Transform InstantiateSpriteObj(Vector2 position, Transform parent, EnvironmentSpriteObjectFactoryConfig factoryConfig, Transform prefabOverride = null)
    {
        // Set Prefab
        Transform prefab = prefabOverride ?? defaultPrefab;

        // Get New Blueprint
        EnvironmentSpriteObjectBlueprint blueprint = GenerateBlueprint(factoryConfig);

        // Instantiate new object
        Transform newObj = GameObject.Instantiate(prefab, position, Quaternion.identity);

        // Get SpriteRenderer
        SpriteRenderer spriteRenderer = newObj.GetComponent<SpriteRenderer>();

        // Apply Random Scale to SpriteRenderer if possible (eg. drawMode tiled or sliced); otherwise apply to the transform

        Vector2 scaleModifier = new Vector2(blueprint.scaleModifier, blueprint.scaleModifier);
        if (spriteRenderer == null || spriteRenderer.drawMode == SpriteDrawMode.Simple)
            newObj.localScale *= scaleModifier;
        else
            spriteRenderer.size *= scaleModifier;


        // Apply random flip
        if (blueprint.isFlipped)
            newObj.localScale = new Vector2(-newObj.localScale.x, newObj.localScale.y);

        // Apply Random Sprite and Color
        spriteRenderer.sprite = blueprint.sprite;
        spriteRenderer.color = blueprint.color;

        // Applying Sprite Sorting
        spriteRenderer.sortingLayerName = blueprint.sortLayerName;
        spriteRenderer.sortingOrder = blueprint.sortOrder;

        // Set Parent
        if (parent != null) newObj.SetParent(parent);

        // Return newObj
        return newObj;
    }

    public Transform InstantiateObj(Vector2 position, Transform parent, EnvironmentObjectFactoryConfig factoryConfig, Transform prefabOverride = null)
    {
        // Set Prefab
        Transform prefab = prefabOverride ?? defaultPrefab;

        // Get New Blueprint
        EnvironmentObjectBlueprint blueprint = GenerateBlueprint(factoryConfig);

        // Instantiate new object
        Transform newObj = GameObject.Instantiate(prefab, position, Quaternion.identity);

        // Apply Random Scale
        newObj.localScale *= new Vector2(blueprint.scaleModifier, blueprint.scaleModifier);

        // Apply random flip
        if (blueprint.isFlipped)
            newObj.localScale = new Vector2(-newObj.localScale.x, newObj.localScale.y);

        // Set Parent
        if (parent != null) newObj.SetParent(parent);

        // Return newObj
        return newObj;
    }

}
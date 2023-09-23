using System;
using UnityEngine;

public class EnvironmentObjectFactoryConfig
{
    [Header("Transform Properties")]
    public bool enableRandomScale = true;
    public MinMax<float> scale = new MinMax<float>(1f, 2f);
    public float maxRotation = 5f;
    public bool enableRandomFlip = true;
}

public class EnvironmentSpriteObjectFactoryConfig : EnvironmentObjectFactoryConfig
{
    [Header("Sprite Properties")]
    public ProbabilityWeightedSpritePool spritePool = null;
    public ProbabilityWeightedColorPool colorPool = null;
    public string sortLayerName = null;
    public int sortOrder;
    public Color secondaryTint = new Color(1f, 1f, 1f, 1f); //Additional tint applied to selected color
}

public class EnvironmentObjectBlueprint
{
    public float scale;
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


public class EnvironmentObjectFactory : ScriptableObject
{

    [SerializeField] Transform defaultPrefab;

    EnvironmentObjectBlueprint GenerateBlueprint(EnvironmentObjectFactoryConfig config)
    {
        return new EnvironmentObjectBlueprint
        {
            scale = config.enableRandomScale ? RNG.RandomRange(config.scale) : -1,
            rotationAmt = RNG.RandomRange(-config.maxRotation, config.maxRotation),
            isFlipped = config.enableRandomFlip ? RNG.RandomBool() : false
        };
    }

    EnvironmentSpriteObjectBlueprint GenerateBlueprint(EnvironmentSpriteObjectFactoryConfig config)
    {
        return new EnvironmentSpriteObjectBlueprint
        {
            scale = config.enableRandomScale ? RNG.RandomRange(config.scale) : -1,
            rotationAmt = RNG.RandomRange(-config.maxRotation, config.maxRotation),
            isFlipped = config.enableRandomFlip ? RNG.RandomBool() : false,
            sprite = config.spritePool.getRandomItem(),
            color = config.colorPool != null ? config.colorPool.getRandomItem() * config.secondaryTint : config.secondaryTint,
            sortLayerName = config.sortLayerName,
            sortOrder = config.sortOrder
        };
    }

    public Transform Instantiate(Vector2 position, Transform parent, EnvironmentSpriteObjectFactoryConfig factoryConfig, Transform prefabOverride = null)
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
        if (factoryConfig.enableRandomScale)
        {
            Vector2 newScale = new Vector2(blueprint.scale, blueprint.scale);
            if (spriteRenderer == null || spriteRenderer.drawMode == SpriteDrawMode.Simple)
                newObj.localScale = newScale;
            else
                spriteRenderer.size *= newScale; //TODO: verify this uniformly scales as expected
        }

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

    public Transform Instantiate(Vector2 position, Transform parent, EnvironmentObjectFactoryConfig factoryConfig, Transform prefabOverride = null)
    {
        // Set Prefab
        Transform prefab = prefabOverride ?? defaultPrefab;

        // Get New Blueprint
        EnvironmentObjectBlueprint blueprint = GenerateBlueprint(factoryConfig);

        // Instantiate new object
        Transform newObj = GameObject.Instantiate(prefab, position, Quaternion.identity);

        // Apply Random Scale
        if (factoryConfig.enableRandomScale)
            newObj.localScale = new Vector2(blueprint.scale, blueprint.scale);

        // Apply random flip
        if (blueprint.isFlipped)
            newObj.localScale = new Vector2(-newObj.localScale.x, newObj.localScale.y);

        // Set Parent
        if (parent != null) newObj.SetParent(parent);

        // Return newObj
        return newObj;
    }

}
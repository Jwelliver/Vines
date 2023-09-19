using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyAssets/ScriptableObjects/Layouts/Level Generator (layout version)")]
public class LevelGeneratorNew : ScriptableObject
{
    [Header("Level")]
    [SerializeField] string proceduralLevelContainerName = "ProceduralLevelContainer";
    [SerializeField] LevelSettings levelSettings;

    [Header("MainTree Layout")]
    [SerializeField] ProceduralObjectLayout mainTreeLayout;

    [Header("Background Layout")]
    [SerializeField] BackgroundSpriteLayoutGroup backgroundLayoutGroup;

    [Header("Prefabs")]

    [SerializeField] Transform winPlatformPrefab;

    Section levelSection;

    private void initLevelSection()
    {
        levelSection = new Section
        {
            startPos = levelSettings.startPos,
            length = RNG.RandomRange(levelSettings.minLength, levelSettings.maxLength),
            startOffset = levelSettings.globalStartOffset,
            endOffset = levelSettings.globalEndOffset
        };
    }

    public void generateLevel()
    {
        initLevelSection();
        addBackgroundLayerSection(levelSection);
        addTreeLayerSection(levelSection, levelSettings.treeLayoutParams);
        addWinPlatform(new Vector2(levelSection.length, levelSettings.startPos.y - 1.5f));
    }

    void addTreeLayerSection(Section section, ProceduralLayoutParams layoutParams)
    {
        string treeLayerParentPath = proceduralLevelContainerName + "/" + "TreeContainer";
        Transform parent = GameObject.Find(treeLayerParentPath).transform ?? null;
        mainTreeLayout.layoutParams = layoutParams;
        mainTreeLayout.GenerateSection(section, parent);
    }

    void addBackgroundLayerSection(Section section)
    {
        backgroundLayoutGroup.GenerateAllLayouts(section);
    }

    void addWinPlatform(Vector2 pos)
    {
        GameObject.Instantiate(winPlatformPrefab, pos, Quaternion.identity);
    }
}


// [CreateAssetMenu(menuName = "MyAssets/ScriptableObjects/Layouts/Level Generator (layout version)")]
// public class LevelGeneratorNew : ScriptableObject
// {
//     [Header("Level")]
//     [SerializeField] string proceduralLevelContainerName = "ProceduralLevelContainer";
//     [SerializeField] LevelSettings levelSettings;

//     [Header("MainTree Layout")]
//     [SerializeField] ProceduralObjectLayout mainTreeLayout;

//     [Header("Background Layout")]
//     [SerializeField] BackgroundLayerGroup backgroundLayers;

//     [Header("Prefabs")]

//     [SerializeField] Transform winPlatformPrefab;

//     Section levelSection;

//     private void initLevelSection()
//     {
//         levelSection = new Section
//         {
//             startPos = levelSettings.startPos,
//             length = RNG.RandomRange(levelSettings.minLength, levelSettings.maxLength),
//             startOffset = levelSettings.globalStartOffset,
//             endOffset = levelSettings.globalEndOffset
//         };
//     }

//     public void generateLevel()
//     {
//         initLevelSection();
//         addBackgroundLayerSection(levelSection);
//         addTreeLayerSection(levelSection, levelSettings.treeLayoutParams);
//         addWinPlatform(new Vector2(levelSection.length, levelSettings.startPos.y - 1.5f));
//     }

//     void addTreeLayerSection(Section section, ProceduralLayoutParams layoutParams)
//     {
//         string treeLayerParentPath = proceduralLevelContainerName + "/" + "TreeContainer";
//         Transform parent = GameObject.Find(treeLayerParentPath).transform ?? null;
//         mainTreeLayout.layoutParams = layoutParams;
//         mainTreeLayout.GenerateSection(section, parent);
//     }

//     void addBackgroundLayerSection(Section section)
//     {
//         backgroundLayoutGroup.GenerateAllLayouts(section);
//     }

//     void addWinPlatform(Vector2 pos)
//     {
//         GameObject.Instantiate(winPlatformPrefab, pos, Quaternion.identity);
//     }
// }


//=========================

/*
091823
    - Generate a prefab at random positions along section using layoutParams (min,max spacing) and return Transforms[]
    - Take returned transforms and pass to appropriate method with randomization settings for that object
        - that method uses the settings to call appropriate static methods that perform the required randomizations
*/

public static class ProceduralGenerator
{
    public static void GenerateLayer<T>(Section section, T layer, Transform parent) where T : ProceduralLayer
    {
        List<Vector2> positions = LayoutPositionGenerator.GeneratePositions(section, layer.layoutParams.minSpacing, layer.layoutParams.maxSpacing, layer.layoutParams.yOffset);
        foreach (Vector2 pos in positions)
        {
            Transform newObj = GameObject.Instantiate(layer.prefab, pos, Quaternion.identity, parent);
            Randomizer.Randomize(newObj, layer.layerParams);
        }
    }
}

public class ProceduralLayer
{
    public Transform prefab;
    public RndTransformParams layerParams;
    public RndLayoutParams layoutParams;
}

public class ProceduralBackgroundLayer : ProceduralLayer
{
    public string id;
    new public RndBgObjectParams layerParams;
    public Color layerTint;
    public string sortingLayerName;
    public int sortOrder;
    public bool enableParallax;
    public int zDistance;

    // public Transform GetParent(string path) {
    //     Transform parent = GameObject.Find(path+"/"+id).transform ?? GameObject.Instantiate()
    // }
}

public class BackgroundLayerGroup
{
    // public string groupParentPath;
    public string groupSortLayerName;
    public List<ProceduralBackgroundLayer> layers;

    public void ApplyLayerInfo()
    {
        for (int i = 0; i < layers.Count; i++)
        {
            ProceduralBackgroundLayer layer = layers[i];
            layer.sortingLayerName = groupSortLayerName;
            layer.sortOrder = i;
        }
    }

    // public void GenerateAllLayers(Section section, Transform parent) {
    //     foreach(ProceduralBackgroundLayer layer in layers) {
    //         layer.GenerateLayer(section);
    //     }
    // }
}

//==========================================================================================================
public class RndLayoutParams
{
    public float minSpacing;
    public float maxSpacing;
    public float yOffset;
}


public class RndParams { }


public class RndTransformParams : RndParams
{
    public bool enableRandomScale;
    public float minScale;
    public float maxScale;
    public bool enableRandomFlip;
}

public class RndBgObjectParams : RndTransformParams
{
    public ProbabilityWeightedSpritePool sprites;
}


//==========================================================================================================
public static class TreeRandomizer
{

}


public static class Randomizer
{

    // Generic Transform Randomizer
    public static void Randomize(Transform obj, RndTransformParams rndParams)
    {
        if (rndParams.enableRandomScale)
        {
            TransformRandomizer.ApplyRandomScale(obj, rndParams.minScale, rndParams.maxScale);
        }
        if (rndParams.enableRandomFlip)
        {
            TransformRandomizer.ApplyRandomFlip(obj);
        }
    }

    // Bg Sprite object randomizer
    public static void Randomize(Transform obj, RndBgObjectParams rndParams)
    {
        if (rndParams.enableRandomScale)
        {
            SpriteRandomizer.ApplyRandomScale(obj, rndParams.minScale, rndParams.maxScale);
        }
        if (rndParams.enableRandomFlip)
        {
            TransformRandomizer.ApplyRandomFlip(obj);
        }
        SpriteRandomizer.ApplyRandomSprite(obj, rndParams.sprites);
    }
}



public static class SpriteRandomizer
{
    static SpriteRenderer GetSpriteRenderer(Transform obj, bool addIfMissing = true)
    {
        try
        {
            SpriteRenderer spriteRenderer = obj.gameObject.GetComponent<SpriteRenderer>();
            return spriteRenderer;
        }
        catch
        {
            return addIfMissing ? obj.gameObject.AddComponent<SpriteRenderer>() : null;
        }
    }

    public static void ApplyRandomScale(Transform obj, float minScale, float maxScale)
    {//overrides base method to apply the scale directly to spriterenderer if the drawMode is sliced or tiled.
        SpriteRenderer spriteRenderer = GetSpriteRenderer(obj);
        if (spriteRenderer == null || spriteRenderer.drawMode == SpriteDrawMode.Simple)
        {
            TransformRandomizer.ApplyRandomScale(obj, minScale, maxScale);
            return;
        }
        float newScalar = RNG.RandomRange(minScale, maxScale);
        Vector2 newScale = new Vector2(newScalar, newScalar);
        spriteRenderer.size *= newScale;
        return;
    }

    public static void ApplyRandomSprite(Transform obj, ProbabilityWeightedSpritePool sprites)
    {
        SpriteRenderer spriteRenderer = GetSpriteRenderer(obj);
        if (spriteRenderer == null) { return; }
        Sprite rndSprite = sprites.getRandomItem();
        spriteRenderer.sprite = rndSprite;
        return;
    }

    public static void ApplyRandomSprite(Transform obj, List<Sprite> sprites)
    {
        SpriteRenderer spriteRenderer = GetSpriteRenderer(obj);
        if (spriteRenderer == null) { return; }
        Sprite rndSprite = RNG.RandomChoice(sprites);
        spriteRenderer.sprite = rndSprite;
        return;
    }
}


public static class TransformRandomizer
{
    // contains generic randomizer methods 

    public static void ApplyRandomScale(Transform obj, float minScale, float maxScale)
    {
        float rndScale = RNG.RandomRange(minScale, maxScale);
        obj.localScale = new Vector2(rndScale, rndScale);
        return;
    }

    public static void ApplyRandomFlip(Transform obj)
    {
        if (RNG.RandomBool())
            obj.localScale = new Vector2(-obj.localScale.x, obj.localScale.y);
    }


}

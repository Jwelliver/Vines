using System;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SpriteWithProbability
{
    public Sprite sprite;
    public float probability;
}

[CreateAssetMenu(menuName = "MyAssets/ScriptableObjects/Procedural Background")]
public class ProceduralBackground : ScriptableObject
{
    [SerializeField] string parentTransformPath;
    [Header("Objects")]
    [SerializeField] Transform prefab;
    [SerializeField] List<SpriteWithProbability> sprites = new List<SpriteWithProbability>();
    [SerializeField] int minDistanceBetweenObjs = 3;
    [SerializeField] int maxDistanceBetweenObjs = 5;
    [SerializeField] float minScale = 3f;
    [SerializeField] float maxScale = 5f;

    [Header("Layer")]
    [SerializeField] string sortLayerName;
    [SerializeField] int sortOrder = 0;
    [SerializeField] Color color = Color.white;

    public int nObjects = 0;


    private Sprite getRandomSprite()
    {
        SpriteWithProbability testSprite = RNG.RandomChoice(sprites);
        if (RNG.SampleProbability(testSprite.probability))
        {
            return testSprite.sprite;
        }
        else
        {
            return getRandomSprite();
        }
    }

    public void staticBatchLayer(Transform parent)
    {
        StaticBatchingUtility.Combine(parent.gameObject);
    }


    public void populateObjects(int levelLength, int levelEdgeOffset)
    {
        Transform parent;
        try
        {
            parent = GameObject.Find(parentTransformPath).transform;
        }
        catch (Exception e)
        {
            Debug.LogError("Error finding parent: " + e);
            return;
        }

        for (int i = -Math.Abs(levelEdgeOffset); i < levelLength + Mathf.Abs(levelEdgeOffset); i += RNG.RandomRange(minDistanceBetweenObjs, maxDistanceBetweenObjs))
        {
            Transform newObj = GameObject.Instantiate(prefab, new Vector2(i, parent.position.y), Quaternion.identity);
            SpriteRenderer newObjSpriteRenderer = newObj.GetComponent<SpriteRenderer>();
            Sprite rndSprite = getRandomSprite();
            newObjSpriteRenderer.sprite = rndSprite;
            newObjSpriteRenderer.color = color;
            newObjSpriteRenderer.sortingLayerName = sortLayerName;
            newObjSpriteRenderer.sortingOrder = sortOrder;
            float rndScale = RNG.RandomRange(minScale, maxScale);
            newObj.localScale = new Vector2(rndScale, rndScale);
            if (RNG.RandomBool()) //random isFlipped
            {
                newObj.localScale = new Vector2(-newObj.localScale.x, newObj.localScale.y);
            }
            newObj.SetParent(parent);
            nObjects++;
        }
        // staticBatchLayer(parent);
    }
}

//=============================================
//0915123 background layer level gen revamp; Simple version
//============================================


/*

class TransformSettings
    * settings to be applied to a single procedural object
    - Contains:
        - scale
        - position
        - isFlipped    

static class TransformSettingsGenerator
    // - generate(Transfrom prefab, Section section, Transform? parent)
    //    * instantiates a layout of prefabs 

    - TransformSettings generateSample(ProceduralLayoutParams params)
        * returns a single TransformSettings from params
    - TransformSettings[] generateSection(ProceduralLayoutParams params, Section section)
        * returns an array of TransformSettings across a given section

ProceduralObjectLayout
    * describes how a prefab will be generated across a given section
    - Contains:
        - Prefab
        - minSpacing, maxSpacing
        - minScale, maxScale
        - enableRandomFlip


ProceduralSpriteLayout: ProceduralObjectLayout
    * a proceduralObjectLayout for sprite objects (eg. used for background layouts)
    - Contains:
        - prefab
        - ProbabilityWeightedSpritePool
        - ProceduralLayout
        - Layername
        - Color


BackgroundLayerGroup
    - Contains:
        - ParentPath
            * primary path for all layers
        - LayerList:
            - LayerName
            - List<ProceduralSpritePool>
        


*/


public static class LayoutPositionGenerator
{
    public static List<Vector2> GeneratePositions(Section section, float minSpacing = 1, float maxSpacing = 3)
    {
        if (minSpacing == 0 && maxSpacing == 0)
        {
            Debug.LogError("Error: Incorrect Spacing values. \nminSpacing: " + minSpacing + " | maxSpacing: " + maxSpacing);
            return null;
        }
        float startX = section.startPos.x;
        float endX = startX + section.length;
        float y = section.startPos.y;
        float endOffset = section.endOffset;
        float startOffset = section.startOffset;

        List<Vector2> positions = new List<Vector2>();
        for (float x = startX + startOffset; x < endX + endOffset; x += RNG.RandomRange(minSpacing, maxSpacing))
        {
            positions.Add(new Vector2(x, y));
        }
        return positions;
    }
}

// public struct TransformSettings
// {
//     public Vector3 scale;
//     public Vector3 position;
//     public bool isFlipped;
// }

[Serializable]
public struct ProceduralLayoutParams
{
    public bool enableRandomScale;
    public float minScale;
    public float maxScale;
    public float minSpacing;
    public float maxSpacing;
    public bool enableRandomFlip;
}

[CreateAssetMenu(menuName = "MyAssets/ScriptableObjects/Layouts/ProceduralObjectLayout")]
public class ProceduralObjectLayout : ScriptableObject
{
    // public string containerParentPath; //where to place layer parent transform
    public Transform prefab;
    public ProceduralLayoutParams layoutParams;
    // private Transform parent;

    public void GenerateSection(Section section, Transform parent = null)
    {
        List<Vector2> positions = LayoutPositionGenerator.GeneratePositions(section, layoutParams.minSpacing, layoutParams.maxSpacing);
        foreach (Vector2 pos in positions)
        {
            Transform newObj = GameObject.Instantiate(prefab, pos, Quaternion.identity, parent);
            ApplyLayoutParams(newObj);
            ApplyExtraSetup(newObj);
        }
    }

    public virtual void ApplyRandomScale(Transform obj)
    {
        if (!layoutParams.enableRandomScale) return;
        float rndScale = RNG.RandomRange(layoutParams.minScale, layoutParams.maxScale);
        obj.localScale = new Vector2(rndScale, rndScale);
        return;
    }

    public virtual void ApplyRandomFlip(Transform obj)
    {
        if (layoutParams.enableRandomFlip && RNG.RandomBool())
            obj.localScale = new Vector2(-obj.localScale.x, obj.localScale.y);
    }

    public virtual void ApplyExtraSetup(Transform obj) { }//override in subclasses for additional setup;

    private void ApplyLayoutParams(Transform obj)
    {
        ApplyRandomScale(obj);
        ApplyRandomFlip(obj);
    }
}

[CreateAssetMenu(menuName = "MyAssets/ScriptableObjects/Layouts/ProceduralSpriteLayout")]
public class ProceduralSpriteObjectLayout : ProceduralObjectLayout
{
    public ProbabilityWeightedSpritePool sprites;
    public string sortLayerName = null;
    public int sortOrder;
    public Color color = new Color(255f, 255f, 255f);

    SpriteRenderer GetSpriteRenderer(Transform obj)
    {
        try
        {
            SpriteRenderer spriteRenderer = obj.gameObject.GetComponent<SpriteRenderer>();
            return spriteRenderer;
        }
        catch
        {
            return obj.gameObject.AddComponent<SpriteRenderer>();
        }
    }

    public override void ApplyRandomScale(Transform obj)
    {//overrides base method to apply the scale directly to spriterenderer if the drawMode is sliced or tiled.
        SpriteRenderer spriteRenderer = GetSpriteRenderer(obj);
        if (spriteRenderer == null || spriteRenderer.drawMode == SpriteDrawMode.Simple)
        {
            base.ApplyRandomScale(obj);
            return;
        }
        float newScalar = RNG.RandomRange(layoutParams.minScale, layoutParams.maxScale);
        Vector2 newScale = new Vector2(newScalar, newScalar);
        spriteRenderer.size *= newScale;
        return;
    }

    public override void ApplyExtraSetup(Transform obj)
    {
        SpriteRenderer spriteRenderer = GetSpriteRenderer(obj);
        if (spriteRenderer == null) { return; }
        Sprite rndSprite = sprites.getRandomItem();
        spriteRenderer.sprite = rndSprite;
        spriteRenderer.color = color;
        spriteRenderer.sortingLayerName = sortLayerName;
        spriteRenderer.sortingOrder = sortOrder;
        return;
    }
}


// public class ProceduralObjectLayer
// {

// }

// public class ProceduralBackgroundLayer
// {
//     public string id;
//     public Vector3 paralaxSpeed = new Vector3(0, 0, 0);
//     public Color color = new Color(255, 255, 255);
//     public ProceduralSpriteObjectLayout layout;

//     // public Transform CreateLayoutParentContainer() {
//     // }
// }

[CreateAssetMenu(menuName = "MyAssets/ScriptableObjects/Layouts/BackgroundSpriteLayoutGroup")]
public class BackgroundSpriteLayoutGroup : ScriptableObject
{
    public string layerName;
    public List<ProceduralSpriteObjectLayout> layouts;

    void OnValidate()
    {
        UpdateLayouts();
    }

    public void UpdateLayouts()
    {
        //applies layerName and sortOrder to layouts
        for (int i = 0; i < layouts.Count; i++)
        {
            ProceduralSpriteObjectLayout layout = layouts[i];
            layout.sortLayerName = layerName;
            layout.sortOrder = i;
        }
    }

    public void GenerateAllLayouts(Section section, Transform parent = null)
    {
        foreach (ProceduralSpriteObjectLayout layout in layouts)
        {
            layout.GenerateSection(section, parent);
        }
    }
}
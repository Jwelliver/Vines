using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProbWeightedItem<T>
{
    public T item;
    public float probability;
}

[Serializable]
class ProbWeightItemList<T>
{
    public List<ProbWeightedItem<T>> items = new List<ProbWeightedItem<T>>();
    public T getRandom()
    {
        if (items.Count == 0) { return default; }//should return nullable value of T
        ProbWeightedItem<T> item = RNG.RandomChoice(items);
        if (RNG.SampleProbability(item.probability))
        {
            return item.item;
        }
        else
        {
            return getRandom();
        }
    }
}


[Serializable]
public class ProceduralObject
{
    // public string id;
    // public string parentPath;
    public Transform prefab;
    public bool enableRandomScale = true;
    public float minScale;
    public float maxScale;
    public bool enableRandomFlip = true;

    // private Transform getParent()
    // {
    //     Transform parent;
    //     try
    //     {
    //         parent = GameObject.Find(parentPath).transform;
    //         return parent;

    //     }
    //     catch (Exception e)
    //     {
    //         Debug.LogError("Error > Parent not found: parentPath: " + parentPath + "\n Exception: " + e);
    //         return null;
    //     }
    // }

    public virtual void applyRandomScale(Transform obj)
    {
        if (!enableRandomScale) return;
        float rndScale = RNG.RandomRange(minScale, maxScale);
        obj.localScale = new Vector2(rndScale, rndScale);
        return;
    }

    private void applyRandomFlip(Transform obj)
    {
        if (enableRandomFlip && RNG.RandomBool())
            obj.localScale = new Vector2(-obj.localScale.x, obj.localScale.y);
        return;
    }

    public void createSingle(Vector2 position, Transform parent)
    {
        // Transform parent = parentRef ?? getParent();
        if (parent == null) return;
        Transform newObj = GameObject.Instantiate(prefab, position, Quaternion.identity, parent);
        applyRandomScale(newObj);
        applyRandomFlip(newObj);
        objectSetup(newObj);
    }

    public void createMany(List<Vector2> positions, Transform parent)//, Transform? parent
    {
        // Transform parent = getParent();
        if (parent == null) return;
        foreach (Vector2 pos in positions)
        {
            createSingle(pos, parent);
        }
    }

    public virtual void objectSetup(Transform obj)
    {
        return;
    }
}

[Serializable]
class ProceduralSpriteObject : ProceduralObject
{
    public ProbWeightItemList<Sprite> weightedSprites;
    public string sortLayerName = null;
    public int sortOrder;
    public Color color = new Color(255f, 255f, 255f);

    SpriteRenderer getSpriteRenderer(Transform obj)
    {
        try
        {
            SpriteRenderer spriteRenderer = obj.gameObject.GetComponent<SpriteRenderer>();
            return spriteRenderer;
        }
        catch (Exception e)
        {
            Debug.LogError("Error > cannot find sprite renderer on obj: " + obj.name + "\n Exception: " + e);
            return null;
        }
    }

    public override void applyRandomScale(Transform obj)
    {
        SpriteRenderer spriteRenderer = getSpriteRenderer(obj);
        if (spriteRenderer == null || spriteRenderer.drawMode == SpriteDrawMode.Simple)
        {
            base.applyRandomScale(obj);
            return;
        }
        float newScalar = RNG.RandomRange(minScale, maxScale);
        Vector2 newScale = new Vector2(newScalar, newScalar);
        spriteRenderer.size *= newScale; //TODO: verify this uniformly scales as expected
        return;
    }

    public override void objectSetup(Transform obj)
    {
        SpriteRenderer spriteRenderer = getSpriteRenderer(obj);
        if (spriteRenderer == null) { return; }
        Sprite rndSprite = weightedSprites.getRandom();
        spriteRenderer.sprite = rndSprite;
        spriteRenderer.color = color;
        spriteRenderer.sortingLayerName = sortLayerName;
        spriteRenderer.sortingOrder = sortOrder;
        return;
    }
}



[Serializable]
public class Section
{
    public Vector2 startPos;
    public int length;
    public float startOffset = 0;
    public float endOffset = 0;

    public Section getCopy()
    {
        return new Section
        {
            startPos = startPos,
            length = length,
            startOffset = startOffset,
            endOffset = endOffset
        };
    }
}

class ProceduralLayerGenerator
{
    private List<Vector2> getPositions(Section section, float minSpacing = 1, float maxSpacing = 3)
    {
        if (minSpacing == 0 && maxSpacing == 0 || maxSpacing < minSpacing)
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

    public void populateLayerSection<T>(ProceduralLayer<T> proceduralLayer, Section section, Transform parent) where T : ProceduralObject
    {
        if (proceduralLayer == null || proceduralLayer.proceduralObject == null || section == null)
        {
            Debug.LogError("Error: missing params");
            return;
        }
        if (!proceduralLayer.enabled) { return; }
        List<Vector2> positions = getPositions(section, proceduralLayer.minSpacing, proceduralLayer.maxSpacing);
        proceduralLayer.proceduralObject.createMany(positions, parent);
    }
}


[Serializable]
public class ProceduralLayer<T> where T : ProceduralObject
{
    public string id;
    public bool enabled = true;
    public T proceduralObject;
    public float minSpacing;
    public float maxSpacing;
    public float yOffset;
    public bool enableParallax;
    public float zDistance;
}


[CreateAssetMenu(menuName = "MyAssets/ScriptableObjects/Level Generator")]
public class LevelGenerator : ScriptableObject
{
    [Header("Level")]
    [SerializeField] string proceduralLevelContainerName = "ProceduralLevelContainer";
    [SerializeField] string treeParentName = "TreeContainer";
    [SerializeField] string backgroundParentName = "SpriteLayers";
    [SerializeField] LevelSettings levelSettings;

    [Header("Background Layers")]
    [SerializeField] List<ProceduralLayer<ProceduralSpriteObject>> backgroundLayers = new List<ProceduralLayer<ProceduralSpriteObject>>();

    [Header("Prefabs")]

    [SerializeField] Transform treePrefab;

    [SerializeField] Transform winPlatformPrefab;
    [SerializeField] Transform blankParent;

    Section levelSection;

    ProceduralLayerGenerator layerGenerator = new ProceduralLayerGenerator();



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
        addTreeLayerSection(levelSection, levelSettings.treesMinSpacing, levelSettings.treesMaxSpacing);
        addWinPlatform(new Vector2(levelSection.length, levelSettings.startPos.y - 1.5f));
    }

    void addTreeLayerSection(Section section, float minSpacing, float maxSpacing)
    {
        string treeLayerParentPath = proceduralLevelContainerName + "/" + treeParentName;
        Transform treeLayerParent = GameObject.Find(treeLayerParentPath).transform;
        ProceduralObject tree = new ProceduralObject
        {
            // id = "Tree",
            // parentPath = treeLayerParentPath,
            prefab = treePrefab,
            enableRandomScale = false,
        };
        ProceduralLayer<ProceduralObject> layer = new ProceduralLayer<ProceduralObject>
        {
            id = "TreeLayer",
            proceduralObject = tree,
            minSpacing = minSpacing,
            maxSpacing = maxSpacing,
        };
        layerGenerator.populateLayerSection(layer, section, treeLayerParent);
    }

    // void addBackgroundLayerSection(Section section)
    // {
    //     foreach (ProceduralLayer<ProceduralSpriteObject> i in backgroundLayers)
    //     {
    //         if (i.yOffset != 0)
    //         {
    //             Section newSection = section.getCopy();
    //             newSection.startPos = new Vector2(section.startPos.x, section.startPos.y + i.yOffset);
    //             layerGenerator.populateLayerSection(i, newSection);
    //             // Debug.Log("addBackgroundLayerSection() > section.startPos.y: " + section.startPos.y + " | yOffset: " + i.yOffset);
    //             continue;
    //         }
    //         layerGenerator.populateLayerSection(i, section);
    //     }
    // }

    void addBackgroundLayerSection(Section section)
    {
        Transform layerParentContainer = GameObject.Find(proceduralLevelContainerName + "/" + backgroundParentName).transform;

        for (int i = 0; i < backgroundLayers.Count; i++)
        {
            ProceduralLayer<ProceduralSpriteObject> layer = backgroundLayers[i];
            if (!layer.enabled) { continue; }
            Transform layerParent = GameObject.Instantiate(blankParent, layerParentContainer);
            layerParent.name = layer.id;
            if (layer.enableParallax)
            {
                layerParent.gameObject.AddComponent<Paralaxer>();
                layerParent.position = new Vector3(layerParent.position.x, layerParent.position.y, layer.zDistance);
            }

            if (layer.yOffset != 0)
            {
                Section newSection = section.getCopy();
                newSection.startPos = new Vector2(section.startPos.x, section.startPos.y + layer.yOffset);
                layerGenerator.populateLayerSection(layer, newSection, layerParent);
                // Debug.Log("addBackgroundLayerSection() > section.startPos.y: " + section.startPos.y + " | yOffset: " + i.yOffset);
                continue;
            }
            layerGenerator.populateLayerSection(layer, section, layerParent);
        }
    }


    void addWinPlatform(Vector2 pos)
    {
        GameObject.Instantiate(winPlatformPrefab, pos, Quaternion.identity);
    }



}



//============== ORIG 091023
// [CreateAssetMenu]
// public class LevelGenerator : ScriptableObject
// {
//     [Header("Level")]
//     [SerializeField] string proceduralLevelContainerName = "ProceduralLevelContainer";
//     [SerializeField] int levelLengthMin = 50;
//     [SerializeField] int levelLengthMax = 50;
//     [SerializeField] int levelEdgeOffset = 0;

//     [Header("Trees")]
//     // [SerializeField] Transform treeContainer;
//     [SerializeField] RectTransform tree;
//     [SerializeField] int minDistanceBetweenTrees = 3;
//     [SerializeField] int maxDistanceBetweenTrees = 5;

//     [Header("Background Layers")]
//     [SerializeField] List<ProceduralBackground> foliageLayers = new List<ProceduralBackground>();
//     [SerializeField] List<ProceduralBackground> paralaxLayers = new List<ProceduralBackground>();

//     [Header("Prefabs")]

//     [SerializeField] Transform backgroundObject;

//     [SerializeField] Transform winPlatform;

//     int levelLength;


//     // void createProceduralParent() {
//     //     GameObject parent = new GameObject("Procedural_SO");
//     // }

//     //int levelLength, int levelStartOffset, int minTreeDistance, int maxTreeDistance
//     public void generateLevel()
//     {
//         levelLength = getRandomLevelLength();
//         populateBackground();
//         populateTrees();
//         generateWinPlatform();
//     }

//     int getRandomLevelLength()
//     {
//         return UnityEngine.Random.Range(levelLengthMin, levelLengthMax);
//     }
//     void populateTrees()
//     {
//         string treeContainerPath = proceduralLevelContainerName + "/" + "TreeContainer";
//         Transform treeContainer = GameObject.Find(treeContainerPath).transform;
//         for (int i = -Math.Abs(levelEdgeOffset); i < levelLength + Math.Abs(levelEdgeOffset); i += UnityEngine.Random.Range(minDistanceBetweenTrees, maxDistanceBetweenTrees))
//         {
//             GameObject.Instantiate(tree, new Vector2(i, 0), Quaternion.identity, treeContainer);
//         }
//     }

//     public int getTotalBackgroundObjects()
//     {
//         int totalObjects = 0;
//         foreach (ProceduralBackground i in foliageLayers)
//         {
//             totalObjects += i.nObjects;
//         }
//         foreach (ProceduralBackground i in paralaxLayers)
//         {
//             totalObjects += i.nObjects;
//         }
//         return totalObjects;
//     }

//     void populateBackground()
//     {

//         foreach (ProceduralBackground i in foliageLayers)
//         {
//             i.populateObjects(levelLength, levelEdgeOffset);
//         }
//         foreach (ProceduralBackground i in paralaxLayers)
//         {
//             i.populateObjects(levelLength, levelEdgeOffset);
//         }

//     }

//     void generateWinPlatform()
//     {
//         GameObject.Instantiate(winPlatform, new Vector2(levelLength, -1.5f), Quaternion.identity);
//     }


// }

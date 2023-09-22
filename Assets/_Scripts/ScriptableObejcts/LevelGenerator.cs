using System;
using System.Collections.Generic;
using System.Linq;
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
    public Transform prefab;
    public bool enableRandomScale = true;
    public float minScale;
    public float maxScale;
    public bool enableRandomFlip = true;

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

    public void createMany(List<Vector2> positions, Transform parent)
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
    public Color color = new Color(1f, 1f, 1f, 1f);

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
    public bool useAutoZDistance;
    public float zDistance;
}


[CreateAssetMenu(menuName = "MyAssets/ScriptableObjects/Level Generator")]
public class LevelGenerator : ScriptableObject
{
    [Header("Level")]
    [SerializeField] LevelSettings levelSettings;

    [Header("Container Paths")]
    [SerializeField] string proceduralElementsPath = "Environment/ProceduralElements";
    [SerializeField] string treeParentName = "TreeContainer";
    [SerializeField] string backgroundParentName = "SpriteLayers";
    [SerializeField] string lightShaftContainerName = "LightShafts";


    [Header("Factories")]
    [SerializeField] TreeFactory treeFactory;
    [SerializeField] VineFactory vineFactory;
    [SerializeField] LightShaftFactory lightShaftFactory;

    [Header("Background Layers")]
    [SerializeField] float zDistanceInterval = 5f;//any layer that doesn't have a set zDistance will have Abs(sortOrder) * zDistanceInterval applied
    [SerializeField] List<ProceduralLayer<ProceduralSpriteObject>> backgroundLayers = new List<ProceduralLayer<ProceduralSpriteObject>>();

    [Header("Prefabs")]
    [SerializeField] Transform winPlatformPrefab;
    [SerializeField] Transform blankParentPrefab; //Used to create empty parent container for individual spriteLayers

    Section levelSection;

    ProceduralLayerGenerator layerGenerator = new ProceduralLayerGenerator();

    private void InitLevelSection()
    {
        levelSection = new Section
        {
            startPos = levelSettings.startPos,
            length = RNG.RandomRange(levelSettings.levelLength),
            startOffset = levelSettings.globalStartOffset,
            endOffset = levelSettings.globalEndOffset
        };
    }

    private void InitFactories()
    {
        treeFactory.SetDefaultFactoryConfig(levelSettings.treeSettings);
        vineFactory.SetDefaultFactoryConfig(levelSettings.vineSettings);
        lightShaftFactory.SetLightShaftContainerParent(GameObject.Find(GetElementContainerPath(lightShaftContainerName)).transform);
    }

    private void DeInitFactories()
    {
        // Do any factory cleanup here
        lightShaftFactory.SetLightShaftContainerParent(null);
    }

    private void InitRNG()
    {
        if (levelSettings.rngSeed != -1) { RNG.SetSeed(levelSettings.rngSeed); }
    }

    public void GenerateLevel()
    {
        InitRNG();
        InitFactories();
        InitLevelSection();
        AddBackgroundLayerSection(levelSection);
        AddTreeLayerSection(levelSection);
        AddWinPlatform(new Vector2(levelSection.length, levelSettings.startPos.y));
        BatchLightShafts();
        DeInitFactories();
    }

    void AddTreeLayerSection(Section section)
    {
        Transform treeLayerParent = GameObject.Find(GetElementContainerPath(treeParentName)).transform;
        List<Vector2> positions = GeneratePositions(section, levelSettings.treeSpacing);
        foreach (Vector2 position in positions)
        {
            treeFactory.GenerateTree(position, treeLayerParent, null);
        }
        // StaticBatchingUtility.Combine(treeLayerParent.gameObject);
    }

    string GetElementContainerPath(string containerName)
    {
        return proceduralElementsPath + "/" + containerName;
    }

    void AddBackgroundLayerSection(Section section)
    {
        Transform layerParentContainer = GameObject.Find(GetElementContainerPath(backgroundParentName)).transform;

        Dictionary<string, int> sortLayerOrdering = new Dictionary<string, int>();

        for (int i = 0; i < backgroundLayers.Count; i++)
        {
            ProceduralLayer<ProceduralSpriteObject> layer = backgroundLayers[i];
            if (!layer.enabled) { continue; }

            //handle sortLayer and auto sortOrder
            string sortLayerName = layer.proceduralObject.sortLayerName;
            if (!sortLayerOrdering.ContainsKey(sortLayerName)) { sortLayerOrdering.Add(sortLayerName, 0); }
            else sortLayerOrdering[sortLayerName]--;
            layer.proceduralObject.sortOrder = sortLayerOrdering[sortLayerName];

            // Use Auto zDistance if enabled, otherwise use the layer's set zDistance
            float zDistance = layer.useAutoZDistance ? i * zDistanceInterval : layer.zDistance;

            //
            Transform layerParent = GameObject.Instantiate(blankParentPrefab, layerParentContainer);
            layerParent.name = layer.id;
            if (layer.enableParallax)
            {
                layerParent.gameObject.AddComponent<Paralaxer>();
                layerParent.position = new Vector3(layerParent.position.x, layerParent.position.y, zDistance);
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
            // StaticBatchingUtility.Combine(layerParent.gameObject);
        }
    }


    private void AddWinPlatform(Vector2 pos)
    {
        GameObject.Instantiate(winPlatformPrefab, pos, Quaternion.identity);
    }

    private void BatchLightShafts()
    {
        Transform lightShaftContainer = GameObject.Find(GetElementContainerPath(lightShaftContainerName)).transform;
        StaticBatchingUtility.Combine(lightShaftContainer.gameObject);
    }


    private List<Vector2> GeneratePositions(Section section, MinMax<float> spacing, float yOffset = 0)
    {
        if (spacing.min == 0 && spacing.max == 0)
        {
            Debug.LogError("Error: Incorrect Spacing values. \nminSpacing: " + spacing.min + " | maxSpacing: " + spacing.max);
            return null;
        }
        float startX = section.startPos.x;
        float endX = startX + section.length;
        float y = section.startPos.y + yOffset;
        float endOffset = section.endOffset;
        float startOffset = section.startOffset;

        List<Vector2> positions = new List<Vector2>();
        for (float x = startX + startOffset; x < endX + endOffset; x += RNG.RandomRange(spacing))
        {
            positions.Add(new Vector2(x, y));
        }
        return positions;
    }
}
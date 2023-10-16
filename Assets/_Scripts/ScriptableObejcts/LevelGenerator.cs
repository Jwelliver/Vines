using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class ProbWeightedItem<T>
{
    public T item;
    public float probability;
}

[Serializable]
public class ProbWeightItemList<T>
{
    public List<ProbWeightedItem<T>> items = new List<ProbWeightedItem<T>>();
    public T getRandomItem()
    {
        if (items.Count == 0) { return default; }//should return nullable value of T
        ProbWeightedItem<T> item = RNG.RandomChoice(items);
        if (RNG.SampleProbability(item.probability))
        {
            return item.item;
        }
        else
        {
            return getRandomItem();
        }
    }
}


// [Serializable]
// public class ProceduralObject
// {
//     public Transform prefab;
//     public bool enableRandomScale = true;
//     public float minScale;
//     public float maxScale;

//     public MinMax<float> rotation;
//     public bool enableRandomFlip = true;

//     public virtual void applyRandomScale(Transform obj)
//     {
//         if (!enableRandomScale) return;
//         float rndScale = RNG.RandomRange(minScale, maxScale);
//         obj.localScale = new Vector2(rndScale, rndScale);
//         return;
//     }

//     private void applyRandomRotation(Transform obj)
//     {
//         obj.localRotation = Quaternion.Euler(new Vector3(0, 0, obj.eulerAngles.z + RNG.RandomRange(rotation)));
//     }

//     private void applyRandomFlip(Transform obj)
//     {
//         if (enableRandomFlip && RNG.RandomBool())
//             obj.localScale = new Vector2(-obj.localScale.x, obj.localScale.y);
//         return;
//     }

//     public void createSingle(Vector2 position, Transform parent)
//     {
//         // Transform parent = parentRef ?? getParent();
//         if (parent == null) return;
//         Transform newObj = GameObject.Instantiate(prefab, position, Quaternion.identity, parent);
//         applyRandomScale(newObj);
//         applyRandomFlip(newObj);
//         applyRandomRotation(newObj);
//         objectSetup(newObj);
//     }

//     public void createMany(List<Vector2> positions, Transform parent)
//     {
//         // Transform parent = getParent();
//         if (parent == null) return;
//         foreach (Vector2 pos in positions)
//         {
//             createSingle(pos, parent);
//         }
//     }

//     public virtual void objectSetup(Transform obj)
//     {
//         return;
//     }
// }

// [Serializable]
// public class ProceduralSpriteObject : ProceduralObject
// {
//     public ProbWeightItemList<Sprite> weightedSprites;
//     public string sortLayerName = null;
//     public int sortOrder;
//     public Color color = new Color(1f, 1f, 1f, 1f);

//     SpriteRenderer getSpriteRenderer(Transform obj)
//     {
//         try
//         {
//             SpriteRenderer spriteRenderer = obj.gameObject.GetComponent<SpriteRenderer>();
//             return spriteRenderer;
//         }
//         catch (Exception e)
//         {
//             Debug.LogError("Error > cannot find sprite renderer on obj: " + obj.name + "\n Exception: " + e);
//             return null;
//         }
//     }

//     public override void applyRandomScale(Transform obj)
//     {
//         SpriteRenderer spriteRenderer = getSpriteRenderer(obj);
//         if (spriteRenderer == null || spriteRenderer.drawMode == SpriteDrawMode.Simple)
//         {
//             base.applyRandomScale(obj);
//             return;
//         }
//         float newScalar = RNG.RandomRange(minScale, maxScale);
//         Vector2 newScale = new Vector2(newScalar, newScalar);
//         spriteRenderer.size *= newScale; //TODO: verify this uniformly scales as expected
//         return;
//     }

//     public override void objectSetup(Transform obj)
//     {
//         SpriteRenderer spriteRenderer = getSpriteRenderer(obj);
//         if (spriteRenderer == null) { return; }
//         Sprite rndSprite = weightedSprites.getRandom();
//         spriteRenderer.sprite = rndSprite;
//         spriteRenderer.color = color;
//         spriteRenderer.sortingLayerName = sortLayerName;
//         spriteRenderer.sortingOrder = sortOrder;
//         return;
//     }
// }




// class ProceduralLayerGenerator
// {
//     private List<Vector2> getPositions(Section section, float minSpacing = 1, float maxSpacing = 3)
//     {
//         if (minSpacing == 0 && maxSpacing == 0 || maxSpacing < minSpacing)
//         {
//             Debug.LogError("Error: Incorrect Spacing values. \nminSpacing: " + minSpacing + " | maxSpacing: " + maxSpacing);
//             return null;
//         }
//         float startX = section.startPos.x;
//         float endX = startX + section.length;
//         float y = section.startPos.y;

//         List<Vector2> positions = new List<Vector2>();
//         for (float x = startX; x < endX; x += RNG.RandomRange(minSpacing, maxSpacing))
//         {
//             positions.Add(new Vector2(x, y));
//         }
//         return positions;
//     }

//     public void populateLayerSection<T>(ProceduralLayer<T> proceduralLayer, Section section, Transform parent) where T : ProceduralObject
//     {
//         if (proceduralLayer == null || proceduralLayer.proceduralObject == null || section == null)
//         {
//             Debug.LogError("Error: missing params");
//             return;
//         }
//         if (!proceduralLayer.enabled) { return; }
//         List<Vector2> positions = getPositions(section, proceduralLayer.minSpacing, proceduralLayer.maxSpacing);
//         List<Vector2> positionsWithObjectYOffset = new List<Vector2>();
//         foreach (Vector2 pos in positions)
//         {
//             positionsWithObjectYOffset.Add(pos + new Vector2(0, pos.y + RNG.RandomRange(proceduralLayer.objectYOffsetVariance)));
//         }
//         proceduralLayer.proceduralObject.createMany(positionsWithObjectYOffset, parent);
//     }
// }


// [Serializable]
// public class ProceduralLayer<T> where T : ProceduralObject
// {
//     public string id;
//     public bool enabled = true;
//     public T proceduralObject;
//     public float minSpacing;
//     public float maxSpacing;
//     public float yOffset;
//     public MinMax<float> objectYOffsetVariance;
//     public bool enableParallax;

//     [Header("Sorting")]
//     public bool useAutoSortOrder = true;
//     public int autoSortOrderStart = 0;
//     public bool useAutoZDistance;
//     public float zDistance;

//     [Header("SortingGroup")]
//     public bool useSortGroup;
//     public bool useObjectSortLayer;
//     public bool useObjectSortOrder;
//     public LayerMask sortGroupLayer;
//     public int sortGroupOrderInLayer;
// }


// public class EnvironmentLayerGenerator()
// {

//     public void GenerateLayer(EnvironmentLayer envLayer, List<Vector2> positions, Transform parent)
//     {

//     }
// }


[Serializable]
public class Section
{
    public Vector2 startPos;
    public int length;
    public Vector2 endPos => new Vector2(startPos.x + length, startPos.y);

    public Section Copy()
    {
        return new Section
        {
            startPos = startPos,
            length = length
        };
    }
}

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



public enum SectionFillType
{
    ALL,
    TREES_ONLY,
    BG_ONLY
}


/*
    todo 
    Reusable sections //? I'm really tired atm just trying to get this down; may sound like gibberish, hopefully you fill out the rest later. gl
        - Create a section that retains all objects in SectionElement
            - For env layers, save env id and envobj blueprint/config; whatever info is needed to pull from it and also reconstruct it if needed
                - each SectionEl can have id or index in the list for identification and matching between sections

        - When creating a new section, pull elements from the old section at random (from the same layerId) and place them in the new section instead of instantiating a new obj
            - update the element's positions list associated with the new section index, so whenever that section is being reconstructed from reusables, this object will be in the same location with the same properties

            - Add the section to a list of sections in the level gen, and we can track where the player is in each section, then keep adding on new sections with recycled obects


*/



[CreateAssetMenu(menuName = "MyAssets/ScriptableObjects/Level Generator")]
public class LevelGenerator : ScriptableObject
{
    [Header("Level")]
    public LevelSettings levelSettings;

    [Header("Container Paths")]
    [SerializeField] string proceduralElementsPath = "Environment/ProceduralElements";
    [SerializeField] string treeLayersContainerName = "TreeLayersContainer";
    [SerializeField] string backgroundParentName = "SpriteLayers";
    [SerializeField] string lightShaftContainerName = "LightShafts";

    [Header("Factories")]
    [SerializeField] EnvironmentObjectFactory environmentObjectFactory;
    [SerializeField] TreeFactory treeFactory;
    [SerializeField] VineFactory vineFactory;
    [SerializeField] LightShaftFactory lightShaftFactory;
    [SerializeField] SunspotFactory sunspotFactory;

    [Header("Background Layers")]
    [SerializeField] float zDistanceInterval = 5f;//any layer that doesn't have a set zDistance will have Abs(sortOrder) * zDistanceInterval applied
    [SerializeField] EnvironmentPreset environmentPreset;

    [Header("Prefabs")]
    [SerializeField] Transform startPlatformPrefab;
    [SerializeField] Transform winPlatformPrefab;
    [SerializeField] Transform blankParentPrefab; //Used to create empty parent container for individual spriteLayers
    [SerializeField] Transform debugSectionMarkerPrefab;



    Section currentSection;
    Transform startPlatformRef;
    Transform winPlatformRef;

    // EnvironmentLayerGenerator envLayerGenerator = new EnvironmentLayerGenerator();

    private void InitCurrentSection()
    {   // Run once on level start;

        currentSection = new Section
        {
            startPos = levelSettings.startPos,
            length = RNG.RandomRange(levelSettings.levelLength) * (int)levelSettings.direction
        };
        return;


        //TODO: implement
        // if(levelSettings.direction==LevelDirection.BOTH) {
        //     currentSection = new Section {

        //     }
        // }

    }

    private void InitFactories()
    {
        // treeFactory.SetDefaultFactoryConfig(levelSettings.treeSettings);
        // vineFactory.SetDefaultFactoryConfig(levelSettings.vineSettings);
        lightShaftFactory.SetLightShaftContainerParent(GameObject.Find(GetElementContainerPath(lightShaftContainerName)).transform);
    }

    private void DeInitFactories()
    {
        // Do any factory cleanup here
        lightShaftFactory.SetLightShaftContainerParent(null);
    }

    void OnDisable()
    {
        DeInitFactories();
        startPlatformRef = null;
        winPlatformRef = null;
    }

    private void InitRNG()
    {
        if (levelSettings.rngSeed != -1) { RNG.SetSeed(levelSettings.rngSeed); }
    }

    public void ReInit()
    {
        //Util function for quickly regenerating the level; Removes existing level components, and regenerates
        RemoveEnvironment();
        Debug.Log("Init New Level...");
        InitLevel();
    }

    private void RemoveEnvironment()
    {
        // Cleanup Env layers
        GameObject treeLayersContainer = GameObject.Find(GetElementContainerPath(treeLayersContainerName));
        GameObject spriteLayersContainer = GameObject.Find(GetElementContainerPath(backgroundParentName));
        GameObject lightShaftsContainer = GameObject.Find(GetElementContainerPath(lightShaftContainerName));
        Debug.Log("Removing Tree Layers...");
        foreach (Transform treeLayerParent in treeLayersContainer.transform)
        {
            Destroy(treeLayerParent.gameObject);
        }
        Debug.Log("Removing Env Sprite Layers...");
        foreach (Transform spriteLayer in spriteLayersContainer.transform)
        {
            Destroy(spriteLayer.gameObject);
        }
        Debug.Log("Removing Lightshafts...");
        foreach (Transform lightShaft in lightShaftsContainer.transform)
        {
            Destroy(lightShaft.gameObject);
        }
        Debug.Log("Removing Platforms...");
        // Cleanup platforms;
        if (startPlatformRef != null)
        {
            Destroy(startPlatformRef.gameObject);
        }
        if (winPlatformRef != null)
        {
            Destroy(winPlatformRef.gameObject);
        }
        Debug.Log("Cleanup Complete.");
    }

    private Section AddSectionOffset(Section section, int offsetLength, SectionFillType fillType)
    {
        // Adds a Section Offset to the given section; Negative offsetLength will prepend the new section before the given section, Positive will append the section to the given section, starting at section.endpos.
        Section offsetSection = section.Copy();
        if (offsetLength < 0) { offsetSection.startPos += new Vector2(offsetLength, 0); }
        else { offsetSection.startPos = section.endPos; }
        offsetSection.length = Math.Abs(offsetLength);
        GenerateSection(offsetSection, fillType);
        // Debug.Log("Section: " + section.startPos + " | " + section.length);
        // Debug.Log("Section Offset: " + offsetSection.startPos + " | " + offsetSection.length);
        return offsetSection;
    }

    public Section GetCurrentSection()
    {
        return currentSection;
    }

    public void InitLevel()
    {
        InitRNG(); // This Must be First;
        Debug.Log("Seed: " + RNG.GetCurrentSeed());
        InitFactories();
        if (levelSettings.levelType == LevelType.NORMAL)
        {
            InitNormalMode();
        }
        else if (levelSettings.levelType == LevelType.ENDLESS)
        {
            InitEndlessMode();
        }
    }

    void InitNormalMode()
    {
        // Set up current section
        InitCurrentSection();
        // // Add StartPlatform
        // AddStartPlatform(currentSection.startPos);
        // Add Win Platform (must do this before placing trees to ensure VineOverrideZones are found)
        AddWinPlatform(currentSection.endPos);
        // Prepend start BG section and Small Tree section
        AddSectionOffset(currentSection, -100, SectionFillType.BG_ONLY);
        AddSectionOffset(currentSection, -30, SectionFillType.TREES_ONLY);
        // Generate Main Section
        GenerateSection(currentSection, SectionFillType.ALL);
        // Append end BG section
        AddSectionOffset(currentSection, 100, SectionFillType.BG_ONLY);
        // BatchLightShafts();
        DeInitFactories();
    }

    void InitEndlessMode()
    {
        // Set up current section
        InitCurrentSection();
        // // Add StartPlatform
        // AddStartPlatform(currentSection.startPos);
        // Prepend start BG section
        AddSectionOffset(currentSection, -100, SectionFillType.ALL);
        // Generate First Section
        GenerateSection(currentSection, SectionFillType.ALL);
    }

    public void ExtendCurrentSection()
    {   // Used for Endless mode
        int extensionLength = 50; // * Don't go too low (i.e. below max treeSpacing), otherwise it affects treeSpacing
        //Create a SectionOffset and set currentSection to the new section that is returned
        currentSection = AddSectionOffset(currentSection, extensionLength, SectionFillType.ALL);
        // ! DEBUGING MARKER
        // GameObject.Instantiate(debugSectionMarkerPrefab, currentSection.startPos, Quaternion.identity);
    }

    void GenerateSection(Section section, SectionFillType fillType)
    {
        if (fillType == SectionFillType.ALL || fillType == SectionFillType.BG_ONLY)
        {
            AddEnvironmentLayerSection(section);
        }
        if (fillType == SectionFillType.ALL || fillType == SectionFillType.TREES_ONLY)
        {
            AddTreeLayerSection(section, levelSettings.treeLayers);
        }
        //TODO: Temp adding sunspots in all cases; Need to setup system that allows us to only gen background sunspots with env and default/tree sunspots with tree gen
        AddSunspotLayers(section);
    }

    void AddTreeLayerSection(Section section, TreeLayer treeLayer)
    {
        // Get treeLayersContainer
        Transform treeLayersContainer = GameObject.Find(GetElementContainerPath(treeLayersContainerName)).transform;
        // Find or Create a parent to contain this treeLayer and name it
        string treeLayerParentName = "TreeLayer " + treeLayer.layerIndex.ToString() + " " + treeLayer.id;
        Transform treeLayerParent = treeLayersContainer.Find(treeLayerParentName) ?? GameObject.Instantiate(blankParentPrefab, treeLayersContainer);
        treeLayerParent.position = new Vector2(0, 0);
        treeLayerParent.name = treeLayerParentName; // TODO: We're unecessarily renaming the tree if it already exists; refactor.
        // Generate Positions and generate a tree at each position
        List<Vector2> positions = GeneratePositions(section, treeLayer.spacing);
        int treeIndex = 0; //loop index tracker
        foreach (Vector2 position in positions)
        {// Since we're receiving a transform, we just name it here
            treeFactory.GenerateTree(position, treeLayerParent, treeLayer).name = "Tree." + treeLayer.layerIndex.ToString() + "." + treeIndex.ToString();
            treeIndex++;
        }
        InitManualTrees();
        // StaticBatchingUtility.Combine(treeLayerParent.gameObject);
    }

    void AddTreeLayerSection(Section section, List<TreeLayer> treeLayers)
    {
        for (int i = 0; i < treeLayers.Count; i++)
        {
            TreeLayer treeLayer = treeLayers[i];
            // * Assign the layerIndex here to be used by TreeFactory for appropriate sorting

            if (treeLayer.enabled)
            {
                treeLayer.layerIndex = i;
                AddTreeLayerSection(section, treeLayer);
            }
        }
    }

    string GetElementContainerPath(string containerName)
    {
        return proceduralElementsPath + "/" + containerName;
    }

    void AddEnvironmentLayerSection(Section section)
    {
        Transform layerParentContainer = GameObject.Find(GetElementContainerPath(backgroundParentName)).transform;

        Dictionary<string, int> sortLayerOrdering = new Dictionary<string, int>();

        for (int i = 0; i < environmentPreset.environmentLayers.Count; i++)
        {
            EnvironmentLayer layer = environmentPreset.environmentLayers[i];
            if (!layer.enabled) { continue; }

            //handle sortLayer and auto sortOrder
            string sortLayerName = layer.sortLayerName;
            // TODO: Test and ensure the sortOrder is being handled properly; Just replaced old condition from procObjlayer
            if (layer.autoSort == EnvLayerAutoSortType.StartFromMySortOrder || layer.autoSort == EnvLayerAutoSortType.Inherit)
            {
                if (!sortLayerOrdering.ContainsKey(sortLayerName)) { sortLayerOrdering.Add(sortLayerName, layer.sortOrder); }
                else sortLayerOrdering[sortLayerName]--;
                layer.sortOrder = sortLayerOrdering[sortLayerName];
            }

            // Use Auto zDistance if enabled, otherwise use the layer's set zDistance
            float zDistance = layer.useAutoZDistance ? i * zDistanceInterval : layer.zDistance;

            //
            Transform layerParent = layerParentContainer.Find(layer.id) ?? GameObject.Instantiate(blankParentPrefab, layerParentContainer);
            layerParent.name = layer.id;
            layerParent.position = new Vector3(0, 0);
            // Setup Optional SortingGroup
            if (layer.useSortGroup)
            {
                // Add SortingGorup component if it doesn't already exist;
                SortingGroup sortingGroup;
                bool hasSortingGroup = layerParent.gameObject.TryGetComponent<SortingGroup>(out sortingGroup);
                if (!hasSortingGroup) sortingGroup = layerParent.gameObject.AddComponent<SortingGroup>();
                // Setup the SortingGroup Layer;
                sortingGroup.sortingLayerName = sortLayerName;
                // Setup SortingGroup order
                sortingGroup.sortingOrder = layer.sortOrder;
            }
            if (layer.enableParallax)
            {
                Paralaxer paralaxer;
                bool hasParalaxer = layerParent.gameObject.TryGetComponent<Paralaxer>(out paralaxer);
                if (!hasParalaxer) paralaxer = layerParent.gameObject.AddComponent<Paralaxer>();
                layerParent.position = new Vector3(layerParent.position.x, layerParent.position.y, zDistance);
            }

            // Get Positions
            List<Vector2> positions = GeneratePositions(section, layer.spacing, layer.yOffset);

            // Get EnvObjFactoryConfig from layer
            EnvironmentObjectFactoryConfig environmentObjectFactoryConfig = layer.GetEnvironmentObjectFactoryConfig();



            // Generate objs
            for (int j = 0; j < positions.Count; j++)
            {
                Vector2 position = positions[j];
                // Apply yOffset variance
                position.y += RNG.RandomRange(-layer.yOffsetVariance, layer.yOffsetVariance);
                // Instantiate object
                Transform newEnvObj;
                if (layer.type == EnvLayerType.PrefabOnly)
                {
                    newEnvObj = environmentObjectFactory.InstantiateObj(position, layerParent, environmentObjectFactoryConfig, layer.prefab);
                }
                else if (layer.type == EnvLayerType.SpriteObj)
                {
                    newEnvObj = environmentObjectFactory.InstantiateSpriteObj(position, layerParent, environmentObjectFactoryConfig as EnvironmentSpriteObjectFactoryConfig, layer.prefab);
                }

            }
            // StaticBatchingUtility.Combine(layerParent.gameObject);
        }
    }

    void AddSunspotLayers(Section section)
    {
        //TODO: You will need to place sunspot layers at the same zdistance as env layers, or just child them to env layers so that parallax works correctly with the light
        //TODO: Refactor the parent and layer containers:
        // todo: ... Place primary SunspotLayers container inside the environment parent; Create an individual sub parent for each layer of sunspots
        // Instantiate parent
        Transform sunspotParent = GameObject.Instantiate(blankParentPrefab);
        sunspotParent.name = "SunspotLayers";

        foreach (SunspotBlueprint sunspotBlueprint in levelSettings.sunspots)
        {
            List<Vector2> positions = GeneratePositions(section, sunspotBlueprint.spacing, 0);
            foreach (Vector2 pos in positions)
            {
                Vector2 posWithYOffset = pos + new Vector2(0, RNG.RandomRange(sunspotBlueprint.yOffset));
                sunspotFactory.GenerateSunspot(posWithYOffset, sunspotBlueprint, sunspotParent);
            }


        }

    }

    private void AddStartPlatform(Vector2 pos)
    {
        startPlatformRef = GameObject.Instantiate(startPlatformPrefab, pos, Quaternion.identity);
    }

    private void AddWinPlatform(Vector2 pos)
    {
        winPlatformRef = GameObject.Instantiate(winPlatformPrefab, pos, Quaternion.identity);
    }

    private void InitManualTrees()
    { // Attempt to initialize manual trees in game; Note: Initing here instead of ManualTree.OnStart() to maintain consistent RNG order.
        ManualTree[] manualTrees = GameObject.FindObjectsOfType<ManualTree>();
        foreach (ManualTree tree in manualTrees)
        {
            tree.AttemptInit();
        }
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

        List<Vector2> positions = new List<Vector2>();
        for (float x = startX; x < endX; x += RNG.RandomRange(spacing))
        {
            positions.Add(new Vector2(x, y));
        }
        return positions;
    }
}



//==============================================================================
// BELOW: Orig before environmentLayer refactor 100523
//==============================================================================

// using System;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Rendering;

// [Serializable]
// public class ProbWeightedItem<T>
// {
//     public T item;
//     public float probability;
// }

// [Serializable]
// public class ProbWeightItemList<T>
// {
//     public List<ProbWeightedItem<T>> items = new List<ProbWeightedItem<T>>();
//     public T getRandom()
//     {
//         if (items.Count == 0) { return default; }//should return nullable value of T
//         ProbWeightedItem<T> item = RNG.RandomChoice(items);
//         if (RNG.SampleProbability(item.probability))
//         {
//             return item.item;
//         }
//         else
//         {
//             return getRandom();
//         }
//     }
// }


// [Serializable]
// public class ProceduralObject
// {
//     public Transform prefab;
//     public bool enableRandomScale = true;
//     public float minScale;
//     public float maxScale;

//     public MinMax<float> rotation;
//     public bool enableRandomFlip = true;

//     public virtual void applyRandomScale(Transform obj)
//     {
//         if (!enableRandomScale) return;
//         float rndScale = RNG.RandomRange(minScale, maxScale);
//         obj.localScale = new Vector2(rndScale, rndScale);
//         return;
//     }

//     private void applyRandomRotation(Transform obj)
//     {
//         obj.localRotation = Quaternion.Euler(new Vector3(0, 0, obj.eulerAngles.z + RNG.RandomRange(rotation)));
//     }

//     private void applyRandomFlip(Transform obj)
//     {
//         if (enableRandomFlip && RNG.RandomBool())
//             obj.localScale = new Vector2(-obj.localScale.x, obj.localScale.y);
//         return;
//     }

//     public void createSingle(Vector2 position, Transform parent)
//     {
//         // Transform parent = parentRef ?? getParent();
//         if (parent == null) return;
//         Transform newObj = GameObject.Instantiate(prefab, position, Quaternion.identity, parent);
//         applyRandomScale(newObj);
//         applyRandomFlip(newObj);
//         applyRandomRotation(newObj);
//         objectSetup(newObj);
//     }

//     public void createMany(List<Vector2> positions, Transform parent)
//     {
//         // Transform parent = getParent();
//         if (parent == null) return;
//         foreach (Vector2 pos in positions)
//         {
//             createSingle(pos, parent);
//         }
//     }

//     public virtual void objectSetup(Transform obj)
//     {
//         return;
//     }
// }

// [Serializable]
// public class ProceduralSpriteObject : ProceduralObject
// {
//     public ProbWeightItemList<Sprite> weightedSprites;
//     public string sortLayerName = null;
//     public int sortOrder;
//     public Color color = new Color(1f, 1f, 1f, 1f);

//     SpriteRenderer getSpriteRenderer(Transform obj)
//     {
//         try
//         {
//             SpriteRenderer spriteRenderer = obj.gameObject.GetComponent<SpriteRenderer>();
//             return spriteRenderer;
//         }
//         catch (Exception e)
//         {
//             Debug.LogError("Error > cannot find sprite renderer on obj: " + obj.name + "\n Exception: " + e);
//             return null;
//         }
//     }

//     public override void applyRandomScale(Transform obj)
//     {
//         SpriteRenderer spriteRenderer = getSpriteRenderer(obj);
//         if (spriteRenderer == null || spriteRenderer.drawMode == SpriteDrawMode.Simple)
//         {
//             base.applyRandomScale(obj);
//             return;
//         }
//         float newScalar = RNG.RandomRange(minScale, maxScale);
//         Vector2 newScale = new Vector2(newScalar, newScalar);
//         spriteRenderer.size *= newScale; //TODO: verify this uniformly scales as expected
//         return;
//     }

//     public override void objectSetup(Transform obj)
//     {
//         SpriteRenderer spriteRenderer = getSpriteRenderer(obj);
//         if (spriteRenderer == null) { return; }
//         Sprite rndSprite = weightedSprites.getRandom();
//         spriteRenderer.sprite = rndSprite;
//         spriteRenderer.color = color;
//         spriteRenderer.sortingLayerName = sortLayerName;
//         spriteRenderer.sortingOrder = sortOrder;
//         return;
//     }
// }




// class ProceduralLayerGenerator
// {
//     private List<Vector2> getPositions(Section section, float minSpacing = 1, float maxSpacing = 3)
//     {
//         if (minSpacing == 0 && maxSpacing == 0 || maxSpacing < minSpacing)
//         {
//             Debug.LogError("Error: Incorrect Spacing values. \nminSpacing: " + minSpacing + " | maxSpacing: " + maxSpacing);
//             return null;
//         }
//         float startX = section.startPos.x;
//         float endX = startX + section.length;
//         float y = section.startPos.y;

//         List<Vector2> positions = new List<Vector2>();
//         for (float x = startX; x < endX; x += RNG.RandomRange(minSpacing, maxSpacing))
//         {
//             positions.Add(new Vector2(x, y));
//         }
//         return positions;
//     }

//     public void populateLayerSection<T>(ProceduralLayer<T> proceduralLayer, Section section, Transform parent) where T : ProceduralObject
//     {
//         if (proceduralLayer == null || proceduralLayer.proceduralObject == null || section == null)
//         {
//             Debug.LogError("Error: missing params");
//             return;
//         }
//         if (!proceduralLayer.enabled) { return; }
//         List<Vector2> positions = getPositions(section, proceduralLayer.minSpacing, proceduralLayer.maxSpacing);
//         List<Vector2> positionsWithObjectYOffset = new List<Vector2>();
//         foreach (Vector2 pos in positions)
//         {
//             positionsWithObjectYOffset.Add(pos + new Vector2(0, pos.y + RNG.RandomRange(proceduralLayer.objectYOffsetVariance)));
//         }
//         proceduralLayer.proceduralObject.createMany(positionsWithObjectYOffset, parent);
//     }
// }


// [Serializable]
// public class ProceduralLayer<T> where T : ProceduralObject
// {
//     public string id;
//     public bool enabled = true;
//     public T proceduralObject;
//     public float minSpacing;
//     public float maxSpacing;
//     public float yOffset;
//     public MinMax<float> objectYOffsetVariance;
//     public bool enableParallax;

//     [Header("Sorting")]
//     public bool useAutoSortOrder = true;
//     public int autoSortOrderStart = 0;
//     public bool useAutoZDistance;
//     public float zDistance;

//     [Header("SortingGroup")]
//     public bool useSortGroup;
//     public bool useObjectSortLayer;
//     public bool useObjectSortOrder;
//     public LayerMask sortGroupLayer;
//     public int sortGroupOrderInLayer;
// }


// public class EnvironmentLayerGenerator() {

//     public void GenerateLayer(EnvironmentLayer envLayer, List<Vector2> positions, Transform parent) {

//     }
// }


// [Serializable]
// public class Section
// {
//     public Vector2 startPos;
//     public int length;
//     public Vector2 endPos => new Vector2(startPos.x + length, startPos.y);

//     public Section Copy()
//     {
//         return new Section
//         {
//             startPos = startPos,
//             length = length
//         };
//     }
// }

// [Serializable]
// public enum EnvLayerType {
//     SpriteObj,
//     PrefabOnly
// }

// [Serializable]
// public enum EnvLayerAutoSortType {
//     Disable,
//     StartFromMySortOrder,
//     Inherit
// }

// [Serializable]
// public class EnvironmentLayer
// {
//     [Header("Layer Properties")]
//     public string id;
//     public bool enabled = true;
//     [Header("Placement")]
//     public MinMax<float> spacing;
//     public float yOffset;
//     public float yOffsetVariance;
//     [Header("Parallax")]
//     public bool enableParallax;
//     public bool useAutoZDistance;
//     public float zDistance;
//     [Header("Object Properties")]
//     public EnvLayerType type;
//     public Transform prefab;
//     public MinMax<float> scaleModifier = new MinMax<float>(1f,1f);
//     public float maxRotation;
//     public bool enableRandomFlip;
//     public ProbWeightItemList<Sprite> sprites;
//     [Header("Sorting")]
//     public LayerMask sortLayer;
//     public int sortOrder;
//     public EnvLayerAutoSortType autoSort;
//     public Color layerTint = new Color(1f,1f,1f,1f);
//     [Header("SortingGroup")]
//     public bool useSortGroup;

//     // [SerializeReference] public EnvironmentObjectFactoryConfig factoryConfig;
// }



// public enum SectionFillType
// {
//     ALL,
//     TREES_ONLY,
//     BG_ONLY
// }


// /*
//     todo 
//     Reusable sections //? I'm really tired atm just trying to get this down; may sound like gibberish, hopefully you fill out the rest later. gl
//         - Create a section that retains all objects in SectionElement
//             - For env layers, save env id and envobj blueprint/config; whatever info is needed to pull from it and also reconstruct it if needed
//                 - each SectionEl can have id or index in the list for identification and matching between sections

//         - When creating a new section, pull elements from the old section at random (from the same layerId) and place them in the new section instead of instantiating a new obj
//             - update the element's positions list associated with the new section index, so whenever that section is being reconstructed from reusables, this object will be in the same location with the same properties

//             - Add the section to a list of sections in the level gen, and we can track where the player is in each section, then keep adding on new sections with recycled obects


// */



// [CreateAssetMenu(menuName = "MyAssets/ScriptableObjects/Level Generator")]
// public class LevelGenerator : ScriptableObject
// {
//     [Header("Level")]
//     public LevelSettings levelSettings;

//     [Header("Container Paths")]
//     [SerializeField] string proceduralElementsPath = "Environment/ProceduralElements";
//     [SerializeField] string treeLayersContainerName = "TreeLayersContainer";
//     [SerializeField] string backgroundParentName = "SpriteLayers";
//     [SerializeField] string lightShaftContainerName = "LightShafts";

//     [Header("Factories")]
//     [SerializeField] TreeFactory treeFactory;
//     [SerializeField] VineFactory vineFactory;
//     [SerializeField] LightShaftFactory lightShaftFactory;

//     [Header("Background Layers")]
//     [SerializeField] float zDistanceInterval = 5f;//any layer that doesn't have a set zDistance will have Abs(sortOrder) * zDistanceInterval applied
//     // [SerializeField] List<EnvironmentLayer> environmentLayers = new List<EnvironmentLayer>();
//     // [SerializeField] EnvironmentPreset environmentPreset;
//     [SerializeField] BackgroundPreset backgroundPreset;

//     [Header("Prefabs")]
//     [SerializeField] Transform startPlatformPrefab;
//     [SerializeField] Transform winPlatformPrefab;
//     [SerializeField] Transform blankParentPrefab; //Used to create empty parent container for individual spriteLayers
//     [SerializeField] Transform debugSectionMarkerPrefab;

//     Section currentSection;

//     // ProceduralLayerGenerator layerGenerator = new ProceduralLayerGenerator();
//     EnvironmentLayerGenerator envLayerGenerator = new EnvironmentLayerGenerator();

//     private void InitCurrentSection()
//     {   // Run once on level start;

//         currentSection = new Section
//         {
//             startPos = levelSettings.startPos,
//             length = RNG.RandomRange(levelSettings.levelLength) * (int)levelSettings.direction
//         };
//         return;


//         //TODO: implement
//         // if(levelSettings.direction==LevelDirection.BOTH) {
//         //     currentSection = new Section {

//         //     }
//         // }

//     }

//     private void InitFactories()
//     {
//         // treeFactory.SetDefaultFactoryConfig(levelSettings.treeSettings);
//         // vineFactory.SetDefaultFactoryConfig(levelSettings.vineSettings);
//         lightShaftFactory.SetLightShaftContainerParent(GameObject.Find(GetElementContainerPath(lightShaftContainerName)).transform);
//     }

//     private void DeInitFactories()
//     {
//         // Do any factory cleanup here
//         lightShaftFactory.SetLightShaftContainerParent(null);
//     }

//     void OnDisable()
//     {
//         DeInitFactories();
//     }

//     private void InitRNG()
//     {
//         if (levelSettings.rngSeed != -1) { RNG.SetSeed(levelSettings.rngSeed); }
//     }

//     private Section AddSectionOffset(Section section, int offsetLength, SectionFillType fillType)
//     {
//         // Adds a Section Offset to the given section; Negative offsetLength will prepend the new section before the given section, Positive will append the section to the given section, starting at section.endpos.
//         Section offsetSection = section.Copy();
//         if (offsetLength < 0) { offsetSection.startPos += new Vector2(offsetLength, 0); }
//         else { offsetSection.startPos = section.endPos; }
//         offsetSection.length = Math.Abs(offsetLength);
//         GenerateSection(offsetSection, fillType);
//         // Debug.Log("Section: " + section.startPos + " | " + section.length);
//         // Debug.Log("Section Offset: " + offsetSection.startPos + " | " + offsetSection.length);
//         return offsetSection;
//     }

//     public Section GetCurrentSection()
//     {
//         return currentSection;
//     }

//     public void InitLevel()
//     {
//         InitRNG(); // This Must be First;
//         Debug.Log("Seed: " + RNG.GetCurrentSeed());
//         InitFactories();
//         if (levelSettings.levelType == LevelType.NORMAL)
//         {
//             InitNormalMode();
//         }
//         else if (levelSettings.levelType == LevelType.ENDLESS)
//         {
//             InitEndlessMode();
//         }
//     }

//     void InitNormalMode()
//     {
//         // Set up current section
//         InitCurrentSection();
//         // // Add StartPlatform
//         // AddStartPlatform(currentSection.startPos);
//         // Add Win Platform (must do this before placing trees to ensure VineOverrideZones are found)
//         AddWinPlatform(currentSection.endPos);
//         // Prepend start BG section and Small Tree section
//         AddSectionOffset(currentSection, -100, SectionFillType.BG_ONLY);
//         AddSectionOffset(currentSection, -30, SectionFillType.TREES_ONLY);
//         // Generate Main Section
//         GenerateSection(currentSection, SectionFillType.ALL);
//         // Append end BG section
//         AddSectionOffset(currentSection, 100, SectionFillType.BG_ONLY);
//         BatchLightShafts();
//         DeInitFactories();
//     }

//     void InitEndlessMode()
//     {
//         // Set up current section
//         InitCurrentSection();
//         // // Add StartPlatform
//         // AddStartPlatform(currentSection.startPos);
//         // Prepend start BG section
//         AddSectionOffset(currentSection, -100, SectionFillType.ALL);
//         // Generate First Section
//         GenerateSection(currentSection, SectionFillType.ALL);
//     }

//     public void ExtendCurrentSection()
//     {   // Used for Endless mode
//         int extensionLength = 50; // * Don't go too low (i.e. below max treeSpacing), otherwise it affects treeSpacing
//         //Create a SectionOffset and set currentSection to the new section that is returned
//         currentSection = AddSectionOffset(currentSection, extensionLength, SectionFillType.ALL);
//         // ! DEBUGING MARKER
//         // GameObject.Instantiate(debugSectionMarkerPrefab, currentSection.startPos, Quaternion.identity);
//     }

//     void GenerateSection(Section section, SectionFillType fillType)
//     {
//         if (fillType == SectionFillType.ALL || fillType == SectionFillType.BG_ONLY)
//         {
//             AddBackgroundLayerSection(section);
//         }
//         if (fillType == SectionFillType.ALL || fillType == SectionFillType.TREES_ONLY)
//         {
//             AddTreeLayerSection(section, levelSettings.treeLayers);
//         }
//     }

//     void AddTreeLayerSection(Section section, TreeLayer treeLayer)
//     {
//         // Get treeLayersContainer
//         Transform treeLayersContainer = GameObject.Find(GetElementContainerPath(treeLayersContainerName)).transform;
//         // Find or Create a parent to contain this treeLayer and name it
//         string treeLayerParentName = "TreeLayer " + treeLayer.layerIndex.ToString() + " " + treeLayer.id;
//         Transform treeLayerParent = treeLayersContainer.Find(treeLayerParentName) ?? GameObject.Instantiate(blankParentPrefab, treeLayersContainer);
//         treeLayerParent.position = new Vector2(0, 0);
//         treeLayerParent.name = treeLayerParentName; // TODO: We're unecessarily renaming the tree if it already exists; refactor.
//         // Generate Positions and generate a tree at each position
//         List<Vector2> positions = GeneratePositions(section, treeLayer.spacing);
//         int treeIndex = 0; //loop index tracker
//         foreach (Vector2 position in positions)
//         {// Since we're receiving a transform, we just name it here
//             treeFactory.GenerateTree(position, treeLayerParent, treeLayer).name = "Tree." + treeLayer.layerIndex.ToString() + "." + treeIndex.ToString();
//             treeIndex++;
//         }
//         InitManualTrees();
//         // StaticBatchingUtility.Combine(treeLayerParent.gameObject);
//     }

//     void AddTreeLayerSection(Section section, List<TreeLayer> treeLayers)
//     {
//         for (int i = 0; i < treeLayers.Count; i++)
//         {
//             TreeLayer treeLayer = treeLayers[i];
//             // * Assign the layerIndex here to be used by TreeFactory for appropriate sorting

//             if (treeLayer.enabled)
//             {
//                 treeLayer.layerIndex = i;
//                 AddTreeLayerSection(section, treeLayer);
//             }
//         }
//     }

//     string GetElementContainerPath(string containerName)
//     {
//         return proceduralElementsPath + "/" + containerName;
//     }

//     void AddBackgroundLayerSection(Section section)
//     {
//         Transform layerParentContainer = GameObject.Find(GetElementContainerPath(backgroundParentName)).transform;

//         Dictionary<string, int> sortLayerOrdering = new Dictionary<string, int>();

//         for (int i = 0; i < backgroundPreset.backgroundLayers.Count; i++)
//         {
//             ProceduralLayer<ProceduralSpriteObject> layer = backgroundPreset.backgroundLayers[i];
//             if (!layer.enabled) { continue; }

//             //handle sortLayer and auto sortOrder
//             string sortLayerName = layer.proceduralObject.sortLayerName;
//             if (layer.useAutoSortOrder)
//             {
//                 if (!sortLayerOrdering.ContainsKey(sortLayerName)) { sortLayerOrdering.Add(sortLayerName, layer.autoSortOrderStart); }
//                 else sortLayerOrdering[sortLayerName]--;
//                 layer.proceduralObject.sortOrder = sortLayerOrdering[sortLayerName];
//             }

//             // Use Auto zDistance if enabled, otherwise use the layer's set zDistance
//             float zDistance = layer.useAutoZDistance ? i * zDistanceInterval : layer.zDistance;

//             //
//             Transform layerParent = layerParentContainer.Find(layer.id) ?? GameObject.Instantiate(blankParentPrefab, layerParentContainer);
//             layerParent.name = layer.id;
//             layerParent.position = new Vector3(0, 0);
//             // Setup Optional SortingGroup
//             if (layer.useSortGroup)
//             {
//                 // Add SortingGorup component
//                 SortingGroup sortingGroup = layerParent.gameObject.AddComponent<SortingGroup>();
//                 // Setup the SortingGroup Layer; If using objectSortLayer, apply to layerName, otherwise apply to sortLayerId from setting in layer; TODO: note: the object should also use the layerMask type instead of a string; update that and we can change to using a ternary here instead of fussing with either string or the id.
//                 if (layer.useObjectSortLayer) { sortingGroup.sortingLayerName = sortLayerName; }
//                 else { sortingGroup.sortingLayerID = layer.sortGroupLayer; }
//                 // Setup SortingGroup order
//                 sortingGroup.sortingOrder = layer.useObjectSortOrder ? sortLayerOrdering[sortLayerName] : layer.sortGroupOrderInLayer;
//             }
//             if (layer.enableParallax)
//             {
//                 layerParent.gameObject.AddComponent<Paralaxer>();
//                 layerParent.position = new Vector3(layerParent.position.x, layerParent.position.y, zDistance);
//             }

//             if (layer.yOffset != 0)
//             {
//                 Section newSection = section.Copy();
//                 newSection.startPos = new Vector2(section.startPos.x, section.startPos.y + layer.yOffset);
//                 layerGenerator.populateLayerSection(layer, newSection, layerParent);
//                 continue;
//             }
//             layerGenerator.populateLayerSection(layer, section, layerParent);
//             // StaticBatchingUtility.Combine(layerParent.gameObject);
//         }
//     }

//     private void AddStartPlatform(Vector2 pos)
//     {
//         GameObject.Instantiate(startPlatformPrefab, pos, Quaternion.identity);
//     }

//     private void AddWinPlatform(Vector2 pos)
//     {
//         GameObject.Instantiate(winPlatformPrefab, pos, Quaternion.identity);
//     }

//     private void InitManualTrees()
//     { // Attempt to initialize manual trees in game; Note: Initing here instead of ManualTree.OnStart() to maintain consistent RNG order.
//         ManualTree[] manualTrees = GameObject.FindObjectsOfType<ManualTree>();
//         foreach (ManualTree tree in manualTrees)
//         {
//             tree.AttemptInit();
//         }
//     }

//     private void BatchLightShafts()
//     {
//         Transform lightShaftContainer = GameObject.Find(GetElementContainerPath(lightShaftContainerName)).transform;
//         StaticBatchingUtility.Combine(lightShaftContainer.gameObject);
//     }

//     private List<Vector2> GeneratePositions(Section section, MinMax<float> spacing, float yOffset = 0)
//     {
//         if (spacing.min == 0 && spacing.max == 0)
//         {
//             Debug.LogError("Error: Incorrect Spacing values. \nminSpacing: " + spacing.min + " | maxSpacing: " + spacing.max);
//             return null;
//         }
//         float startX = section.startPos.x;
//         float endX = startX + section.length;
//         float y = section.startPos.y + yOffset;

//         List<Vector2> positions = new List<Vector2>();
//         for (float x = startX; x < endX; x += RNG.RandomRange(spacing))
//         {
//             positions.Add(new Vector2(x, y));
//         }
//         return positions;
//     }
// }
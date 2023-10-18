using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;





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



// [CreateAssetMenu(menuName = "MyAssets/ScriptableObjects/Level Generator")]
public class LevelGenerator : MonoBehaviour
{
    public static LevelGenerator Instance;
    [Header("Level")]
    public LevelSettings levelSettings;

    [Header("Container Paths")]
    [SerializeField] string proceduralElementsPath = "Environment/ProceduralElements";
    [SerializeField] string treeLayersContainerName = "TreeLayersContainer";
    [SerializeField] string backgroundParentName = "SpriteLayers";
    [SerializeField] string lightShaftContainerName = "LightShafts";

    [Header("Background Layers")]
    [SerializeField] float zDistanceInterval = 1f;//any layer that doesn't have a set zDistance will have Abs(sortOrder) * zDistanceInterval applied
    [SerializeField] EnvironmentPreset environmentPreset;

    [Header("Prefabs")]
    [SerializeField] Transform startPlatformPrefab;
    [SerializeField] Transform winPlatformPrefab;
    [SerializeField] Transform blankParentPrefab; //Used to create empty parent container for individual spriteLayers
    [SerializeField] Transform debugSectionMarkerPrefab;

    public static Action OnInitLevelGenComplete;

    Section currentSection;
    Transform startPlatformRef;
    Transform winPlatformRef;

    // EnvironmentLayerGenerator envLayerGenerator = new EnvironmentLayerGenerator();

    void Awake()
    {
        //Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

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
        LightShaftFactory.Instance.SetLightShaftContainerParent(GameObject.Find(GetElementContainerPath(lightShaftContainerName)).transform);
    }

    private void DeInitFactories()
    {
        // Do any factory cleanup here
        LightShaftFactory.Instance.SetLightShaftContainerParent(null);
    }


    void OnDestroy()
    {
        DeInitFactories();
        startPlatformRef = null;
        winPlatformRef = null;
        OnInitLevelGenComplete = null;
        Instance = null;
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
        // Set FF Time
        // Time.timeScale = 3;
        InitFactories();
        if (levelSettings.levelType == LevelType.NORMAL)
        {
            InitNormalMode();
        }
        else if (levelSettings.levelType == LevelType.ENDLESS)
        {
            InitEndlessMode();
        }
        // Invoke callback action if not null;
        // FinishInitLevelGen();
    }

    IEnumerator FinishInitLevelGen()
    {
        // Just waits some time before setting timeScale back to 1 and 
        yield return new WaitForSeconds(1);
        //Suspend All vine segments;
        VineSuspenseManager.SuspendAllNotVisible();

        //Reset TimeScale;
        Time.timeScale = 1;

        OnInitLevelGenComplete?.Invoke();
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
            TreeFactory.Instance.GenerateTree(position, treeLayerParent, treeLayer).name = "Tree." + treeLayer.layerIndex.ToString() + "." + treeIndex.ToString();
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
                    newEnvObj = EnvironmentObjectFactory.Instance.InstantiateObj(position, layerParent, environmentObjectFactoryConfig, layer.prefab);
                }
                else if (layer.type == EnvLayerType.SpriteObj)
                {
                    newEnvObj = EnvironmentObjectFactory.Instance.InstantiateSpriteObj(position, layerParent, environmentObjectFactoryConfig as EnvironmentSpriteObjectFactoryConfig, layer.prefab);
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
                SunspotFactory.Instance.GenerateSunspot(posWithYOffset, sunspotBlueprint, sunspotParent);
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
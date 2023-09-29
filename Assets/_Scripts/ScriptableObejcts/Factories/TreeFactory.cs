using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TreeFactoryConfig
{   // Contains config for TreeFactory

    [Header("Size")]
    public MinMax<float> treeScale = new MinMax<float>(1f, 1.25f);
    public MinMax<float> trunkHeight = new MinMax<float>(1.5f, 2.5f);
    public float maxTreeRotation = 5f;
    public float maxPalmRotation = 5f;

    // [Header("Sprite Sorting")]
    // public int baseTrunkSortOrder = -10;
    // public int basePalmSortOrder = 40;

    [Header("Vines")]
    public int maxVines = 4;

    [Header("Light Shafts")]
    public float pctChangeLightShaft = 0.3f;
    public int maxLightShafts = 2;
}

public class TreeConfig
{
    // Contains config for single tree (instance of randomly chosen settings from TreeFactoryConfig)
    public float treeScale;
    public float trunkHeight;
    public float treeAngleOffset;
    public float palmAngleOffset;
    public int palmSortOrder;
    public int trunkSortOrder;
    public int nVines;
    public int nLightShafts;
    public Transform rndPalmPrefab;
    public Sprite rndTrunkSprite;
    public VineFactoryConfig vineFactoryConfig; //optional; Using null will use VineFactory's default
}

public class NewTreeAssembly
{
    // Contains components used by TreeFactory to pass around as a new tree is configured and built
    public TreeConfig treeConfig;
    public Transform newTree;
    public RectTransform trunk;
    public RectTransform palmPrefabAnchor; //TODO: destroy after use.
    public Transform vinesContainer;
    public List<Vector2> palmAnchorPositions = new List<Vector2>();

    public NewTreeAssembly(Transform _newTree, TreeConfig _treeConfig)
    {
        newTree = _newTree;
        treeConfig = _treeConfig;
        trunk = (RectTransform)newTree.Find("Trunk");
        palmPrefabAnchor = (RectTransform)trunk.Find("PalmPrefabAnchor");
        vinesContainer = (RectTransform)newTree.Find("VinesContainer");
    }
}

[CreateAssetMenu(menuName = "MyAssets/ScriptableObjects/Factories/TreeFactory")]
public class TreeFactory : ScriptableObject
{
    [SerializeField] TreeFactoryConfig defaultFactoryConfig = new TreeFactoryConfig();
    [Header("Factories")]
    [SerializeField] VineFactory vineFactory;
    [SerializeField] LightShaftFactory lightShaftFactory;

    [Header("Prefabs")]
    [SerializeField] Transform treePrefab;
    [SerializeField] List<Transform> palmPrefabs;
    [SerializeField] List<Sprite> trunkSprites;
    [Header("TreeLayer Sort Settings")]

    // TODO: would like to have these in tree config, but then the range pool doesn't work if we are feeding the factory config every generate call; Maybe instead(or in addition to) using factoryConfig override, have levelgen set the default factory to use when building the entire layer.
    [SerializeField] int palmSortOrderBase = 40;
    [SerializeField] int palmSortOrderPoolLength = 10; //how many sortOrders on top of palmBaseSortOrders to use
    [SerializeField] int trunkSortOrderBase = -10;


    private List<int> palmSortOrderPool;

    public void SetDefaultFactoryConfig(TreeFactoryConfig newConfig)
    {
        defaultFactoryConfig = null;
        defaultFactoryConfig = newConfig;
    }

    TreeConfig GetRandomTreeConfig(TreeFactoryConfig factoryConfigOverride = null, VineFactoryConfig vineFactoryConfigOverride = null)
    {
        TreeFactoryConfig factoryConfig = factoryConfigOverride ?? defaultFactoryConfig;
        return new TreeConfig
        {
            treeScale = RNG.RandomRange(factoryConfig.treeScale),
            trunkHeight = RNG.RandomRange(factoryConfig.trunkHeight),
            treeAngleOffset = RNG.RandomRange(-factoryConfig.maxTreeRotation, factoryConfig.maxTreeRotation),
            palmAngleOffset = RNG.RandomRange(-factoryConfig.maxPalmRotation, factoryConfig.maxPalmRotation),
            palmSortOrder = GetPalmSortOrder(),
            trunkSortOrder = trunkSortOrderBase,
            nLightShafts = RNG.SampleOccurrences(factoryConfig.maxLightShafts, factoryConfig.pctChangeLightShaft),
            nVines = RNG.RandomRange(1, factoryConfig.maxVines),
            rndPalmPrefab = RNG.RandomChoice(palmPrefabs),
            rndTrunkSprite = RNG.RandomChoice(trunkSprites),
            vineFactoryConfig = vineFactoryConfigOverride
        };
    }

    public Transform GenerateTree(Vector2 position, Transform parent, TreeFactoryConfig factoryConfigOverride = null, VineFactoryConfig vineFactoryConfigOverride = null)
    {
        // Instantiate new tree prefab
        Transform newTree = GameObject.Instantiate(treePrefab, position, Quaternion.identity, parent);
        // Get new random TreeConfig
        TreeConfig newTreeConfig = GetRandomTreeConfig(factoryConfigOverride ?? defaultFactoryConfig, vineFactoryConfigOverride);
        // AssembleTree
        AssembleTree(newTree, newTreeConfig);
        // Return New Tree
        return newTree;
    }

    public Transform GenerateTree(Vector2 position, Transform parent, TreeLayer treeLayer)
    { // Generate a Tree from a given TreeLayer; Automatically handles layer sorting for trunk and palms

        // Instantiate new tree prefab
        Transform newTree = GameObject.Instantiate(treePrefab, position, Quaternion.identity, parent);
        // Get new random TreeConfig
        TreeConfig newTreeConfig = GetRandomTreeConfig(treeLayer.treeSettings, treeLayer.vineSettings);
        // Manually adjust sortorders for palm and trunk 
        newTreeConfig.palmSortOrder += palmSortOrderPoolLength * (treeLayer.layerIndex + 1); // ensure we get a unique range of length palmSortOrderPool length for each treeLayerIndex
        newTreeConfig.trunkSortOrder -= treeLayer.layerIndex;
        //Assemble
        AssembleTree(newTree, newTreeConfig);
        return newTree;
    }

    private NewTreeAssembly AssembleTree(Transform newTree, TreeConfig treeConfig)
    {
        // Setup new tree assembly
        NewTreeAssembly newTreeAssembly = new NewTreeAssembly(newTree, treeConfig);

        // Assemble New Tree
        InitTree(newTreeAssembly);
        InitPalms(newTreeAssembly);
        PopulateVines(newTreeAssembly);
        PopulateLightShafts(newTreeAssembly);
        return newTreeAssembly;
    }


    private void InitTree(NewTreeAssembly newTreeAssembly)
    {
        // Apply scale to tree
        float treeScale = newTreeAssembly.treeConfig.treeScale;
        newTreeAssembly.newTree.localScale = new Vector2(treeScale, treeScale);

        // Apply scale to trunk
        Transform trunk = newTreeAssembly.trunk;
        float trunkHeight = newTreeAssembly.treeConfig.trunkHeight;
        trunk.localScale = new Vector3(trunk.localScale.x, trunkHeight);

        // Assign random sprite
        SpriteRenderer trunkSpriteRenderer = trunk.GetComponent<SpriteRenderer>();
        trunkSpriteRenderer.sprite = newTreeAssembly.treeConfig.rndTrunkSprite;
        trunkSpriteRenderer.sortingOrder = newTreeAssembly.treeConfig.trunkSortOrder;

        // Apply random rotation
        newTreeAssembly.newTree.eulerAngles = Vector3.forward * newTreeAssembly.treeConfig.treeAngleOffset; //TODO: verify this is working; otherwise use Quaternion.Euler()
    }


    private int GetPalmSortOrder()
    {
        // If the pool is not initiated or is empty, rebuild it.
        if (palmSortOrderPool == null || palmSortOrderPool.Count == 0)
        {
            palmSortOrderPool = new List<int>();
            for (int i = 0; i < palmSortOrderPoolLength; i++)
            {
                palmSortOrderPool.Add(palmSortOrderBase + i);
            }
        }
        // Get Random Choice from PalmSortOrderPool
        int sortOrder = RNG.RandomChoice(palmSortOrderPool);
        // Remove the choice we just got;
        palmSortOrderPool.Remove(sortOrder);
        return sortOrder;
    }

    private void InitPalms(NewTreeAssembly newTreeAssembly)
    {
        // Setup Instantiation params for new palm
        Transform rndPalmPrefab = newTreeAssembly.treeConfig.rndPalmPrefab;

        // Apply position 
        Vector3 position = newTreeAssembly.palmPrefabAnchor.position;
        Transform newTree = newTreeAssembly.newTree;

        // Instantiate newPalm at newTree's palmPrefabAnchor position
        Transform newPalm = Instantiate(rndPalmPrefab, position, Quaternion.identity, newTree);

        // Set Palm name
        newPalm.name = "Palm";

        // Apply Random SortOrder to prevent overlap flickering
        newPalm.GetComponent<SpriteRenderer>().sortingOrder = newTreeAssembly.treeConfig.palmSortOrder;

        // Apply random rotation
        newPalm.eulerAngles = Vector3.forward * newTreeAssembly.treeConfig.palmAngleOffset;

        // Get Palm AnchorContainer (children of which represent where vines and lightshafts can be placed)
        Transform palmAnchorContainer = newPalm.Find("Anchors");
        if (palmAnchorContainer == null)
        {
            Debug.LogError("Palm Anchor not found in tree " + name + " palmName: " + newPalm.name);
            return;
        }
        // Add all anchor positions to palmAnchorPositions
        foreach (Transform anchor in palmAnchorContainer)
        {
            newTreeAssembly.palmAnchorPositions.Add(anchor.position);
        }

        // Destroy anchor container now that we don't need it
        Destroy(palmAnchorContainer.gameObject);
    }


    private void PopulateVines(NewTreeAssembly newTreeAssembly, VineFactoryConfig vineFactoryConfigOverride = null)
    {
        Transform vinesContainer = newTreeAssembly.vinesContainer;
        for (int i = 0; i < newTreeAssembly.treeConfig.nVines; i++)
        {
            Vector2 rndPosition = RNG.RandomChoice(newTreeAssembly.palmAnchorPositions);
            vineFactory.GenerateVine(rndPosition, vinesContainer, vineFactoryConfigOverride ?? newTreeAssembly.treeConfig.vineFactoryConfig);
        }
    }

    private void PopulateLightShafts(NewTreeAssembly newTreeAssembly)
    {
        for (int i = 0; i < newTreeAssembly.treeConfig.nLightShafts; i++)
        {
            Vector2 rndPosition = RNG.RandomChoice(newTreeAssembly.palmAnchorPositions);
            lightShaftFactory.GenerateLightShaft(rndPosition);
        }
    }
}
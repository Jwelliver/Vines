using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TreeFactoryConfig
{   // Contains config for TreeFactory

    [Header("Size")]
    public MinMax<float> treeScale = new MinMax<float>(1f, 1.25f);
    public MinMax<float> trunkHeight = new MinMax<float>(1.5f, 2.5f);
    public float maxAngle = 5f;

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
    public float angle;
    public int nVines;
    public int nLightShafts;
    public Transform rndPalmPrefab;
    public Sprite rndTrunkSprite;
}

public class NewTreeAssembly
{
    // Contains components used by TreeFactory to pass around as a new tree is configured and built
    public TreeConfig treeConfig;
    public Transform newTree;
    public RectTransform trunk;
    public RectTransform palmPrefabAnchor;
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
    [SerializeField] Transform treePrefab;
    [SerializeField] VineFactory vineFactory;
    [SerializeField] LightShaftFactory lightShaftFactory;
    [SerializeField] List<Transform> palmPrefabs;
    [SerializeField] List<Sprite> trunkSprites;

    public void SetDefaultFactoryConfig(TreeFactoryConfig newConfig)
    {
        defaultFactoryConfig = null;
        defaultFactoryConfig = newConfig;
    }

    TreeConfig GetRandomTreeConfig(TreeFactoryConfig factoryConfigOverride = null)
    {
        TreeFactoryConfig factoryConfig = factoryConfigOverride ?? defaultFactoryConfig;
        return new TreeConfig
        {
            treeScale = RNG.RandomRange(factoryConfig.treeScale),
            trunkHeight = RNG.RandomRange(factoryConfig.trunkHeight),
            angle = RNG.RandomRange(-factoryConfig.maxAngle, factoryConfig.maxAngle),
            nLightShafts = RNG.SampleOccurrences(factoryConfig.maxLightShafts, factoryConfig.pctChangeLightShaft),
            nVines = RNG.RandomRange(1, factoryConfig.maxVines),
            rndPalmPrefab = RNG.RandomChoice(palmPrefabs),
            rndTrunkSprite = RNG.RandomChoice(trunkSprites)
        };
    }

    public Transform GenerateTree(Vector2 position, Transform parent, TreeFactoryConfig factoryConfigOverride = null)
    {
        // Instantiate new tree prefab
        Transform newTree = GameObject.Instantiate(treePrefab, position, Quaternion.identity, parent);
        // Get new random TreeConfig
        TreeConfig newTreeConfig = GetRandomTreeConfig(factoryConfigOverride ?? defaultFactoryConfig);
        // Setup new tree assembly
        NewTreeAssembly newTreeAssembly = new NewTreeAssembly(newTree, newTreeConfig);
        // Assemble New Tree
        InitTree(newTreeAssembly);
        InitPalms(newTreeAssembly);
        PopulateVines(newTreeAssembly);
        PopulateLightShafts(newTreeAssembly);
        // Destroy Assembly Objects
        newTreeAssembly = null;
        newTreeConfig = null;
        // Return New Tree
        return newTree;
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

        // Apply random rotation
        newTreeAssembly.newTree.eulerAngles = Vector3.forward * newTreeAssembly.treeConfig.angle; //TODO: verify this is working; otherwise use Quaternion.Euler()
    }

    private void InitPalms(NewTreeAssembly newTreeAssembly)
    {
        // Setup Instantiation params for new palm
        Transform rndPalmPrefab = newTreeAssembly.treeConfig.rndPalmPrefab;
        Vector2 position = newTreeAssembly.palmPrefabAnchor.position;
        Transform newTree = newTreeAssembly.newTree;

        // Instantiate newPalm at newTree's palmPrefabAnchor position
        Transform newPalm = Instantiate(rndPalmPrefab, position, Quaternion.identity, newTree);

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


    private void PopulateVines(NewTreeAssembly newTreeAssembly)
    {
        Transform vinesContainer = newTreeAssembly.vinesContainer;
        for (int i = 0; i < newTreeAssembly.treeConfig.nVines; i++)
        {
            Vector2 rndPosition = RNG.RandomChoice(newTreeAssembly.palmAnchorPositions);
            vineFactory.GenerateVine(rndPosition, vinesContainer);
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
using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class LevelGenerator : ScriptableObject
{
    [Header("Level")]
    [SerializeField] string proceduralLevelContainerName = "ProceduralLevelContainer";
    [SerializeField] int levelLengthMin = 50;
    [SerializeField] int levelLengthMax = 50;
    [SerializeField] int levelEdgeOffset = 0;

    [Header("Trees")]
    // [SerializeField] Transform treeContainer;
    [SerializeField] RectTransform tree;
    [SerializeField] int minDistanceBetweenTrees = 3;
    [SerializeField] int maxDistanceBetweenTrees = 5;

    [Header("Background Layers")]
    [SerializeField] List<ProceduralBackground> foliageLayers = new List<ProceduralBackground>();
    [SerializeField] List<ProceduralBackground> paralaxLayers = new List<ProceduralBackground>();

    [Header("Objects")]

    [SerializeField] Transform winPlatform;

    int levelLength;


    // void createProceduralParent() {
    //     GameObject parent = new GameObject("Procedural_SO");
    // }

    //int levelLength, int levelStartOffset, int minTreeDistance, int maxTreeDistance
    public void generateLevel()
    {
        levelLength = getRandomLevelLength();
        // populateBackground();
        populateTrees();
        generateWinPlatform();
    }

    int getRandomLevelLength()
    {
        return UnityEngine.Random.Range(levelLengthMin, levelLengthMax);
    }
    void populateTrees()
    {
        string treeContainerPath = proceduralLevelContainerName + "/" + "TreeContainer";
        Transform treeContainer = GameObject.Find(treeContainerPath).transform;
        for (int i = -Math.Abs(levelEdgeOffset); i < levelLength + Math.Abs(levelEdgeOffset); i += UnityEngine.Random.Range(minDistanceBetweenTrees, maxDistanceBetweenTrees))
        {
            GameObject.Instantiate(tree, new Vector2(i, 0), Quaternion.identity, treeContainer);
        }
    }

    void populateBackground()
    {
        //populate foliage
        foreach (ProceduralBackground i in foliageLayers)
        {
            i.populateObjects(levelLength, levelEdgeOffset);
        }
        foreach (ProceduralBackground i in paralaxLayers)
        {
            i.populateObjects(levelLength, levelEdgeOffset);
        }

    }

    void generateWinPlatform()
    {
        GameObject.Instantiate(winPlatform, new Vector2(levelLength, -1.5f), Quaternion.identity);
    }

}

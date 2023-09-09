using System;
using UnityEditor.PackageManager;
using UnityEngine;

[CreateAssetMenu]
public class LevelGenerator : ScriptableObject
{
    [Header("Level")]
    [SerializeField] string proceduralLevelContainerName = "ProceduralLevelContainer";
    [SerializeField] int levelLength = 50;
    [SerializeField] int levelStartOffset = 0;

    [Header("Trees")]
    // [SerializeField] Transform treeContainer;
    [SerializeField] RectTransform tree;
    [SerializeField] int minDistanceBetweenTrees = 3;
    [SerializeField] int maxDistanceBetweenTrees = 5;

    [Header("Objects")]

    [SerializeField] Transform winPlatform;

    // void createProceduralParent() {
    //     GameObject parent = new GameObject("Procedural_SO");
    // }

    //int levelLength, int levelStartOffset, int minTreeDistance, int maxTreeDistance
    public void generateLevel() {
        populateTrees();
        generateWinPlatform();
    }

    void populateTrees() {
        string treeContainerPath = proceduralLevelContainerName + "/" + "TreeContainer";
        Transform treeContainer = GameObject.Find(treeContainerPath).transform;
        for(int i=levelStartOffset; i<levelLength+levelStartOffset;i+=UnityEngine.Random.Range(minDistanceBetweenTrees,maxDistanceBetweenTrees)) { 
            GameObject.Instantiate(tree, new Vector2(i, 0),Quaternion.identity,treeContainer);
        }
    }

    void generateWinPlatform() {
        GameObject.Instantiate(winPlatform, new Vector2(levelLength,-1.5f), Quaternion.identity);
    }

}

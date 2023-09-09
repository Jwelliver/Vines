using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Level")]
    [SerializeField] int levelLength = 50;
    [SerializeField] int levelStartOffset = 0;

    [Header("Trees")]
    [SerializeField] Transform treeContainer;
    [SerializeField] RectTransform tree;
    [SerializeField] int minDistanceBetweenTrees = 3;
    [SerializeField] int maxDistanceBetweenTrees = 5;

    [Header("Objects")]

    [SerializeField] Transform winPlatform;


    // Start is called before the first frame update
    void Start()
    {
        generateLevel();
    }

    void generateLevel() {
        populateTrees();
        generateWinPlatform();
    }

    void populateTrees() {
        for(int i=levelStartOffset; i<levelLength+levelStartOffset;i+=Random.Range(minDistanceBetweenTrees,maxDistanceBetweenTrees)) { 
            GameObject.Instantiate(tree, new Vector2(i, 0),Quaternion.identity,treeContainer);
        }
    }

    void generateWinPlatform() {
        GameObject.Instantiate(winPlatform, new Vector2(levelLength,-1.5f), Quaternion.identity);
    }

}

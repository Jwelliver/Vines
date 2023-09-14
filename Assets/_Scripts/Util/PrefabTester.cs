/**
091323
Quick script to use in a blank scene to iterate on a single prefab by deleting and recreating a new version 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabTester : MonoBehaviour
{
    [SerializeField] Transform prefabToTest;
    [SerializeField] KeyCode destroyAndCreateNew = KeyCode.D;
    [SerializeField] KeyCode disableAndCreateNew = KeyCode.Space;
    [SerializeField] bool loadOnStart = true;
    [SerializeField] Vector3 loadPosition = new Vector3(0, 0, 0);

    private Transform currentPrefabInstance;


    void Start()
    {
        if (loadOnStart)
        {
            createNewInstance();
        }
    }

    void createNewInstance()
    {
        if (prefabToTest == null) { return; }
        currentPrefabInstance = GameObject.Instantiate(prefabToTest, loadPosition, Quaternion.identity);
    }

    void destroyCurrentPrefab()
    {
        if (currentPrefabInstance == null) { return; }
        GameObject.Destroy(currentPrefabInstance.gameObject);
    }

    void disableCurrentPrefab()
    {
        if (currentPrefabInstance == null) { return; }
        currentPrefabInstance.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(disableAndCreateNew))
        {
            disableCurrentPrefab();
            createNewInstance();
        }
        if (Input.GetKeyDown(destroyAndCreateNew))
        {
            destroyCurrentPrefab();
            createNewInstance();
        }
    }
}

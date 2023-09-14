using System;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SpriteWithProbability
{
    public Sprite sprite;
    public float probability;
}

[CreateAssetMenu(menuName = "MyAssets/ScriptableObjects/Procedural Background")]
public class ProceduralBackground : ScriptableObject
{
    [SerializeField] string parentTransformPath;
    [Header("Objects")]
    [SerializeField] Transform prefab;
    [SerializeField] List<SpriteWithProbability> sprites = new List<SpriteWithProbability>();
    [SerializeField] int minDistanceBetweenObjs = 3;
    [SerializeField] int maxDistanceBetweenObjs = 5;
    [SerializeField] float minScale = 3f;
    [SerializeField] float maxScale = 5f;

    [Header("Layer")]
    [SerializeField] string sortLayerName;
    [SerializeField] int sortOrder = 0;
    [SerializeField] Color color = Color.white;

    public int nObjects = 0;


    private Sprite getRandomSprite()
    {
        SpriteWithProbability testSprite = sprites[UnityEngine.Random.Range(0, sprites.Count)];
        if (UnityEngine.Random.Range(0f, 1f) < testSprite.probability)
        {
            return testSprite.sprite;
        }
        else
        {
            return getRandomSprite();
        }
    }

    public void staticBatchLayer(Transform parent)
    {
        StaticBatchingUtility.Combine(parent.gameObject);
    }


    public void populateObjects(int levelLength, int levelEdgeOffset)
    {
        Transform parent;
        try
        {
            parent = GameObject.Find(parentTransformPath).transform;
        }
        catch (Exception e)
        {
            Debug.LogError("Error finding parent: " + e);
            return;
        }

        for (int i = -Math.Abs(levelEdgeOffset); i < levelLength + Mathf.Abs(levelEdgeOffset); i += UnityEngine.Random.Range(minDistanceBetweenObjs, maxDistanceBetweenObjs))
        {
            Transform newObj = GameObject.Instantiate(prefab, new Vector2(i, parent.position.y), Quaternion.identity);
            SpriteRenderer newObjSpriteRenderer = newObj.GetComponent<SpriteRenderer>();
            Sprite rndSprite = getRandomSprite();
            newObjSpriteRenderer.sprite = rndSprite;
            newObjSpriteRenderer.color = color;
            newObjSpriteRenderer.sortingLayerName = sortLayerName;
            newObjSpriteRenderer.sortingOrder = sortOrder;
            float rndScale = RNG.RandomRange(minScale, maxScale);
            newObj.localScale = new Vector2(rndScale, rndScale);
            bool isFlipped = RNG.RandomBool();
            if (isFlipped)
            {
                newObj.localScale = new Vector2(-newObj.localScale.x, newObj.localScale.y);
            }
            newObj.SetParent(parent);
            nObjects++;
        }
        // staticBatchLayer(parent);
    }
}


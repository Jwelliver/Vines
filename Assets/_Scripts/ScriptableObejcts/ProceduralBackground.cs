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
        SpriteWithProbability testSprite = RNG.RandomChoice(sprites);
        if (RNG.SampleProbability(testSprite.probability))
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

        for (int i = -Math.Abs(levelEdgeOffset); i < levelLength + Mathf.Abs(levelEdgeOffset); i += RNG.RandomRange(minDistanceBetweenObjs, maxDistanceBetweenObjs))
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
            if (RNG.RandomBool()) //random isFlipped
            {
                newObj.localScale = new Vector2(-newObj.localScale.x, newObj.localScale.y);
            }
            newObj.SetParent(parent);
            nObjects++;
        }
        // staticBatchLayer(parent);
    }
}

//=============================================
//0915123 background layer level gen revamp; Simple version
//============================================


/*

class TransformSettings
    * settings to be applied to a single procedural object
    - Contains:
        - scale
        - position
        - isFlipped    

static class TransformSettingsGenerator
    // - generate(Transfrom prefab, Section section, Transform? parent)
    //    * instantiates a layout of prefabs 

    - TransformSettings generateSample(ProceduralLayoutParams params)
        * returns a single TransformSettings from params
    - TransformSettings[] generateSection(ProceduralLayoutParams params, Section section)
        * returns an array of TransformSettings across a given section

ProceduralObjectLayout
    * describes how a prefab will be generated across a given section
    - Contains:
        - Prefab
        - minSpacing, maxSpacing
        - minScale, maxScale
        - enableRandomFlip


ProceduralSpriteLayout: ProceduralObjectLayout
    * a proceduralObjectLayout for sprite objects (eg. used for background layouts)
    - Contains:
        - prefab
        - ProbabilityWeightedSpritePool
        - ProceduralLayout
        - Layername
        - Color


BackgroundLayerGroup
    - Contains:
        - ParentPath
            * primary path for all layers
        - LayerList:
            - LayerName
            - List<ProceduralSpritePool>
        


*/



// public struct TransformSettings
// {
//     public Vector3 scale;
//     public Vector3 position;
//     public bool isFlipped;
// }







// public class ProceduralObjectLayer
// {

// }

// public class ProceduralBackgroundLayer
// {
//     public string id;
//     public Vector3 paralaxSpeed = new Vector3(0, 0, 0);
//     public Color color = new Color(255, 255, 255);
//     public ProceduralSpriteObjectLayout layout;

//     // public Transform CreateLayoutParentContainer() {
//     // }
// }


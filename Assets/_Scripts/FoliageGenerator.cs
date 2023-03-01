using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpriteWithProbability {
    public Sprite sprite;
    public float probability;
}

public class FoliageGenerator : MonoBehaviour
{

    [Header("Size")]
    [SerializeField] int levelLength = 50;
    [SerializeField] int levelStartOffset = 0;

    [Header("Objects")]

    [SerializeField] RectTransform foliageObj;
    [SerializeField] List<SpriteWithProbability> foliageSprites = new List<SpriteWithProbability>();
    [SerializeField] int minDistanceBetweenObjs = 3;
    [SerializeField] int maxDistanceBetweenObjs = 5;
    [SerializeField] float minScale = 3f;
    [SerializeField] float maxScale = 5f;

    [Header("Layer")]
    [SerializeField] string sortLayerName;
    [SerializeField] int sortOrder = 0;



    // Start is called before the first frame update
    void Start()
    {
        populateParalaxObjects();
    }

    Sprite getRandomSprite(){
        SpriteWithProbability testSprite = foliageSprites[Random.Range(0,foliageSprites.Count)];
        if(Random.Range(0f,1f)<testSprite.probability) {
            return testSprite.sprite;
        } else {
            return getRandomSprite();
        }
    }

    void populateParalaxObjects() {
        for(int i=levelStartOffset; i<levelLength+levelStartOffset;i+=Random.Range(minDistanceBetweenObjs,maxDistanceBetweenObjs)) { 
            Transform newObj = GameObject.Instantiate(foliageObj, new Vector2(i, transform.position.y),Quaternion.identity);
            SpriteRenderer newObjSpriteRenderer = newObj.GetComponent<SpriteRenderer>();
            // Sprite rndSprite = foliageSprites;
            Sprite rndSprite = getRandomSprite();
            // Sprite rndSprite = foliageSprites.Keys[Random.Range(0,foliageSprites.Count)];
            newObjSpriteRenderer.sprite = rndSprite;
            // newObjSpriteRenderer.color = color;
            newObjSpriteRenderer.sortingLayerName = sortLayerName;
            newObjSpriteRenderer.sortingOrder = sortOrder;
            float rndScale = Random.Range(minScale,maxScale);
            newObj.localScale = new Vector2(rndScale,rndScale);
            bool isFlipped = Random.Range(0f,1f) < 0.5f;
            if(isFlipped) {
                newObj.localScale = new Vector2(-newObj.localScale.x, newObj.localScale.y);
            }
            newObj.SetParent(transform);
        }
    }
}

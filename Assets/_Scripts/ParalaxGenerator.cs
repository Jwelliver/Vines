using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParalaxGenerator : MonoBehaviour
{
    [Header("Size")]
    [SerializeField] int levelLength = 50;
    [SerializeField] int levelStartOffset = 0;

    [Header("Objects")]

    [SerializeField] RectTransform paralaxObject;
    [SerializeField] List<Sprite> paralaxObjSprites = new List<Sprite>();
    [SerializeField] Color color = Color.black;
    [SerializeField] int minDistanceBetweenTrees = 3;
    [SerializeField] int maxDistanceBetweenTrees = 5;
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

    void populateParalaxObjects() {
        for(int i=levelStartOffset; i<levelLength+levelStartOffset;i+=Random.Range(minDistanceBetweenTrees,maxDistanceBetweenTrees)) { 
            Transform newObj = GameObject.Instantiate(paralaxObject, new Vector2(i, transform.position.y),Quaternion.identity);
            SpriteRenderer newObjSpriteRenderer = newObj.GetComponent<SpriteRenderer>();
            Sprite rndSprite = paralaxObjSprites[Random.Range(0,paralaxObjSprites.Count)];
            newObjSpriteRenderer.sprite = rndSprite;
            newObjSpriteRenderer.color = color;
            newObjSpriteRenderer.sortingLayerName = sortLayerName;
            newObjSpriteRenderer.sortingOrder = sortOrder;
            float rndScale = Random.Range(minScale,maxScale);
            newObj.localScale = new Vector2(rndScale,rndScale);
            newObj.SetParent(transform);
        }
    }
}

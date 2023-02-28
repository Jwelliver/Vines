using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralTree : MonoBehaviour
{
    // [SerializeField] int minHeight = 10;
    // [SerializeField] int maxHeight = 15;
    // [SerializeField] float minTrunkWidth = 1f;
    // [SerializeField] float maxTrunkWidth = 4f;

    [SerializeField] float minScale = 1f;
    [SerializeField] float maxScale = 3f;

    [SerializeField] float maxAngle = 3f;

    [SerializeField] int maxVines = 3;

    [SerializeField] float pctChanceLightShaft = 0.5f;
    [SerializeField] int maxLightShafts = 3;

    [SerializeField] float distanceToDeactivate = 20f;

    [SerializeField] RectTransform trunk;
    [SerializeField] RectTransform palms;
    [SerializeField] Transform vine;
    [SerializeField] Transform lightShaft;
    [SerializeField] List<Sprite> palmSprites;
    [SerializeField] List<Sprite> trunkSprites;
    
    
    bool isFlipped;

    Transform player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").transform;
        initTree();
        populateVines();
        populateLightShafts();
    }

    void Update() {
        checkDistanceFromPlayer();
    }

    void checkDistanceFromPlayer() {
        float distance = Vector2.Distance(transform.position, player.position);
        if(distance>distanceToDeactivate) {
            trunk.gameObject.SetActive(false);
        } else {
            trunk.gameObject.SetActive(true);
        }
    }

    void initTree() {
        float rndScale = Random.Range(minScale,maxScale);
        transform.localScale = new Vector2(rndScale,rndScale);

        // Assign random sprites
        trunk.GetComponent<SpriteRenderer>().sprite = getRandomSprite(trunkSprites);
        palms.GetComponent<SpriteRenderer>().sprite = getRandomSprite(palmSprites);

        //randomly flip transform -- had to comment out since lightshafts will also change dirs; come back if you can
        // if(Random.Range(0f,1f)<0.5) {
        //     transform.localScale = new Vector3(-1f, 1f, 1f);
        //     isFlipped = true;
        // }
        // else {
        //     transform.localScale = new Vector3(1f, 1f, 1f);
        // }

        transform.eulerAngles = Vector3.forward * Random.Range(0,maxAngle);
    }

    Sprite getRandomSprite(List<Sprite> options) {
        return options[Random.Range(0,options.Count)];
    }

    void populateVines() {
        int nVines = Random.Range(1,maxVines);
        for(int i =0; i<nVines; i++) {
            // Vector2 newVinePosition = (Vector2)transform.position + new Vector2(Random.Range(-palms.rect.width/2,palms.rect.width/2),trunk.rect.height*1.3f);//
            Vector2 newVinePosition = (Vector2)palms.position + new Vector2(Random.Range(-palms.rect.width/2,palms.rect.width/2),-palms.rect.height/4);
            Transform newVine = GameObject.Instantiate(vine,newVinePosition, Quaternion.identity);
            newVine.SetParent(palms);
            // newVine.localPosition = new Vector2(newVine.localPosition.x, 0.7f);
            //set vine position to bottom of palm
            // vine.position = (Vector2)palms.position - new Vector2(0,palms.localScale.y/2);
            //set vine horizontal position randomly along the bottom of the palm
            // vine.position = (Vector2)palms.position + new Vector2(Random.Range(-palms.localScale.x/2,palms.localScale.x/2),-palms.localScale.y/2);
        }
    }

    void populateLightShafts() {
        for(int i=0;i<maxLightShafts;i++) {
            if(Random.Range(0f,1f)<pctChanceLightShaft) {
                Vector2 newShaftPosition = (Vector2)transform.position + new Vector2(Random.Range(-palms.rect.width/2,palms.rect.width/2),trunk.rect.height*1.5f);
                Transform newLightShaft = GameObject.Instantiate(lightShaft,newShaftPosition,Quaternion.identity);
                
                // if(isFlipped) newLightShaft.localScale = new Vector3(-1f, 1f, 1f);
                // else newLightShaft.localScale = new Vector3(1f, 1f, 1f);
                newLightShaft.SetParent(palms);

            }

        }
    }
}

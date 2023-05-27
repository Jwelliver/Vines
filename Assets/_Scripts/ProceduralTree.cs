using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralTree : MonoBehaviour
{
    [SerializeField] float minTrunkHeight = 1f;
    [SerializeField] float maxTrunkHeight = 3f;
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

    SpriteRenderer trunkSpriteRenderer;
    SpriteRenderer palmsSpriteRenderer;
    
    
    bool isFlipped;

    Transform player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").transform;
        trunkSpriteRenderer = trunk.GetComponent<SpriteRenderer>();
        palmsSpriteRenderer = palms.GetComponent<SpriteRenderer>();
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
        trunk.localScale = new Vector3(trunk.localScale.x,Random.Range(minTrunkHeight,maxTrunkHeight));

        // Assign random sprites
        trunkSpriteRenderer.sprite = getRandomSprite(trunkSprites);
        palmsSpriteRenderer.sprite = getRandomSprite(palmSprites);

        transform.eulerAngles = Vector3.forward * Random.Range(0,maxAngle);
    }

    Sprite getRandomSprite(List<Sprite> options) {
        return options[Random.Range(0,options.Count)];
    }

    Vector2 getRandomLocationInPalms() {
        return (Vector2)palms.position + new Vector2(Random.Range(-palms.rect.width/2,palms.rect.width/2),-palms.rect.height/4);
    }

    void populateVines() {
        int nVines = Random.Range(1,maxVines);
        for(int i =0; i<nVines; i++) {
            Transform newVine = GameObject.Instantiate(vine,getRandomLocationInPalms(), Quaternion.identity, transform); //palms (PARENT - REMOVED)
        }
    }

    void populateLightShafts() {
        for(int i=0;i<maxLightShafts;i++) {
            if(Random.Range(0f,1f)<pctChanceLightShaft) {
                Vector2 newShaftPosition = (Vector2)transform.position + new Vector2(Random.Range(-palms.rect.width/2,palms.rect.width/2),trunk.rect.height*1.5f);
                Transform newLightShaft = GameObject.Instantiate(lightShaft,newShaftPosition,Quaternion.identity, palms);
            }

        }
    }
}

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

    [SerializeField] RectTransform trunk;
    [SerializeField] RectTransform palms;
    [SerializeField] Transform vine;
    [SerializeField] Transform lightShaft;
    [SerializeField] List<Sprite> palmSprites;
    [SerializeField] List<Sprite> trunkSprites;

    Transform myTransform;
    Transform vinesContainer;
    SpriteRenderer trunkSpriteRenderer;
    SpriteRenderer palmsSpriteRenderer;


    void Awake()
    {
        myTransform = transform;
        vinesContainer = myTransform.Find("VinesContainer");
        trunkSpriteRenderer = trunk.GetComponent<SpriteRenderer>();
        palmsSpriteRenderer = palms.GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {

        initTree();
        // initPalms();
        populateVines();
        populateLightShafts();
    }

    void initTree()
    {

        float rndScale = RNG.RandomRange(minScale, maxScale);
        transform.localScale = new Vector2(rndScale, rndScale);
        trunk.localScale = new Vector3(trunk.localScale.x, RNG.RandomRange(minTrunkHeight, maxTrunkHeight));

        // Assign random sprites
        trunkSpriteRenderer.sprite = RNG.RandomChoice(trunkSprites);//getRandomSprite(trunkSprites);
        palmsSpriteRenderer.sprite = RNG.RandomChoice(palmSprites); ////091723 removed for new Palms

        transform.eulerAngles = Vector3.forward * RNG.RandomRange(0, maxAngle);
    }

    void initPalms()
    { //091823 for use with individual palmLeaf tree only
        palms.GetComponent<PalmLeaf>().CreatePalmLeaves();
    }

    Vector2 getRandomLocationInPalms()
    {   //returns position in palms rect
        float offsetX = RNG.RandomRange(-palms.sizeDelta.x / 2, palms.sizeDelta.x / 2);
        float offsetY = RNG.RandomRange(-palms.sizeDelta.y / 2, palms.sizeDelta.y / 2);
        return (Vector2)palms.position + new Vector2(offsetX, offsetY);
    }


    // Vector2 getRandomLocationInPalmLeaves() //Not in use 091823
    // { 
    //     Debug.Log("PalmLeaves childCount: "+palms.childCount);
    //     RectTransform rndLeaf = (RectTransform)palms.GetChild(RNG.RandomRange(0,palms.childCount));
    //     Debug.Log("RndLeaf: "+ rndLeaf);
    //     float offsetX = RNG.RandomRange(-rndLeaf.sizeDelta.x / 2, rndLeaf.sizeDelta.x / 2);
    //     float offsetY = RNG.RandomRange(-rndLeaf.sizeDelta.y / 2, rndLeaf.sizeDelta.y / 2);
    //     return (Vector2)rndLeaf.position + new Vector2(offsetX, offsetY);
    // }

    void populateVines()
    {
        int nVines = RNG.RandomRange(1, maxVines);
        for (int i = 0; i < nVines; i++)
        {
            GameObject.Instantiate(vine, getRandomLocationInPalms(), Quaternion.identity, vinesContainer); //palms (PARENT - REMOVED)
        }
    }

    void populateLightShafts()
    {
        for (int i = 0; i < maxLightShafts; i++)
        {
            if (RNG.SampleProbability(pctChanceLightShaft))
            {
                Vector2 newShaftPosition = getRandomLocationInPalms();

                Transform newLightShaft = Instantiate(lightShaft);
                newLightShaft.position = newShaftPosition;

                // Debug.Log("FinalLightShaftPosition: " + newLightShaft.position + " | rndPalmPos: " + newShaftPosition + " | palmsPositions: " + palms.position);
                // newLightShaft.gameObject.SendMessage("setParent");
            }
        }
    }

    // void populateLightShafts()
    // {
    //     for (int i = 0; i < maxLightShafts; i++)
    //     {
    //         if (Random.Range(0f, 1f) < pctChanceLightShaft)
    //         {
    //             Vector2 newShaftPosition = (Vector2)transform.position + new Vector2(Random.Range(-palms.rect.width / 2, palms.rect.width / 2), trunk.rect.height * 1.5f);
    //             Transform newLightShaft = GameObject.Instantiate(lightShaft, newShaftPosition, Quaternion.identity, palms);
    //         }

    //     }
    // }
}

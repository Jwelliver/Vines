using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ProceduralTree : MonoBehaviour
{
    [SerializeField] float minTrunkHeight = 1.5f;
    [SerializeField] float maxTrunkHeight = 2.5f;
    [SerializeField] float minScale = 1f;
    [SerializeField] float maxScale = 1.25f;

    [SerializeField] float maxAngle = 5f;

    [SerializeField] int maxVines = 4;

    [SerializeField] float pctChanceLightShaft = 0.3f;
    [SerializeField] int maxLightShafts = 2;

    [SerializeField] RectTransform trunk;
    [SerializeField] RectTransform palmContainer;
    [SerializeField] Transform vine;
    [SerializeField] Transform lightShaft;
    [SerializeField] List<Transform> palmPrefabs;
    [SerializeField] List<Sprite> trunkSprites;

    Transform myTransform;
    Transform vinesContainer;
    SpriteRenderer trunkSpriteRenderer;
    // Transform palm;
    List<Vector2> palmAnchorPositions = new List<Vector2>();

    // SpriteRenderer palmsSpriteRenderer;


    void Awake()
    {
        myTransform = transform;
        vinesContainer = myTransform.Find("VinesContainer");
        trunkSpriteRenderer = trunk.GetComponent<SpriteRenderer>();
        // palmsSpriteRenderer = palms.GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {

        initTree();
        initPalms();
        populateVines();
        populateLightShafts();
    }

    void initTree()
    {

        float rndScale = RNG.RandomRange(minScale, maxScale);
        transform.localScale = new Vector2(rndScale, rndScale);
        trunk.localScale = new Vector3(trunk.localScale.x, RNG.RandomRange(minTrunkHeight, maxTrunkHeight));

        // Assign random sprite
        trunkSpriteRenderer.sprite = RNG.RandomChoice(trunkSprites);

        transform.eulerAngles = Vector3.forward * RNG.RandomRange(0, maxAngle);
    }

    void initPalms()
    {
        // get random palm Prefab
        Transform rndPalm = RNG.RandomChoice(palmPrefabs);
        // place at palms location
        Transform newPalm = Instantiate(rndPalm, palmContainer.position, Quaternion.identity, transform);
        // match palm scale to tree scale
        // newPalm.localScale = transform.localScale;
        // get Palm AnchorContainer
        Transform palmAnchorContainer = newPalm.Find("Anchors");
        if (palmAnchorContainer == null)
        {
            Debug.LogError("Palm Anchor not found in tree " + name + " palmName: " + newPalm.name);
            return;
        }
        //add all anchor positions to palmAnchorPositions
        foreach (Transform anchor in palmAnchorContainer)
        {
            palmAnchorPositions.Add(anchor.position);
        }
        //Set Parent to palm container
        // newPalm.SetParent(transform);

        // destroy anchor container now that we don't need it;
        Destroy(palmAnchorContainer.gameObject);
    }

    Vector2 getRandomLocationInPalms()
    {
        return RNG.RandomChoice(palmAnchorPositions);
    }


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
            }
        }
    }
}

//===== ORIG (pre palm prefabs 092023)
// public class ProceduralTree : MonoBehaviour
// {
//     [SerializeField] float minTrunkHeight = 1f;
//     [SerializeField] float maxTrunkHeight = 3f;
//     [SerializeField] float minScale = 1f;
//     [SerializeField] float maxScale = 3f;

//     [SerializeField] float maxAngle = 3f;

//     [SerializeField] int maxVines = 3;

//     [SerializeField] float pctChanceLightShaft = 0.5f;
//     [SerializeField] int maxLightShafts = 3;

//     [SerializeField] RectTransform trunk;
//     [SerializeField] RectTransform palms;
//     [SerializeField] Transform vine;
//     [SerializeField] Transform lightShaft;
//     [SerializeField] List<Sprite> palmSprites;
//     [SerializeField] List<Sprite> trunkSprites;

//     Transform myTransform;
//     Transform vinesContainer;
//     SpriteRenderer trunkSpriteRenderer;
//     SpriteRenderer palmsSpriteRenderer;


//     void Awake()
//     {
//         myTransform = transform;
//         vinesContainer = myTransform.Find("VinesContainer");
//         trunkSpriteRenderer = trunk.GetComponent<SpriteRenderer>();
//         palmsSpriteRenderer = palms.GetComponent<SpriteRenderer>();
//     }

//     // Start is called before the first frame update
//     void Start()
//     {

//         initTree();
//         // initPalms();
//         populateVines();
//         populateLightShafts();
//     }

//     void initTree()
//     {

//         float rndScale = RNG.RandomRange(minScale, maxScale);
//         transform.localScale = new Vector2(rndScale, rndScale);
//         trunk.localScale = new Vector3(trunk.localScale.x, RNG.RandomRange(minTrunkHeight, maxTrunkHeight));

//         // Assign random sprites
//         trunkSpriteRenderer.sprite = RNG.RandomChoice(trunkSprites);//getRandomSprite(trunkSprites);
//         palmsSpriteRenderer.sprite = RNG.RandomChoice(palmSprites); ////091723 removed for new Palms

//         transform.eulerAngles = Vector3.forward * RNG.RandomRange(0, maxAngle);
//     }

//     void initPalms()
//     { //091823 for use with individual palmLeaf tree only
//         palms.GetComponent<PalmLeaf>().CreatePalmLeaves();
//     }

//     Vector2 getRandomLocationInPalms()
//     {   //returns position in palms rect
//         float offsetX = RNG.RandomRange(-palms.sizeDelta.x / 2, palms.sizeDelta.x / 2);
//         float offsetY = RNG.RandomRange(-palms.sizeDelta.y / 2, palms.sizeDelta.y / 2);
//         return (Vector2)palms.position + new Vector2(offsetX, offsetY);
//     }


//     void populateVines()
//     {
//         int nVines = RNG.RandomRange(1, maxVines);
//         for (int i = 0; i < nVines; i++)
//         {
//             GameObject.Instantiate(vine, getRandomLocationInPalms(), Quaternion.identity, vinesContainer); //palms (PARENT - REMOVED)
//         }
//     }

//     void populateLightShafts()
//     {
//         for (int i = 0; i < maxLightShafts; i++)
//         {
//             if (RNG.SampleProbability(pctChanceLightShaft))
//             {
//                 Vector2 newShaftPosition = getRandomLocationInPalms();

//                 Transform newLightShaft = Instantiate(lightShaft);
//                 newLightShaft.position = newShaftPosition;

//                 // Debug.Log("FinalLightShaftPosition: " + newLightShaft.position + " | rndPalmPos: " + newShaftPosition + " | palmsPositions: " + palms.position);
//                 // newLightShaft.gameObject.SendMessage("setParent");
//             }
//         }
//     }
// }

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ProceduralLightShaft : MonoBehaviour
{

    [SerializeField] float heightVariation = 1f; // +/-variance added to distance to ground  
    [SerializeField] float groundLevel = 0f;
    [SerializeField] float minWidth = 0.5f;
    [SerializeField] float maxWidth = 2f;

    [SerializeField] float minAngle = 50f;
    [SerializeField] float maxAngle = 60f;

    [SerializeField] float minIntensity = 1f;
    [SerializeField] float maxIntensity = 3f;

    private Transform lightshaftContainer;

    // [SerializeField] float pctChanceNaturalAnimation = 0.2f;
    // [SerializeField] float minAnimSpeed = 0.5f;
    // [SerializeField] float maxAnimSpeed = 1f;

    // Animator animator;

    // Start is called before the first frame update
    void Awake()
    {
        // animator = GetComponent<Animator>();
        try
        {
            lightshaftContainer = GameObject.Find("Environment/ProceduralElements/Lightshafts").transform;
        }
        catch
        {
            lightshaftContainer = null;
        }

    }

    void Start()
    {
        initShaft();
    }


    public void setParent()
    {
        if (lightshaftContainer == null)
        {
            return;
        }
        transform.SetParent(lightshaftContainer);
    }

    void reset()
    {
        transform.localScale = new Vector3(0, 0, 0);
        transform.localRotation = Quaternion.Euler(new Vector2(0, 0));
    }


    void initShaft()
    {
        reset();
        //rotate shaft
        float rotation = RNG.RandomRange(minAngle, maxAngle);

        //size shaft
        float width = RNG.RandomRange(minWidth, maxWidth);
        float distanceToGround = findDistanceToGround(transform.position, transform.eulerAngles.z, groundLevel);
        if (distanceToGround == -1)
        {
            Debug.LogError("Error initializing lightShaft: Ground not Found. Angle Searched: " + rotation);
            gameObject.SetActive(false);
            return;
        }

        distanceToGround += RNG.RandomRange(-heightVariation, heightVariation);

        Vector2 scale = new Vector2(width, distanceToGround);
        transform.localScale = scale;

        //apply rotation
        transform.localRotation = Quaternion.AngleAxis(rotation, Vector3.forward);

        Light2D light = transform.GetComponent<Light2D>();
        light.intensity = RNG.RandomRange(minIntensity, maxIntensity);
        setParent();
    }

    // private Vector2 GetDimensionInPX(GameObject obj)
    // {
    //     SpriteRenderer spriteRenderer;
    //     try
    //     {
    //         spriteRenderer = obj.GetComponent<SpriteRenderer>();
    //     }
    //     catch
    //     {
    //         return obj.transform.localScale;
    //     }
    //     Vector2 tmpDimension;

    //     tmpDimension.x = obj.transform.localScale.x / spriteRenderer.sprite.bounds.size.x;  // this is gonna be our width
    //     tmpDimension.y = obj.transform.localScale.y / spriteRenderer.sprite.bounds.size.y;  // this is gonna be our height
    //     Debug.Log("Obj size: " + tmpDimension);
    //     return tmpDimension;
    // }

    // IEnumerator checkForSizeChange()
    // {
    //     yield return new WaitForSeconds(1f);
    //     // animator.speed = Random.Range(-maxAnimSpeed,maxAnimSpeed);
    //     if (Random.Range(0f, 1f) < pctChanceNaturalAnimation)
    //     {
    //         animator.speed = Random.Range(minAnimSpeed, maxAnimSpeed);
    //     }
    //     else
    //     {
    //         animator.speed = 0;
    //     }
    //     StartCoroutine(checkForSizeChange());
    // }

    // void OnDestroy()
    // {
    //     StopAllCoroutines();
    // }

    public float findDistanceToGround(Vector2 objectOriginPosition, float transformRotation, float targetY)
    {
        // Convert angle to radians
        float theta = transformRotation * Mathf.Deg2Rad;

        // Consider the y difference. If targetY is above the object, flip the angle.
        if (targetY > objectOriginPosition.y)
            theta = Mathf.PI - theta;

        // Calculate x2
        float x2 = objectOriginPosition.x - (objectOriginPosition.y - targetY) / Mathf.Tan(theta);

        float distance;
        // If the angle is not a straight line (not 0 or 180 degrees)
        if (transformRotation % 180 != 0)
        {
            // Calculate distance: d = sqrt((x2-x1)² + (y2-y1)²)
            distance = Mathf.Sqrt(Mathf.Pow(x2 - objectOriginPosition.x, 2) + Mathf.Pow(targetY - objectOriginPosition.y, 2));

            // If the distance is supposed to be in the negative x direction, multiply by -1
            if (x2 < objectOriginPosition.x)
                distance *= -1;

            // return distance;
        }
        else // if angle is 0 or 180 degrees, simply return the y difference
        {
            distance = objectOriginPosition.y - targetY;
        }
        // Debug.Log("Position: " + objectOriginPosition + " | rotation: " + transformRotation + " | targetY: " + targetY + "| distance: " + distance);
        return Mathf.Abs(distance);
    }



}


//================== NOT IN USE

// public void SetWorldScaleY(GameObject gameObject, float targetWorldScaleY)
// {
//     // Reset localScale.y to 1 to measure the original sprite size in world units
//     var originalLocalScale = gameObject.transform.localScale;
//     gameObject.transform.localScale = new Vector3(originalLocalScale.x, 1, originalLocalScale.z);

//     // Measure the sprite size in world units (this may vary depending on how your sprite's pixels to units are set up)
//     var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
//     var spriteSizeInWorldUnits = spriteRenderer != null ? spriteRenderer.bounds.size.y : 0.0f;

//     // Scale object
//     gameObject.transform.localScale = new Vector3(originalLocalScale.x, targetWorldScaleY / spriteSizeInWorldUnits, originalLocalScale.z);
// }

// public void SetWorldScaleY(GameObject gameObject, float targetWorldScaleY)
// {
//     // Reset localScale.y to 1 to measure the original sprite size in world units
//     var originalLocalScale = gameObject.transform.localScale;
//     gameObject.transform.localScale = new Vector3(originalLocalScale.x, 1, originalLocalScale.z);

//     // Measure the sprite size in world units
//     var renderer = gameObject.GetComponent<Renderer>();
//     var spriteSizeInWorldUnits = renderer != null ? renderer.bounds.size.y : 0.0f;

//     // Scale object
//     gameObject.transform.localScale = new Vector3(originalLocalScale.x, targetWorldScaleY / spriteSizeInWorldUnits, originalLocalScale.z);
// }
// public void SetWorldScale(GameObject gameObject, Vector3 targetWorldScale)
// {
//     // Reset localScale to 1 to measure the original sprite size in world units.
//     Vector3 originalLocalScale = gameObject.transform.localScale;
//     gameObject.transform.localScale = new Vector3(1, 1, 1);

//     // Measure the sprite size in world units.
//     Renderer renderer = gameObject.GetComponent<Sprite>();
//     Vector3 spriteSizeInWorldUnits = renderer != null ? renderer.bounds.size : new Vector3(0.0f, 0.0f, 0.0f);

//     // Calculate scale multipliers.
//     float multiplierX = spriteSizeInWorldUnits.x == 0 ? 1 : targetWorldScale.x / spriteSizeInWorldUnits.x;
//     float multiplierY = spriteSizeInWorldUnits.y == 0 ? 1 : targetWorldScale.y / spriteSizeInWorldUnits.y;
//     float multiplierZ = spriteSizeInWorldUnits.z == 0 ? 1 : targetWorldScale.z / spriteSizeInWorldUnits.z;

//     // Set scale of the object.
//     gameObject.transform.localScale = new Vector3(originalLocalScale.x * multiplierX, originalLocalScale.y * multiplierY, originalLocalScale.z * multiplierZ);
// }

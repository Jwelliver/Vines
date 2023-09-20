using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RndSprite : MonoBehaviour
{
    [SerializeField] ProbabilityWeightedSpritePool spritePool;
    [SerializeField] ProbabilityWeightedColorPool colorPool;
    [SerializeField] SpriteRenderer spriteRenderer;


    // Start is called before the first frame update
    void Awake()
    {
        // If SpriteRenderer not provided, try to auto assign from this gameObject.
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        // if it's still null, log error;
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer Missing on object: " + name);
        }
        else
        {
            spriteRenderer.sprite = spritePool.getRandomItem();
            if (colorPool != null)
            {
                spriteRenderer.color = colorPool.getRandomItem();
            }

        }

    }
}

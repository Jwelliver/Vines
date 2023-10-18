using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "MyAssets/ScriptableObjects/Factories/VineAdornmentFactory")]
public class VineAdornmentFactory : ScriptableObject
{
    [SerializeField] List<Transform> vineAdornmentPrefabs = new List<Transform>();
    [SerializeField] ProbabilityWeightedSpritePool sprites;
    [SerializeField] ProbabilityWeightedColorPool colors;

    public static VineAdornmentFactory Instance;

    void OnEnable()
    {
        //Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this);
        }
    }

    void OnDisable()
    {
        Instance = null;
    }


    public Transform GenerateVineAdornment(Vector2 position, Transform parent, MinMax<float> scale)
    {
        // Get Random Adornment Prefab
        Transform rndAdornment = RNG.RandomChoice(vineAdornmentPrefabs);

        // Get Random Rotation
        float rndRotation = RNG.RandomRange(0, 359);

        // Instantiate new Vine Adornment Prefab
        Transform newAdornment = GameObject.Instantiate(rndAdornment, position, Quaternion.Euler(0, 0, rndRotation));

        // Apply Random Scale
        float rndScale = RNG.RandomRange(scale);
        newAdornment.localScale = new Vector2(rndScale, rndScale);

        // Appy Random Sprite
        Sprite rndSprite = sprites.getRandomItem();
        SpriteRenderer spriteRenderer = newAdornment.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("Error: No SpriteRenderer found on adornment: " + newAdornment.name);
            return null;
        }
        spriteRenderer.sprite = rndSprite;

        // Apply Random Color
        Color rndColor = colors.getRandomItem();
        spriteRenderer.color = rndColor;

        // Set Parent
        if (parent != null) { newAdornment.SetParent(parent); }

        // Return New Adornment
        return newAdornment;

    }

}
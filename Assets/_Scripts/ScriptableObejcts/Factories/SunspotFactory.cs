using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[Serializable]
public struct SunspotBlueprint
{//TODO: this should be a "SunspotLayer" struct described in LevelSettings; and if you do use the blueprint, use it like the other factories as the finalized param storage for each sunspot.

    //TODO: add fields for falloff amt and strength
    public string id;
    public MinMax<float> scaleMultiplier;
    public MinMax<float> spacing;
    public MinMax<float> yOffset;
    public float maxRotation;
    public string TargetSortingLayer;
    public MinMax<float> intensity;
    // public MinMax<float> fallOff;
    public MinMax<float> fallOffStrength;
    public List<Transform> sunspotPrefabs;
}



[CreateAssetMenu(menuName = "MyAssets/ScriptableObjects/Factories/SunspotFactory")]
public class SunspotFactory : ScriptableObject
{
    // [SerializeField] List<Transform> sunspotPrefabs = new List<Transform>();


    public Transform GenerateSunspot(Vector2 position, SunspotBlueprint sunspotBlueprint, Transform parent = null)
    {
        //Pick Random sunnspot prefab
        Transform rndPrefab = RNG.RandomChoice(sunspotBlueprint.sunspotPrefabs);

        // Get random rotation;
        Quaternion rndRotation = Quaternion.Euler(new Vector3(0, 0, RNG.RandomRange(-sunspotBlueprint.maxRotation, sunspotBlueprint.maxRotation)));

        // Instantiate
        Transform newSunspot = GameObject.Instantiate(rndPrefab, position, rndRotation, parent);
        // Apply random scale;
        float rndScalar = RNG.RandomRange(sunspotBlueprint.scaleMultiplier);
        newSunspot.localScale *= new Vector2(rndScalar, rndScalar);


        // Get light2D component apply changes
        Light2D light2D = newSunspot.GetComponent<Light2D>();
        // Apply random intensity
        light2D.intensity = RNG.RandomRange(sunspotBlueprint.intensity);
        light2D.falloffIntensity = RNG.RandomRange(sunspotBlueprint.fallOffStrength);
        // // Set the target sorting layer NOTE * Can't access light2D sortingLayer in any immediately easy way, so, skipping for now.


        return newSunspot;
    }
}
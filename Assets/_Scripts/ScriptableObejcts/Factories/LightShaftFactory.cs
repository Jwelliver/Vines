using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[Serializable]
public class LightShaftFactoryConfig
{
    public float heightVariation = 5f;
    public float groundLevel = -3f;
    public MinMax<float> width = new MinMax<float>(1f, 3f);
    public MinMax<float> angle = new MinMax<float>(20f, 30f);
    public MinMax<float> intensity = new MinMax<float>(1f, 3f);
}


public class LightShaftFactory : ScriptableObject
{
    [SerializeField] Transform lightShaftPrefab;
    [SerializeField] LightShaftFactoryConfig defaultFactoryConfig = new LightShaftFactoryConfig();
    [SerializeField] string lightShaftContainerPath;

    public void SetDefaultFactoryConfig(LightShaftFactoryConfig newDefaultFactoryConfig)
    {
        defaultFactoryConfig = null;
        defaultFactoryConfig = newDefaultFactoryConfig;
    }

    public void GenerateLightShaft(Vector2 position)
    {

        Transform newLightShaft = GameObject.Instantiate(lightShaftPrefab, position, Quaternion.identity);

        //reset transform (avoids issues if re-running position/angle)
        newLightShaft.localScale = new Vector3(0, 0, 0);
        newLightShaft.localRotation = Quaternion.Euler(new Vector2(0, 0));

        //rotate shaft
        float rotation = RNG.RandomRange(defaultFactoryConfig.angle);

        //size shaft
        float width = RNG.RandomRange(defaultFactoryConfig.width);

        float distanceToGround = FindDistanceToGround(newLightShaft.position, newLightShaft.eulerAngles.z, defaultFactoryConfig.groundLevel);
        if (distanceToGround == -1)
        {
            Debug.LogError("Error initializing lightShaft: Ground not Found. Angle Searched: " + rotation);
            newLightShaft.gameObject.SetActive(false);
            return;
        }

        distanceToGround += RNG.RandomRange(-defaultFactoryConfig.heightVariation, defaultFactoryConfig.heightVariation);

        Vector2 scale = new Vector2(width, distanceToGround);
        newLightShaft.localScale = scale;

        //apply rotation
        newLightShaft.localRotation = Quaternion.AngleAxis(rotation, Vector3.forward);

        // apply intensity
        Light2D light = newLightShaft.GetComponent<Light2D>();
        light.intensity = RNG.RandomRange(defaultFactoryConfig.intensity);

        // Set Parent
        Transform lightShaftContainer = GameObject.Find(lightShaftContainerPath).transform;
        newLightShaft.SetParent(lightShaftContainer);

    }

    //TODO: this should be in a static util class
    public float FindDistanceToGround(Vector2 objectOriginPosition, float transformRotation, float targetY)
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

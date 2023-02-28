using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ProceduralLightShaft : MonoBehaviour
{

    [SerializeField] float height = 50f;
    [SerializeField] float minWidth = 0.5f;
    [SerializeField] float maxWidth = 2f;

    [SerializeField] float minAngle = 50f;
    [SerializeField] float maxAngle = 60f;

    [SerializeField] float minIntensity = 1f;
    [SerializeField] float maxIntensity = 3f;


    [SerializeField] Transform shaftSpriteLight;


    // Start is called before the first frame update
    void Start()
    {
        initShaft();
    }


    void initShaft() {
        //size shaft
        float width = Random.Range(minWidth,maxWidth);
        Vector2 scale = new Vector2(width,height);
        shaftSpriteLight.localScale = scale;
        //reposition shaft
        shaftSpriteLight.position -= new Vector3(0,height/2);
        //rotate shaft
        float rotation = Random.Range(minAngle,maxAngle);
        transform.localRotation = Quaternion.AngleAxis(rotation, Vector3.forward);

        Light2D light = shaftSpriteLight.GetComponent<Light2D>();
        light.intensity = Random.Range(minIntensity,maxIntensity);
    }
}

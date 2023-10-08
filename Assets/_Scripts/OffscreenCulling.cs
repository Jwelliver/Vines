using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class OffscreenCulling : MonoBehaviour
{
    [SerializeField] float xDistanceOffset = 1.5f;

    private Camera cam;
    private Transform myTransform;
    private SpriteRenderer spriteRenderer;


    void Start()
    {
        cam = Camera.main;
        myTransform = transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Debug.Log("Culling: " + name);
    }

    void Update()
    {
        try
        {
            Vector3 screenPoint = cam.WorldToViewportPoint(myTransform.position);
            bool onScreenX = screenPoint.x > -xDistanceOffset && screenPoint.x < xDistanceOffset;//&& screenPoint.y > 0 && screenPoint.y < 1;
            spriteRenderer.enabled = onScreenX;
        }
        catch (Exception e)
        {
            Debug.Log("Error: " + e);
        }

    }
}
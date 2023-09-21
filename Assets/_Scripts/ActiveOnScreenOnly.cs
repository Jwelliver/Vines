using System;
using UnityEngine;

public class ActiveOnScreenOnly : MonoBehaviour
{
    [SerializeField] float xDistanceOffset = 1.5f;
    [SerializeField] GameObject target;

    void Awake()
    {
        if (target == null)
        {
            target = gameObject;
        }
    }


    void Update()
    {
        try
        {
            Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
            bool onScreenX = screenPoint.x > -xDistanceOffset && screenPoint.x < xDistanceOffset;//&& screenPoint.y > 0 && screenPoint.y < 1;
            if (target.activeSelf != onScreenX) { target.SetActive(onScreenX); }
        }
        catch (Exception e)
        {
            Debug.LogError("Error: " + e);
        }
    }
}
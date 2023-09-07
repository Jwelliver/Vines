using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ActiveOnScreenOnly : MonoBehaviour
{
    [SerializeField] float xDistanceOffset = 1.5f;
    [SerializeField] GameObject target;
    

    void Update()
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
        bool onScreenX = screenPoint.x > -xDistanceOffset && screenPoint.x < xDistanceOffset;//&& screenPoint.y > 0 && screenPoint.y < 1;
        if(target.activeSelf!=onScreenX) {target.SetActive(onScreenX); }
    }
}
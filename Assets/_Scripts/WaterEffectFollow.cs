using UnityEngine;

// * 092523: Updating to handle water speed adjustment; Following is now handled by MatchCameraPosition
public class WaterEffectFollow : MonoBehaviour
{
    // [SerializeField] Rigidbody2D playerRb;
    [SerializeField] Transform cameraTransform;
    [SerializeField] SpriteRenderer waterSpriteRenderer;
    Material waterMaterial;

    string waterSpeedRef = "_WaterSpeed"; // property ref in Water material
    float defaultWaterSpeed;

    float prevCameraX;

    void Start()
    {
        // playerPrevPos = playerRb.position;
        waterMaterial = waterSpriteRenderer.material;
        defaultWaterSpeed = waterMaterial.GetFloat(waterSpeedRef);
        prevCameraX = cameraTransform.position.x;
    }

    void Update()
    {
        float currentCameraX = cameraTransform.position.x;
        float diff = prevCameraX - currentCameraX;
        float newSpeed = defaultWaterSpeed + (defaultWaterSpeed * diff);
        if (diff < 0) newSpeed = -newSpeed;
        waterMaterial.SetFloat(waterSpeedRef, newSpeed); //todo: if you keep this, add a buffer so that if ref is not moving, dont keep setting the float; causes artifacts
        prevCameraX = currentCameraX;
        // if (newSpeed != defaultWaterSpeed) Debug.Log(newSpeed);
    }
}

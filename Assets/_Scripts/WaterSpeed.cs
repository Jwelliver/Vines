using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSpeed : MonoBehaviour
{
    [SerializeField] Material water;
    [SerializeField] Rigidbody2D playerRb;
    // [SerializeField] float multiplier = 0.01f;
    // [SerializeField] bool invertDirection;

    float prevPlayerPosX = 0;

    float currentXOffset = 0;

    void Start()
    {
        prevPlayerPosX = playerRb.position.x;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // float newSpeed = playerRb.velocity.x*multiplier;
        // newSpeed = invertDirection ? -newSpeed : newSpeed;
        // water.SetFloat("_WaterSpeed", newSpeed); //*Time.deltaTime
        // water.SetTextureOffset("_WaterTexture", new Vector2(currentXOffset+newSpeed, 0));
        float currentPlayerPosX = playerRb.position.x;
        float amtPlayerMoved = prevPlayerPosX - currentPlayerPosX;
        water.SetTextureOffset("_WaterTexture", new Vector2(currentXOffset + amtPlayerMoved, 0));
        currentXOffset += amtPlayerMoved;
        prevPlayerPosX = currentPlayerPosX;

    }
}

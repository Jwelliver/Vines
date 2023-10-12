using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSpeed : MonoBehaviour
{
    // [SerializeField] Material water;
    [SerializeField] string textureName = "_WaterTexture";
    [SerializeField] SpriteRenderer water;
    [SerializeField] float multiplier = 0.001f;
    // [SerializeField] bool invertDirection;

    Rigidbody2D playerRb;
    private Material waterMaterial;
    private Shader waterShader;

    float prevPlayerPosX = 0;

    Vector2 textureOffset;

    // float currentXOffset = 0;


    void Start()
    {
        waterMaterial = water.sharedMaterial;
        waterShader = waterMaterial.shader;
        textureOffset = waterMaterial.GetTextureOffset(textureName);
        playerRb = GameManager.GetPlayerRef().GetComponent<Rigidbody2D>();
        prevPlayerPosX = playerRb.position.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float newSpeed = playerRb.velocity.x * multiplier;
        // newSpeed = invertDirection ? -newSpeed : newSpeed;
        // water.SetFloat("_WaterSpeed", newSpeed); //*Time.deltaTime
        float currentPlayerPosX = playerRb.position.x;
        float amtPlayerMoved = currentPlayerPosX - prevPlayerPosX;

        textureOffset += new Vector2(amtPlayerMoved, 0);

        // waterMaterial.SetTextureOffset(textureName, textureOffset);
        waterMaterial.SetFloat("_XTileOffset", textureOffset.x);
        // waterMaterial.SetFloat("_WaterSpeed", newSpeed);
        prevPlayerPosX = currentPlayerPosX;

    }
}

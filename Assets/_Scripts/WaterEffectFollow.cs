using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterEffectFollow : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] SpriteRenderer water;

    private Vector2 playerPrevPos;
    // Update is called once per frame

    void Start() {
        playerPrevPos = player.position;
    }
    void Update()
    {
        transform.position = new Vector2(player.position.x, transform.position.y);
        // Vector2 textureOffset = water.material.GetTextureOffset("_WaterTexture") + new Vector2(Time.deltaTime*10f,0);//(Vector2)playerPrevPos - (Vector2)player.position;
        // water.material.SetTextureOffset("_WaterTexture",textureOffset);
        playerPrevPos = player.position;
    }
}

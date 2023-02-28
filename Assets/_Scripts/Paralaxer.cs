using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paralaxer : MonoBehaviour
{
    [SerializeField] float speed = 0.5f;

    [SerializeField] Transform player;
    [SerializeField] Rigidbody2D playerMainRb;
    
    void Start() {
        // playerRb = player.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        transform.position += new Vector3(-playerMainRb.velocity.x*speed*Time.deltaTime,0);
    }
}

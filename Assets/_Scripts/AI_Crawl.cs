using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Crawl : MonoBehaviour
{
    [SerializeField] float sightRange;
    [SerializeField] LayerMask layer;
    [SerializeField] float speed = 1f;

    // Rigidbody2D rb;

    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        tryMove();
    }

    void tryMove()
    {
        Collider2D col = checkVision();
        if (col != null)
        {
            // Debug.Log("Moving");
            // Vector2 newPosition = rb.position + (Vector2)transform.up*speed*Time.deltaTime;
            // rb.MovePosition(newPosition);
            transform.position += transform.up * speed * Time.deltaTime;
        }
    }

    Collider2D checkVision()
    {
        // Debug.DrawRay(transform.position,transform.up*sightRange);
        // return Physics.Raycast(transform.position,transform.up,sightRange, layer);
        return Physics2D.OverlapBox(transform.position, new Vector2(sightRange, sightRange), 0, layer);
    }
}

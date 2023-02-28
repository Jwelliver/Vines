using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Amulet : MonoBehaviour
{
    
    GameManager gameManager;
    Animator animator;
    Collider2D myCollider;

    void Start() {
        myCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    void OnTriggerEnter2D(Collider2D col) {
        if(col.tag=="Player") {
            myCollider.enabled = false;
            gameManager.playerFoundAmulet();
            animator.SetTrigger("OnFound");
        }
        // GameObject.Destroy(gameObject);
    }

}

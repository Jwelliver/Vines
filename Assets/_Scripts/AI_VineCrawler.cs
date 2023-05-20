using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_VineCrawler : MonoBehaviour
{
    [SerializeField] EventOnTrigger crawlColliderEvent;
    [SerializeField] float speed = 1f;
    public bool movementEnabled =true;

    private Transform currentSurface;

    // Start is called before the first frame update
    void Start()
    {
        crawlColliderEvent.onTriggerEnter.AddListener(onCrawlColliderTriggerEnter);
    }

    // Update is called once per frame
    void Update()
    {
        if(movementEnabled) {
            crawl();
        }
    }

    void crawl()
    {
        if(currentSurface!=null) {
            transform.rotation = currentSurface.rotation;
            transform.position += transform.up*speed*Time.deltaTime;
        }
    }

    void onCrawlColliderTriggerEnter(Collider2D col) {
        transform.SetParent(col.transform);
        currentSurface = col.transform;
    }
}

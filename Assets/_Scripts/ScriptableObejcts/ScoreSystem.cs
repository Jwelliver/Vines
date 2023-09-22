using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// TODO: Convert to scriptable object.
// TODO: Add game-wide and session wide stats

/* TODO: Stats to add:
    - avg speed
    - pct of total vines touched 
    - jumps per swing
    - air time
    - # times arrow to the hat after death
    - avg deaths to win ratio
*/
public class ScoreSystem : MonoBehaviour
{

    [SerializeField] FadeText statsFadeTextObj;
    bool isRecordingSwingStats = false;

    Rigidbody2D playerRb;




    //Swing Stats
    Vector2 startPosition = new Vector2(0, 0);
    float maxVelocity = 0;
    float distance = 0;
    float maxHeight = 0;


    // Start is called before the first frame update
    void Start()
    {
        playerRb = GameObject.Find("Player").GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isRecordingSwingStats)
        {
            updateSwingStats();
        }
    }


    void updateSwingStats()
    {
        maxVelocity = (playerRb.velocity.magnitude > maxVelocity) ? playerRb.velocity.magnitude : maxVelocity;
        distance = ((playerRb.position.x - startPosition.x) > distance) ? (playerRb.position.x - startPosition.x) : distance;
        maxHeight = (playerRb.position.y > maxHeight) ? playerRb.position.y : maxHeight;
    }

    void resetSwingStats()
    {
        maxVelocity = 0;
        distance = 0;
        maxHeight = 0;
    }

    void logStats()
    {
        string statsText = "Max Velocity: " + maxVelocity;
        statsText += "\n" + "Distance: " + distance;
        statsText += "\n" + "Max Height: " + maxHeight;

        // Debug.Log("Max Velocity: "+ maxVelocity);
        // Debug.Log("Distance: "+ distance);
        // Debug.Log("Max Height: "+ maxHeight);
        statsFadeTextObj.FadeTo(statsText);
    }


    public void onSwingGrab()
    {
        // Debug.Log("Score.onSwingGrab(): " + playerRb.position.x);
        isRecordingSwingStats = false;
        logStats();
    }

    public void onSwingRelease()
    {
        // Debug.Log("Score.onSwingRelease(): " + playerRb.position.x);
        startPosition = playerRb.position;
        isRecordingSwingStats = true;
    }
}

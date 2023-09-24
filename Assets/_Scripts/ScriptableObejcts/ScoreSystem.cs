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
    Vector2 startPosition = new Vector2(0, 0); // Player start position
    Vector2 swingReleasePosition = new Vector2(0, 0);
    float bestJumpVelocity = 0;
    float bestJumpDistance = 0;
    float bestJumpHeight = 0;
    float totalDistance = 0;


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
            UpdateSwingStats();
        }
    }


    void UpdateSwingStats()
    {
        float jumpDistance = Vector2.Distance(swingReleasePosition, playerRb.position);
        float currentTotalDistance = Mathf.Abs(playerRb.position.x - startPosition.x);
        bestJumpDistance = jumpDistance > bestJumpDistance ? jumpDistance : bestJumpDistance;
        bestJumpVelocity = (playerRb.velocity.magnitude > bestJumpVelocity) ? playerRb.velocity.magnitude : bestJumpVelocity;
        bestJumpHeight = (playerRb.position.y > bestJumpHeight) ? playerRb.position.y : bestJumpHeight;
        totalDistance = (currentTotalDistance > totalDistance) ? currentTotalDistance : totalDistance;
    }

    void resetSwingStats()
    {
        bestJumpVelocity = 0;
        bestJumpDistance = 0;
        bestJumpHeight = 0;
    }

    void UpdateText()
    {
        string statsText = "Best Jump Velocity: " + bestJumpVelocity;
        statsText += "\n" + "Best Jump Distance: " + bestJumpDistance;
        statsText += "\n" + "Best Jump Height: " + bestJumpHeight;
        statsText += "\n" + "Total Distance: " + totalDistance;
        statsFadeTextObj.FadeTo(statsText);
    }


    public void onSwingGrab()
    {
        isRecordingSwingStats = false;
        UpdateText();
    }

    public void onSwingRelease()
    {
        // Debug.Log("Score.onSwingRelease(): " + playerRb.position.x);
        swingReleasePosition = playerRb.position;
        isRecordingSwingStats = true;
    }
}

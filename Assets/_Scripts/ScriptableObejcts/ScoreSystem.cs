using System;
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

    [SerializeField] FadeText newRecordTextObj;
    [SerializeField] AudioSource recordBreakAudio;
    [SerializeField] SimpleSave simpleSave;
    bool isRecordingSwingStats = false;

    public static PlayerStats sessionStats;
    public static PlayerStats RecordStats;

    bool allTimeLevelDistanceBeatThisSession;

    Rigidbody2D playerRb;

    //Swing Stats
    Vector2 startPosition = new Vector2(0, 0); // Player start position
    Vector2 swingReleasePosition = new Vector2(0, 0);

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            playerRb = GameManager.GetPlayerRef().GetComponent<Rigidbody2D>();
            sessionStats = new PlayerStats();
            RecordStats = simpleSave.GetRecordStats();
            Debug.Log("Loading: " + RecordStats.ToString());
        }
        catch (Exception e)
        {
            Debug.LogError("ScoreSystem.Start() error: " + e);
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isRecordingSwingStats)
        {
            UpdateSwingStats();
        }
    }


    void OnNewRecord(string recordKey, float value)
    {
        recordBreakAudio.Play();
        newRecordTextObj.FadeTo("New Record!!\n" + recordKey + "\n" + value.ToString());
        RecordStats.Save();
    }

    void CheckForAllTimeRecord()
    {
        if (sessionStats.bestJumpDistance > RecordStats.bestJumpDistance)
        {
            RecordStats.bestJumpDistance = sessionStats.bestJumpDistance;
            OnNewRecord("Jump Distance", sessionStats.bestJumpDistance);
        }

        if (sessionStats.bestJumpVelocity > RecordStats.bestJumpVelocity)
        {
            RecordStats.bestJumpVelocity = sessionStats.bestJumpVelocity;
            OnNewRecord("Jump Velocity", sessionStats.bestJumpVelocity);
        }

        if (sessionStats.bestJumpVelocity > RecordStats.bestJumpVelocity)
        {
            RecordStats.bestJumpVelocity = sessionStats.bestJumpVelocity;
            OnNewRecord("Jump Height", sessionStats.bestJumpVelocity);
        }

        if (sessionStats.bestLevelDistance > RecordStats.bestLevelDistance)
        {
            RecordStats.bestLevelDistance = sessionStats.bestLevelDistance;
            // Only notify the first time it's beaten on the session, so we don't notify on every grab thereon
            if (!allTimeLevelDistanceBeatThisSession)
            {
                OnNewRecord("Level Distance", sessionStats.bestLevelDistance);
                allTimeLevelDistanceBeatThisSession = true;
            }
            else { RecordStats.Save(); }
        }
    }

    void UpdateJumpDistance()
    {
        float jumpDistance = Vector2.Distance(swingReleasePosition, playerRb.position);
        if (jumpDistance > sessionStats.bestJumpDistance) { sessionStats.bestJumpDistance = jumpDistance; }
    }

    void UpdateJumpVelocity()
    {
        float jumpVelocity = playerRb.velocity.magnitude;
        if (jumpVelocity > sessionStats.bestJumpVelocity) { sessionStats.bestJumpVelocity = jumpVelocity; }
    }

    void UpdateJumpHeight()
    {
        float jumpHeight = playerRb.position.y;
        if (jumpHeight > sessionStats.bestJumpHeight) { sessionStats.bestJumpHeight = jumpHeight; }
    }

    void UpdateLevelDistance()
    {
        float levelDistance = Mathf.Abs(playerRb.position.x - startPosition.x);
        if (levelDistance > sessionStats.bestLevelDistance) { sessionStats.bestLevelDistance = levelDistance; }
    }

    void UpdateSwingStats()
    {
        UpdateJumpDistance();
        UpdateJumpHeight();
        UpdateJumpVelocity();
        UpdateLevelDistance();
    }

    void ResetSessionStats()
    {
        sessionStats.Reset();
    }

    // void UpdateSessionStatsText() //* 092723 now handled by StatsTextDisplay and GameOverUI
    // {
    //     statsFadeTextObj.FadeTo(sessionStats.ToString());
    // }

    public void onSwingGrab()
    {
        isRecordingSwingStats = false;
        // UpdateSessionStatsText();
        CheckForAllTimeRecord();
    }

    public void onSwingRelease()
    {
        // Debug.Log("Score.onSwingRelease(): " + playerRb.position.x);
        swingReleasePosition = playerRb.position;
        isRecordingSwingStats = true;
    }
}

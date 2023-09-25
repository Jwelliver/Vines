using UnityEngine;


public class PlayerStats
{ //todo: move this to own module; Probably should have some version that is static so multiple consumers can update and read as needed
  // ... however, still want the struct version that can be used for session stats, all time stats, etc.
    public int nDeaths = 0;
    public float bestJumpDistance = 0;
    public float bestJumpHeight = 0;
    public float bestJumpVelocity = 0;
    public float bestLevelDistance = 0;

    // public bool saveOnChange = false;
    // public float lifetimeDistanceTraveled;

    public void Save()
    {
        PlayerPrefs.SetInt("nDeaths", nDeaths);
        PlayerPrefs.GetFloat("bestJumpDistance", bestJumpDistance);
        PlayerPrefs.GetFloat("bestJumpHeight", bestJumpHeight);
        PlayerPrefs.GetFloat("bestJumpVelocity", bestJumpVelocity);
        PlayerPrefs.GetFloat("bestLevelDistance", bestLevelDistance);
        Debug.Log("Saving: " + ToString());
        PlayerPrefs.Save();
    }

    public void Reset()
    {
        nDeaths = 0;
        bestJumpDistance = 0;
        bestJumpHeight = 0;
        bestJumpVelocity = 0;
        bestLevelDistance = 0;
    }

    new public string ToString()
    {
        string statsText = "Best Jump Velocity: " + bestJumpVelocity;
        statsText += "\n" + "Best Jump Distance: " + bestJumpDistance;
        statsText += "\n" + "Best Jump Height: " + bestJumpHeight;
        statsText += "\n" + "Total Distance: " + bestLevelDistance;
        return statsText;
    }
}


[CreateAssetMenu(menuName = "MyAssets/ScriptableObjects/SimpleSave")]
public class SimpleSave : ScriptableObject
{
    public PlayerStats RecordStats;

    public PlayerStats LoadAllTimeStats()
    {

        RecordStats = RecordStats ?? new PlayerStats
        {
            nDeaths = PlayerPrefs.GetInt("nDeaths"),
            bestJumpDistance = PlayerPrefs.GetFloat("bestJumpDistance"),
            bestJumpHeight = PlayerPrefs.GetFloat("bestJumpHeight"),
            bestJumpVelocity = PlayerPrefs.GetFloat("bestJumpVelocity"),
            bestLevelDistance = PlayerPrefs.GetFloat("bestLevelDistance"),
            // saveOnChange = true
        };
        return RecordStats;
    }

    public void IncrementDeathCount()
    {
        string key = "nDeaths";
        int nDeaths = PlayerPrefs.GetInt(key, 0);
        nDeaths++;
        PlayerPrefs.SetInt(key, nDeaths);
        PlayerPrefs.Save();
    }
}

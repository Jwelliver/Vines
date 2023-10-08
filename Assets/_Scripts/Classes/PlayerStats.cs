using UnityEngine;

[Serializable]
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
        PlayerPrefs.SetFloat("bestJumpDistance", bestJumpDistance);
        PlayerPrefs.SetFloat("bestJumpHeight", bestJumpHeight);
        PlayerPrefs.SetFloat("bestJumpVelocity", bestJumpVelocity);
        PlayerPrefs.SetFloat("bestLevelDistance", bestLevelDistance);
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
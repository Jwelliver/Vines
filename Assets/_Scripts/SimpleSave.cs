using UnityEngine;





[CreateAssetMenu(menuName = "MyAssets/ScriptableObjects/SimpleSave")]
public class SimpleSave : ScriptableObject
{
    // public PlayerStats RecordStats;

    void OnEnable()
    {

    }

    void OnDisable()
    {
        // RecordStats.Save();
    }

    public PlayerStats GetRecordStats()
    {
        return new PlayerStats
        {
            nDeaths = PlayerPrefs.GetInt("nDeaths"),
            bestJumpDistance = PlayerPrefs.GetFloat("bestJumpDistance"),
            bestJumpHeight = PlayerPrefs.GetFloat("bestJumpHeight"),
            bestJumpVelocity = PlayerPrefs.GetFloat("bestJumpVelocity"),
            bestLevelDistance = PlayerPrefs.GetFloat("bestLevelDistance"),
        };
        // Debug.LogError("SimpleSave.OnEnable() 1");
        // Debug.LogError("SimpleSave.OnEnable() RecordStats: \n" + RecordStats.ToString());
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

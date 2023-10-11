using System;
using System.Collections.Generic;
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

[Serializable]
public class CharacterSettings
{
    public Color hatColor;
    //todo: implement all other colors.
}

[Serializable]
public class PlayerData
{

    // public Guid id = System.Guid.NewGuid(); //TODO: implement uuid
    public string name;
    public CharacterSettings characterSettings;
    public PlayerStats personalStats;

}

[Serializable]
public class GameData
{
    // GameData holds local player's playerData, current quality settings, etc.
    // ... if possible, you should be able to just save and load this object to get everything you need for local persistant data.
    public List<PlayerData> localPlayers;

    public void InitAsNew()
    {

    }

}

public class GameDataManager
{
    // ? might be able to handle saving/loading in GameData itself by using a static singleton Instance in there?

    private static GameData _gameData;

    public static GameData GameData
    {
        get
        {
            if (GameDataManager._gameData == null)
            {
                LoadGameData();
            }
            return _gameData;
        }
    }

    static void LoadGameData()
    {
        string gameDataString = PlayerPrefs.GetString("GameData");
        if (gameDataString == null)
        {
            _gameData = new GameData();
            _gameData.InitAsNew();
        }
        _gameData = JsonUtility.FromJson<GameData>(gameDataString);
    }

    static void SaveGameData()
    {
        string gameDataString = JsonUtility.ToJson(_gameData);
        PlayerPrefs.SetString("GameData", gameDataString);
    }


}
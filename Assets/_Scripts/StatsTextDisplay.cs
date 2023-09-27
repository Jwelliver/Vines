using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public enum StatsDisplayMode
{
    SessionStats,
    AllTimeStats,
    Both
}

public class StatsTextDisplay : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI displayText;
    [SerializeField] StatsDisplayMode statsDisplayMode = new StatsDisplayMode();

    public void RefreshStatsText()
    {
        string newText = "";

        if (statsDisplayMode == StatsDisplayMode.SessionStats || statsDisplayMode == StatsDisplayMode.Both)
        {
            newText += GetSessionStats();
        }
        if (statsDisplayMode == StatsDisplayMode.Both)
        {
            newText += "\n\n";
        }
        if (statsDisplayMode == StatsDisplayMode.AllTimeStats || statsDisplayMode == StatsDisplayMode.Both)
        {
            newText += GetAllTimeStats();
        }
        displayText.text = newText;
    }

    string GetSessionStats()
    {
        return "Session:\n" + ScoreSystem.sessionStats.ToString();
    }

    string GetAllTimeStats()
    {
        return "Best Ever:\n" + SimpleSave.RecordStats.ToString();
    }





}

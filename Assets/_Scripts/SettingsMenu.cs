using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] Toggle TouchScreenToggle;
    [SerializeField] TMPro.TMP_Dropdown VsyncDropdown;
    [SerializeField] TMPro.TMP_Dropdown FrameRateDropdown;
    PlayerSettings tempSettings;

    Dictionary<int, int> frameRateValueToIndex = new Dictionary<int, int>();

    // Events
    void Start()
    {
        GameContext.SetGameState(GameState.SettingsMenu);
        tempSettings = GameContext.ActiveSettings; // start with active settings; //TODO: ensure this doesn't affect ActiveSettings; shouldn't reference since it's a struct.
        InitUI();
    }

    void InitUI()
    {
        // Inits settings menu UI to match active settings;

        // TouchScreenToggle
        TouchScreenToggle.isOn = tempSettings.useTouchScreenControls;
        TouchScreenToggle.onValueChanged.AddListener(SetTouchScreenToggle);
        // VsyncDropdown
        VsyncDropdown.AddOptions(new List<string> { "None", "Every V Blank", "Every 2 V Blanks" });
        VsyncDropdown.value = tempSettings.vsyncCount;
        VsyncDropdown.onValueChanged.AddListener(OnVsyncValueChanged);
        // FrameRateDropdown
        frameRateValueToIndex.Add(-1, 0);
        frameRateValueToIndex.Add(30, 1);
        frameRateValueToIndex.Add(60, 2);
        frameRateValueToIndex.Add(120, 3);
        FrameRateDropdown.AddOptions(new List<string> { "-1", "30", "60", "120" });
        FrameRateDropdown.value = frameRateValueToIndex[tempSettings.targetFrameRate];
        FrameRateDropdown.onValueChanged.AddListener(OnFrameRateValueChanged);

    }

    void DeInitUI()
    {
        //Remove event listeners for UI components
        TouchScreenToggle.onValueChanged.RemoveAllListeners();
        VsyncDropdown.onValueChanged.RemoveAllListeners();
        FrameRateDropdown.onValueChanged.RemoveAllListeners();

    }

    // ===== Value handlers

    public void SetTouchScreenToggle(bool newValue)
    {
        tempSettings.useTouchScreenControls = newValue;
    }

    public void OnVsyncValueChanged(int newValue)
    {
        tempSettings.vsyncCount = newValue;
    }

    public void OnFrameRateValueChanged(int newIndex)
    {
        foreach (KeyValuePair<int, int> kv in frameRateValueToIndex)
        {
            if (kv.Value == newIndex)
            {
                tempSettings.targetFrameRate = kv.Key;
                Debug.Log("FrameRateValue Changed: newValue: " + tempSettings.targetFrameRate);
                return;
            }
        }
    }

    // ===== UTIL

    void ConvertFrameRateToDropdownIndex()
    {

    }


    // ===== NAV BUTTONS (CANCEL / ACCEPT)
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }

    public void AcceptNewSettings()
    {
        // apply new settings;
        // Vsync
        QualitySettings.vSyncCount = tempSettings.vsyncCount;
        // Framerate
        Application.targetFrameRate = tempSettings.targetFrameRate;
        // Update Active Settings
        GameContext.ActiveSettings = tempSettings;
        // back to main
        BackToMainMenu();
    }

    void OnDisable()
    {
        DeInitUI();
    }

    void OnDestroy()
    {
        DeInitUI();
    }
}

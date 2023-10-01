using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelSettingsUIController : MonoBehaviour
{

    [SerializeField] LevelSettings levelSettings;

    [SerializeField] TMPro.TMP_Dropdown levelTypeDropdown;

    [SerializeField] TMPro.TMP_InputField seedInput;

    [SerializeField] Slider levelLengthMin;
    [SerializeField] Slider levelLengthax; //todo build a reusable minMax ui class that we can instantiate from a prefab

    [SerializeField] TMPro.TMP_Dropdown treeLayerSelectDropdown;



    private int selectedTreeLayerIndex;

    void Start()
    {

    }

    void OnDisable()
    {
        DeInitUI();
    }

    void OnDestroy()
    {
        DeInitUI();
    }


    void InitUI()
    {
        // levelTypeDropdown
        levelTypeDropdown.AddOptions(new List<string> { "Normal", "Endless" });
        levelTypeDropdown.value = (int)levelSettings.levelType;
        levelTypeDropdown.onValueChanged.AddListener(OnLevelTypeDropdownChanged);
        // SeedInput
        seedInput.text = levelSettings.rngSeed.ToString();
        seedInput.onValueChanged.AddListener(newVal => { levelSettings.rngSeed = int.Parse(newVal); });
        // TreeLayerSelectDropdown
        List<string> treeLayerIds = new List<string>();
        foreach (TreeLayer treeLayer in levelSettings.treeLayers) { treeLayerIds.Add(treeLayer.id); }
        treeLayerSelectDropdown.AddOptions(treeLayerIds);
        treeLayerSelectDropdown.onValueChanged.AddListener(OnTreeLayerSelectDropdownChanged);
    }

    void DeInitUI()
    {
        levelTypeDropdown.onValueChanged.RemoveAllListeners();
        treeLayerSelectDropdown.onValueChanged.RemoveAllListeners();
        seedInput.onValueChanged.RemoveAllListeners();
    }


    // Change Handlers


    void OnLevelTypeDropdownChanged(int newIndex)
    {
        levelSettings.levelType = newIndex == 0 ? LevelType.NORMAL : LevelType.ENDLESS;
    }

    void OnTreeLayerSelectDropdownChanged(int newIndex)
    {
        // Update SelectedIndex
        selectedTreeLayerIndex = newIndex;
        //Refresh TreeLayer UI
        RefreshTreeLayerUI();
    }

    void RefreshTreeLayerUI()
    {
        // In here, update all the tree settings and vine settings ui to match the existing values of the currently selected treeLayer
    }

}

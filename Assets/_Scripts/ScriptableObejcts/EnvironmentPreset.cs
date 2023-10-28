using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyAssets/ScriptableObjects/EnvironmentPreset")]
public class EnvironmentPreset : ScriptableObject
{
    [SerializeField] string id;
    [SerializeField] public List<EnvironmentLayer> environmentLayers = new List<EnvironmentLayer>();

    // void OnEnable()
    // {
    //     SaveToJson();
    // }

    void SaveToJson()
    {
        if (id.Length == 0)
        {
            Debug.LogError("EnvironmentPreset.SaveToJson() Error: id must not be null (used for filename); Not saving.");
            return;
        }
        string path = "JSON/EnvironmentPresets/" + id;
        bool didSave = JsonFileIO.Save(this, path);
        Debug.Log("EnvPreset.Save() > DidSave: " + didSave);
    }
}
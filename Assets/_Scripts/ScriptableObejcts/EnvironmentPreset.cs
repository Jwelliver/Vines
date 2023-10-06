using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyAssets/ScriptableObjects/EnvironmentPreset")]
public class EnvironmentPreset: ScriptableObject {
    [SerializeField] public List<EnvironmentLayer> environmentLayers = new List<EnvironmentLayer>();
}
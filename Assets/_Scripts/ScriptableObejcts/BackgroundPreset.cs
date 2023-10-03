using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "MyAssets/ScriptableObjects/BackgroundPreset")]
public class BackgroundPreset : ScriptableObject
{
    [SerializeField] public List<ProceduralLayer<ProceduralSpriteObject>> backgroundLayers = new List<ProceduralLayer<ProceduralSpriteObject>>();

    public void SetFromList(List<ProceduralLayer<ProceduralSpriteObject>> layerList)
    {
        backgroundLayers = layerList;
    }
}
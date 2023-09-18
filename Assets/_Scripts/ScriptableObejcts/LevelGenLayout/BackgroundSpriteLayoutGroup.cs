using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "MyAssets/ScriptableObjects/Layouts/BackgroundSpriteLayoutGroup")]
public class BackgroundSpriteLayoutGroup : ScriptableObject
{
    public string layerName;
    public List<ProceduralSpriteObjectLayout> layouts;

    void OnValidate()
    {
        UpdateLayouts();
    }

    public void UpdateLayouts()
    {
        //applies layerName and sortOrder to layouts
        for (int i = 0; i < layouts.Count; i++)
        {
            ProceduralSpriteObjectLayout layout = layouts[i];
            layout.sortLayerName = layerName;
            layout.sortOrder = i;
        }
    }

    public void GenerateAllLayouts(Section section, Transform parent = null)
    {
        foreach (ProceduralSpriteObjectLayout layout in layouts)
        {
            layout.GenerateSection(section, parent);
        }
    }
}
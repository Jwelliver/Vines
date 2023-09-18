using UnityEngine;

using System.Collections.Generic;

[CreateAssetMenu(menuName = "MyAssets/ScriptableObjects/Layouts/ProceduralObjectLayout")]
public class ProceduralObjectLayout : ScriptableObject
{
    // public string containerParentPath; //where to place layer parent transform
    public Transform prefab;
    public ProceduralLayoutParams layoutParams;
    // private Transform parent;

    public void GenerateSection(Section section, Transform parent = null)
    {
        List<Vector2> positions = LayoutPositionGenerator.GeneratePositions(section, layoutParams.minSpacing, layoutParams.maxSpacing);
        foreach (Vector2 pos in positions)
        {
            Transform newObj = GameObject.Instantiate(prefab, pos, Quaternion.identity, parent);
            ApplyLayoutParams(newObj);
            ApplyExtraSetup(newObj);
        }
    }

    public virtual void ApplyRandomScale(Transform obj)
    {
        if (!layoutParams.enableRandomScale) return;
        float rndScale = RNG.RandomRange(layoutParams.minScale, layoutParams.maxScale);
        obj.localScale = new Vector2(rndScale, rndScale);
        return;
    }

    public virtual void ApplyRandomFlip(Transform obj)
    {
        if (layoutParams.enableRandomFlip && RNG.RandomBool())
            obj.localScale = new Vector2(-obj.localScale.x, obj.localScale.y);
    }

    public virtual void ApplyExtraSetup(Transform obj) { }//override in subclasses for additional setup;

    private void ApplyLayoutParams(Transform obj)
    {
        ApplyRandomScale(obj);
        ApplyRandomFlip(obj);
    }
}

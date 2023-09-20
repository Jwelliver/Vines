using UnityEngine;

[CreateAssetMenu(menuName = "MyAssets/ScriptableObjects/Layouts/ProceduralSpriteLayout")]
public class ProceduralSpriteObjectLayout : ProceduralObjectLayout
{
    public ProbabilityWeightedSpritePool sprites;
    public string sortLayerName = null;
    public int sortOrder;
    public Color color = new Color(1f, 1f, 1f);

    SpriteRenderer GetSpriteRenderer(Transform obj)
    {
        try
        {
            SpriteRenderer spriteRenderer = obj.gameObject.GetComponent<SpriteRenderer>();
            return spriteRenderer;
        }
        catch
        {
            return obj.gameObject.AddComponent<SpriteRenderer>();
        }
    }

    public override void ApplyRandomScale(Transform obj)
    {//overrides base method to apply the scale directly to spriterenderer if the drawMode is sliced or tiled.
        SpriteRenderer spriteRenderer = GetSpriteRenderer(obj);
        if (spriteRenderer == null || spriteRenderer.drawMode == SpriteDrawMode.Simple)
        {
            base.ApplyRandomScale(obj);
            return;
        }
        float newScalar = RNG.RandomRange(layoutParams.minScale, layoutParams.maxScale);
        Vector2 newScale = new Vector2(newScalar, newScalar);
        spriteRenderer.size *= newScale;
        return;
    }

    public override void ApplyExtraSetup(Transform obj)
    {
        SpriteRenderer spriteRenderer = GetSpriteRenderer(obj);
        if (spriteRenderer == null) { return; }
        Sprite rndSprite = sprites.getRandomItem();
        spriteRenderer.sprite = rndSprite;
        spriteRenderer.color = color;
        spriteRenderer.sortingLayerName = sortLayerName;
        spriteRenderer.sortingOrder = sortOrder;
        return;
    }
}
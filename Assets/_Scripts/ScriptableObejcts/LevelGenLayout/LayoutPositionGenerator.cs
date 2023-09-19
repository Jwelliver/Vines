using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public struct ProceduralLayoutParams
{
    public bool enableRandomScale;
    public float minScale;
    public float maxScale;
    public float minSpacing;
    public float maxSpacing;
    public bool enableRandomFlip;
}


public static class LayoutPositionGenerator
{
    public static List<Vector2> GeneratePositions(Section section, float minSpacing = 1, float maxSpacing = 3, float yOffset = 0)
    {
        if (minSpacing == 0 && maxSpacing == 0)
        {
            Debug.LogError("Error: Incorrect Spacing values. \nminSpacing: " + minSpacing + " | maxSpacing: " + maxSpacing);
            return null;
        }
        float startX = section.startPos.x;
        float endX = startX + section.length;
        float y = section.startPos.y + yOffset;
        float endOffset = section.endOffset;
        float startOffset = section.startOffset;

        List<Vector2> positions = new List<Vector2>();
        for (float x = startX + startOffset; x < endX + endOffset; x += RNG.RandomRange(minSpacing, maxSpacing))
        {
            positions.Add(new Vector2(x, y));
        }
        return positions;
    }
}

using System;
using UnityEngine;

[Serializable]
public class Section
{
    public Vector2 startPos;
    public int length;
    public Vector2 endPos => new Vector2(startPos.x + length, startPos.y);

    public Section Copy()
    {
        return new Section
        {
            startPos = startPos,
            length = length
        };
    }
}




public enum SectionFillType
{
    ALL,
    TREES_ONLY,
    BG_ONLY
}
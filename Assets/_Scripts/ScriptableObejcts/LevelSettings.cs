using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "MyAssets/ScriptableObjects/Level Settings")]
public class LevelSettings : ScriptableObject
{
    [Header("Level Length")]
    public Vector2 startPos;
    public int minLength;
    public int maxLength;
    public int globalStartOffset = 0;
    public int globalEndOffset = 0;
    [Header("Trees")]
    public int treesMinSpacing;
    public int treesMaxSpacing;
    // public int startOffset;
    // public int endOffset;

}

/*
092423

SortOrderUtil

Allows you to more easily organize sprite order by group within a single layer by using defined startOders per group and offsets for group components
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public struct SortedSprite
{
    public SpriteRenderer spriteRenderer;
    public int sortOrderOffset;
}


public class SortedSpriteGroup : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ApplySorting();
    }

    public string sortLayerName;
    public int initSortOrder; // this is the starting point for the group; 
    public List<SortedSprite> sortedSprites = new List<SortedSprite>();

    public void ApplySorting()
    {
        foreach (SortedSprite sortedSprite in sortedSprites)
        {
            sortedSprite.spriteRenderer.sortingLayerName = sortLayerName;
            sortedSprite.spriteRenderer.sortingOrder = initSortOrder + sortedSprite.sortOrderOffset;
        }
    }
}



using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemPool<T> : ScriptableObject
{
    public string description;
    public List<T> items;

    // public override T getRandomItem()
    // {
    //     return RNG.RandomChoice(items);
    // }
}

[CreateAssetMenu(menuName = "MyAssets/ScriptableObjects/ItemPools/Simple/SpritePool")]
public class SpritePool : ItemPool<Sprite> { }
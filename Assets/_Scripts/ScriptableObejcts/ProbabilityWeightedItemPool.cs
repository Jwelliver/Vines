using System;
using System.Collections.Generic;
using UnityEngine;

// [System.Serializable]
[Serializable]
public class ProbabilityWeightedItem<T>
{
    public T item;
    public float probability;
}

public class ProbabilityWeightedItemPool<T> : ItemPool<ProbabilityWeightedItem<T>>
{
    public ItemPool<T> inputPool;

    private void OnValidate()
    {
        if (inputPool != null)
        {
            populateItems();
        }
    }

    public void populateItems()
    {
        items.Clear(); // clear the current items first

        foreach (T inputItem in inputPool.items)
        {
            var weightedItem = new ProbabilityWeightedItem<T>
            {
                item = (T)(object)inputItem,
                probability = 1f // default probability
            };

            items.Add(weightedItem);
        }
    }

    public T getRandomItem()
    {
        ProbabilityWeightedItem<T> rndItem = RNG.RandomChoice(items);
        if (RNG.SampleProbability(rndItem.probability)) { return rndItem.item; }
        else { return getRandomItem(); }
    }
}

// TODO:  101723: Moved these here from LevelGen; ProbWeightedItem is a dupe of ProbabilityWeightedItem (above); Refactor without corrupting EnvPreset Layers
[Serializable]
public class ProbWeightedItem<T>
{
    public T item;
    public float probability;
}

[Serializable]
public class ProbWeightItemList<T>
{
    public List<ProbWeightedItem<T>> items = new List<ProbWeightedItem<T>>();
    public T getRandomItem()
    {
        if (items.Count == 0) { return default; }//should return nullable value of T
        ProbWeightedItem<T> item = RNG.RandomChoice(items);
        if (RNG.SampleProbability(item.probability))
        {
            return item.item;
        }
        else
        {
            return getRandomItem();
        }
    }
}
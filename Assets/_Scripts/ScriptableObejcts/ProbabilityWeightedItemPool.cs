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
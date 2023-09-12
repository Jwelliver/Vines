using UnityEngine;
using System.Collections.Generic;
using System;

public static class RNG
{
    private static System.Random _randomInstance;
    private static int _seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);

    public static System.Random SysRandomInstance
    {
        get
        {
            if (_randomInstance == null)
                _randomInstance = new System.Random(_seed);
            return _randomInstance;
        }
    }

    public static void SetSeed(int? manualSeed = null)
    {
        if (manualSeed.HasValue)
        {
            _seed = manualSeed.Value;
        }
        else
        {
            _seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        }
        _randomInstance = null;
    }

    public static int RandomRange(int lo, int hi)
    {
        return SysRandomInstance.Next(lo, hi);
    }

    public static float RandomRange(float lo, float hi)
    {
        return (float)(SysRandomInstance.NextDouble() * (hi - lo) + lo);
    }

    public static T RandomChoice<T>(List<T> items, bool exceptionOnEmptyOrNull = false)
    {
        /*returns a random item from the list.*/
        if (items == null || items.Count == 0)
        {
            if (exceptionOnEmptyOrNull) { throw new ArgumentException("Cannot choose a random item from an empty list."); }
            else { return default; }
        }
        return items[SysRandomInstance.Next(0, items.Count)];
    }

    public static bool sampleProbability(float probability = 0.5f)
    {
        /*returns true if random(0f,1f) < probability*/
        return RandomRange(0f, 1f) < probability;
    }

    public static bool coinFlip()
    {
        /*returns true 50% of the time*/
        return sampleProbability(0.5f);
    }
}
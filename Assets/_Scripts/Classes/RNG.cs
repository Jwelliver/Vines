using UnityEngine;
using System.Collections.Generic;
using System;

public static class RNG
{
    private static System.Random _randomInstance;
    private static int _seed = -1;// = UnityEngine.Random.Range(int.MinValue, int.MaxValue);

    public static System.Random SysRandomInstance
    {
        get
        {
            // if seed has not been set; Set it to random;
            if (_seed == -1) { _seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue); }
            // if instance not already init'd create new;
            if (_randomInstance == null) { _randomInstance = new System.Random(_seed); }
            return _randomInstance;
        }
    }

    public static int SetSeed(int? manualSeed = null)
    {
        if (manualSeed.HasValue) { _seed = manualSeed.Value; }
        else { _seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue); }
        // Reinit Sys random with new seed;
        _randomInstance = new System.Random(_seed);
        return _seed;
    }

    public static int GetCurrentSeed()
    {
        return _seed;
    }

    public static int RandomRange(int lo, int hi)
    {
        return SysRandomInstance.Next(lo, hi);
    }

    public static float RandomRange(float lo, float hi)
    {
        return (float)(SysRandomInstance.NextDouble() * (hi - lo) + lo);
    }

    public static float RandomRange(MinMax<float> range)
    {
        return RandomRange(range.min, range.max);
    }

    public static int RandomRange(MinMax<int> range)
    {
        return RandomRange(range.min, range.max);
    }

    public static T RandomChoice<T>(List<T> items, bool exceptionOnEmptyOrNull = false)
    {
        /*returns a random item from the list.*/
        if (items == null || items.Count == 0)
        {
            if (exceptionOnEmptyOrNull) { throw new ArgumentException("Cannot choose a random item from an empty list."); }
            else { return default; }
        }
        if (items.Count == 1) { return items[0]; }
        return items[SysRandomInstance.Next(0, items.Count)];
    }

    public static T RandomChoice<T>(T[] items, bool exceptionOnEmptyOrNull = false)
    {
        /*returns a random item from the list.*/
        if (items == null || items.Length == 0)
        {
            if (exceptionOnEmptyOrNull) { throw new ArgumentException("Cannot choose a random item from an empty list."); }
            else { return default; }
        }
        if (items.Length == 1) { return items[0]; }
        return items[SysRandomInstance.Next(0, items.Length)];
    }

    public static bool SampleProbability(float probability = 0.5f)
    {
        /*returns true if random(0f,1f) < probability*/
        return RandomRange(0f, 1f) < probability;
    }

    public static bool RandomBool()
    {
        /*returns true 50% of the time*/
        return SampleProbability(0.5f);
    }

    public static int SampleOccurrences(int nTests, float probability)
    {
        /*returns the number of times SampleProbability() returned true over nTests*/
        int nOccurrences = 0;
        for (int i = 0; i < nTests; i++)
        {
            if (SampleProbability(probability))
            {
                nOccurrences++;
            }
        }
        return nOccurrences;
    }


    public static int GetRandomNormalDistribution(float mean, float stdDev) //TODO: TEST
    {
        // Use System.Random to generate two uniform random variables
        System.Random rand = SysRandomInstance;
        double u1 = 1.0 - rand.NextDouble(); //uniform(0,1] random doubles
        double u2 = 1.0 - rand.NextDouble();

        // Use Box-Muller method to generate two independent standard normally distributed random variables.
        double randStdNormal = System.Math.Sqrt(-2.0 * System.Math.Log(u1)) *
                            System.Math.Sin(2.0 * System.Math.PI * u2);

        // Scale and shift to get desired mean and stdDev 
        double randNormal = mean + stdDev * randStdNormal;

        //Return the normally distributed random number rounded to the nearest integer value
        return (int)System.Math.Round(randNormal);
    }

    public static int GetRandomNormalDistribution(int min, int max)//TODO: TEST
    {
        double mean = (max + min) / 2;
        double stdDev = (max - min) / 6; // Approximate 99.7% of data between min & max 

        // Use System.Random to generate two uniform random variables
        System.Random rand = SysRandomInstance;
        double u1 = 1.0 - rand.NextDouble(); //uniform(0,1] random doubles
        double u2 = 1.0 - rand.NextDouble();

        // Use Box-Muller method to generate two independent standard normally distributed random variables
        double randStdNormal = System.Math.Sqrt(-2.0 * System.Math.Log(u1)) *
                            System.Math.Sin(2.0 * System.Math.PI * u2);

        // Scale and shift to get desired mean and stdDev 
        double randNormal = mean + stdDev * randStdNormal;

        // Ensure the result is within the range
        randNormal = System.Math.Max(min, System.Math.Min(max, randNormal));

        //Return the normally distributed random number rounded to the nearest integer value
        return (int)System.Math.Round(randNormal);
    }


}
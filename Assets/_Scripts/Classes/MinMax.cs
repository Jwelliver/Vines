using System;
using UnityEngine;

[Serializable]
public class MinMax<T>
{
    public T min;
    public T max;
    public MinMax(T _min, T _max)
    {
        min = _min;
        max = _max;
    }

    public MinMax<T> Copy()
    {
        return new MinMax<T>(min, max);
    }
}
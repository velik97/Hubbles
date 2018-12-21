using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyMath
{
    public delegate int ComparerDelegate<T>(T a, T b);
    public delegate float ToFloatDelegate<T>(T item);
    
    public static int ArgMax<T>(T[] array) where T : IComparable<T>
    {
        int maxIndex = 0;
        
        for (var i = 1; i < array.Length; i++)
        {
            if (array[i].CompareTo(array[maxIndex]) > 0)
                maxIndex = i;
        }
        return maxIndex;
    }
    
    public static int ArgMax<T>(T[] array, ComparerDelegate<T> comparer)
    {
        int maxIndex = 0;
        
        for (var i = 1; i < array.Length; i++)
        {
            if (comparer(array[i], array[maxIndex]) > 0)
                maxIndex = i;
        }
        return maxIndex;
    }
    
    public static int ArgMax<T>(T[] array, ToFloatDelegate<T> transformer)
    {
        int maxIndex = 0;
        
        for (var i = 1; i < array.Length; i++)
        {
            if (transformer(array[i]) > transformer(array[maxIndex]))
                maxIndex = i;
        }
        return maxIndex;
    }
    
    public static int ArgMin<T>(T[] array) where T : IComparable<T>
    {
        int minIndex = 0;
        
        for (var i = 1; i < array.Length; i++)
        {
            if (array[i].CompareTo(array[minIndex]) < 0)
                minIndex = i;
        }
        return minIndex;
    }
    
    public static int ArgMin<T>(T[] array, ComparerDelegate<T> comparer)
    {
        int minIndex = 0;
        
        for (var i = 1; i < array.Length; i++)
        {
            if (comparer(array[i], array[minIndex]) < 0)
                minIndex = i;
        }
        return minIndex;
    }
    
    public static int ArgMin<T>(T[] array, ToFloatDelegate<T> transformer)
    {
        int minIndex = 0;
        
        for (var i = 1; i < array.Length; i++)
        {
            if (transformer(array[i]) < transformer(array[minIndex]))
                minIndex = i;
        }
        return minIndex;
    }

    public static T Max<T>(T[] array) where T : IComparable<T>
    {
        return array[ArgMax(array)];
    }
    
    public static T Max<T>(T[] array, ComparerDelegate<T> comparer)
    {
        return array[ArgMax(array, comparer)];
    }
    
    public static T Max<T>(T[] array, ToFloatDelegate<T> transformer)
    {
        return array[ArgMax(array, transformer)];
    }
    
    public static T Min<T>(T[] array) where T : IComparable<T>
    {
        return array[ArgMin(array)];
    }
    
    public static T Min<T>(T[] array, ComparerDelegate<T> comparer)
    {
        return array[ArgMin(array, comparer)];
    }
    
    public static T Min<T>(T[] array, ToFloatDelegate<T> transformer)
    {
        return array[ArgMin(array, transformer)];
    }
}

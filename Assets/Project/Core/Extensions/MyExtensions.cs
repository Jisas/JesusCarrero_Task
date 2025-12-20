using System.Collections.Generic;
using UnityEngine;

public static class MyExtensions
{
    /// <returns>
    /// Returns -1 if none found
    /// </returns>
    public static int IndexOfItem<T>(this IEnumerable<T> collection, T item)
    {
        if (collection == null)
        {
            Debug.LogError("IndexOfItem Caused: source collection is null");
            return -1;
        }

        var index = 0;
        foreach (var i in collection)
        {
            if (Equals(i, item)) return index;
            ++index;
        }

        return -1;
    }
}

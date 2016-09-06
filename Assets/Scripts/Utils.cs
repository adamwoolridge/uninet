using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Utils
{
    public static void CategorizeDifferences<T>(List<T> a, List<T> b, out List<T> added, out List<T> removed, out List<T> remained)
    {
        added = null;
        removed = null;
        remained = null;

        // Treat lists with no items as null
        if (a != null && a.Count == 0) a = null;
        if (b != null && b.Count == 0) b = null;

        // Both null or empty
        if (a == null && b == null) return;

        // a is null, then everything is new
        if (a == null)
        {
            added = new List<T>(b);
            return;
        }

        // b is null, then everything is removed
        if (b == null)
        {
            removed = new List<T>(a);
            return;
        }

        // By default all B were 'added'
        added = new List<T>(b);

        // By default all A were 'removed'
        removed = new List<T>(a);

        //
        // If in B and A then add to remained, and remove from added and removed
        //
        foreach (T item in b)
        {
            if (!a.Contains(item)) continue;

            if (remained == null) remained = new List<T>();

            remained.Add(item);
            added.Remove(item);
            removed.Remove(item);
        }

        if (added.Count == 0) added = null;
        if (removed.Count == 0) removed = null;

    }
}

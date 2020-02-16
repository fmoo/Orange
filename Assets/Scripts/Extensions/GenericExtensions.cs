using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public static class GenericExtensions {
    public static T PickRandomOne<T>(this IEnumerable<T> a) {
        return (new List<T>(a)).PickRandomOne();
    }

    public static T PickRandomOne<T>(this List<T> a) {
        if (a.Count == 0) return default;
        else if (a.Count == 1) return a[0];
        else return a[Random.Range(0, a.Count)];
    }

    public static T PopRandomOne<T>(this List<T> a) {
        if (a.Count == 0) return default;
        else if (a.Count == 1) {
            T result = a[0];
            a.Clear();
            return result;
        } else {
            int i = Random.Range(0, a.Count);
            T result = a[i];
            a.RemoveAt(i);
            return result;
        }
    }

    public static void Shuffle<T>(this List<T> l) {
        var l2 = new List<T>(l);
        l.Clear();
        while (l2.Count > 0) {
            l.Add(l2.PopRandomOne());
        }
    }

    public static bool IsNullOrEmpty<T>(this IEnumerable<T> sequence) {
        if (sequence == null) return true;

        return !sequence.Any();
    }

    public static void Swap<T>(ref T a, ref T b) {
        T x = a;
        a = b;
        b = x;
    }


    /// <returns>
    ///   Returns -1 if none found
    /// </returns>
    public static int IndexOf<T>(this IEnumerable<T> items, T item) {
        var index = 0;

        foreach (var i in items) {
            if (Equals(i, item)) {
                return index;
            }

            ++index;
        }

        return -1;
    }
}

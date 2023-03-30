using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ArrayExtensions {
    public static T GetNextItemWrapped<T>(this T[] array, T item) {
        var pos = array.IndexOf(item);
        if (pos == -1) {
            pos = 0;
        } else {
            pos = (pos + 1) % array.Length;
        }
        return array[pos];
    }

    public static T GetPreviousItemWrapped<T>(this T[] array, T item) {
        var pos = array.IndexOf(item);
        if (pos == -1) {
            pos = array.Length - 1;
        } else {
            pos -= 1;
            if (pos < 0) {
                pos = array.Length - 1;
            }
        }
        return array[pos];
    }

}
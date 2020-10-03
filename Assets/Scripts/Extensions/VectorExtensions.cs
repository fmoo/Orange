using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtensions {
    public static float GetAngleTo(this Vector2 from, Vector2 to) {
        var sign = Mathf.Sign(from.x * to.y - from.y * to.x);
        return Vector2.Angle(from, to) * sign;
    }

    public static Vector3 Midpoint(this Vector3 v1, Vector3 v2) {
        return (v1 + v2) / 2.0f;
    }

    public static Vector2 xy(this Vector3 v) {
        return new Vector2(v.x, v.y);
    }

    public static Vector2Int ToVector2Int(this Vector2 v) {
        return new Vector2Int((int)v.x, (int)v.y);
    }

    public static Vector3Int ToVector3Int(this Vector2Int v) {
        return new Vector3Int(v.x, v.y, 0);
    }

    public static Vector3 LerpSin(this Vector3 a, Vector3 b, float t) {
        if (t >= 1f) t = 1f;
        return Vector3.Lerp(a, b, Mathf.Sin(t * Mathf.PI));
    }
    public static Vector3 LerpSinUnclamped(this Vector3 a, Vector3 b, float t) {
        return Vector3.Lerp(a, b, Mathf.Sin(t * Mathf.PI));
    }

    public static Vector3 WithX(this Vector3 v, float x) {
        return new Vector3(x, v.y, v.z);
    }
    public static Vector3 WithY(this Vector3 v, float y) {
        return new Vector3(v.x, y, v.z);
    }
    public static Vector3 WithZ(this Vector3 v, float z) {
        return new Vector3(v.x, v.y, z);
    }
    public static Vector3 Round(this Vector3 v, int places) {
        return new Vector3(
            (float)System.Math.Round(v.x, places),
            (float)System.Math.Round(v.y, places),
            (float)System.Math.Round(v.z, places));
    }

    public static int ManhattanDistance(this Vector2Int a, Vector2Int b) {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }
    public static float ManhattanDistance(this Vector2 a, Vector2 b) {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }
}

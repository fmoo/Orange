﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtensions {
    public static float GetAngleTo(this Vector2 from, Vector2 to) {
        var sign = Mathf.Sign(from.x * to.y - from.y * to.x);
        return Vector2.Angle(from, to) * sign;
    }

    public static Vector2 xy(this Vector3 v) {
        return new Vector2(v.x, v.y);
    }

    public static Vector3 LerpSin(this Vector3 a, Vector3 b, float t) {
        if (t >= 1f) t = 1f;
        return Vector3.Lerp(a, b, Mathf.Sin(t * Mathf.PI));
    }
    public static Vector3 LerpSinUnclamped(this Vector3 a, Vector3 b, float t) {
        return Vector3.Lerp(a, b, Mathf.Sin(t * Mathf.PI));
    }
}
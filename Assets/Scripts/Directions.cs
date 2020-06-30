using UnityEngine;

public enum Directions2 {
    LEFT,
    RIGHT,
}

public enum Directions4 {
    UP,
    DOWN,
    LEFT,
    RIGHT,
};

public enum Directions8 {
    UPLEFT,
    UP,
    UPRIGHT,
    RIGHT,
    DOWNRIGHT,
    DOWN,
    DOWNLEFT,
    LEFT,
}

public static class DirectionsUtils {
    public static Directions4 CompassToDirections4(string compass) {
        if (compass == "N") {
            return Directions4.UP;
        } else if (compass == "E") {
            return Directions4.RIGHT;
        } else if (compass == "S") {
            return Directions4.DOWN;
        } else if (compass == "W") {
            return Directions4.LEFT;
        } else {
            Debug.LogWarning("Invalid compass direction provided to DirectionsUtils.CompassToDirections4");
            return Directions4.UP;
        }
    }

    public static Directions8 CompassToDirections8(string compass) {
        if (compass == "N") {
            return Directions8.UP;
        } else if (compass == "E") {
            return Directions8.RIGHT;
        } else if (compass == "S") {
            return Directions8.DOWN;
        } else if (compass == "W") {
            return Directions8.LEFT;
        } else if (compass == "NE") {
            return Directions8.UPRIGHT;
        } else if (compass == "SE") {
            return Directions8.DOWNRIGHT;
        } else if (compass == "SW") {
            return Directions8.DOWNLEFT;
        } else if (compass == "NW") {
            return Directions8.UPLEFT;
        } else {
            Debug.LogWarning("Invalid compass direction provided to DirectionsUtils.CompassToDirections4");
            return Directions8.UP;
        }
    }

    public static float ToFloat(this Directions2 d) {
        switch (d) {
            case Directions2.LEFT:
                return -1f;
            case Directions2.RIGHT:
                return 1f;
        }
        return 0f;
    }
    public static int ToInt(this Directions2 d) {
        switch (d) {
            case Directions2.LEFT:
                return -1;
            case Directions2.RIGHT:
                return 1;
        }
        return 0;
    }

    public static string ToString(this Directions2 d) {
        switch (d) {
            case Directions2.LEFT:
                return "left";
            case Directions2.RIGHT:
                return "right";
        }
        return "";
    }

    public static Vector2 ToVector2(this Directions4 d) {
        switch (d) {
            case Directions4.UP:
                return Vector2.up;
            case Directions4.LEFT:
                return Vector2.left;
            case Directions4.RIGHT:
                return Vector2.right;
            case Directions4.DOWN:
                return Vector2.down;
        }
        return Vector2.zero;
    }

    public static Vector2Int ToVector2Int(this Directions4 d) {
        switch (d) {
            case Directions4.UP:
                return Vector2Int.up;
            case Directions4.LEFT:
                return Vector2Int.left;
            case Directions4.RIGHT:
                return Vector2Int.right;
            case Directions4.DOWN:
                return Vector2Int.down;
        }
        return Vector2Int.zero;
    }

    // TODO: Should we normalize up + left to have a magnitude of 1?
    public static Vector2 ToVector2(this Directions8 d) {
        switch (d) {
            case Directions8.UP:
                return Vector2.up;
            case Directions8.UPLEFT:
                return Vector2.up + Vector2.left;
            case Directions8.UPRIGHT:
                return Vector2.up + Vector2.right;
            case Directions8.LEFT:
                return Vector2.left;
            case Directions8.RIGHT:
                return Vector2.right;
            case Directions8.DOWN:
                return Vector2.down;
            case Directions8.DOWNLEFT:
                return Vector2.down + Vector2.left;
            case Directions8.DOWNRIGHT:
                return Vector2.down + Vector2.right;
        }
        return Vector2.zero;
    }

    public static Vector3 ToVector3(this Directions8 d) {
        var v2 = d.ToVector2();
        return new Vector3(v2.x, v2.y, 0f);
    }

    public static Directions4 NearestDirection4(this Vector2 v) {
        float angle = v.GetAngleTo(Vector2.up);
        if (angle < -135f || angle > 135f) {
            return Directions4.DOWN;
        } else if (angle < -45f) {
            return Directions4.LEFT;
        } else if (angle > 45f) {
            return Directions4.RIGHT;
        } else {
            return Directions4.UP;
        }
    }

    public static Directions4 NearestDirection4(this Vector2Int v) {
        return NearestDirection4(new Vector2(v.x, v.y));
    }

    public static Directions4 NearestDirection4(this Vector3Int v) {
        return NearestDirection4(new Vector2(v.x, v.y));
    }

    public static Directions8 NearestDirection8(this Vector3 v) {
        return v.xy().NearestDirection8();
    }

    public static Directions8 NearestDirection8(this Vector2 v) {
        float angle = v.GetAngleTo(Vector2.up);
        if (angle < -157.5f || angle > 157.5f) {
            return Directions8.DOWN;
        } else if (angle < -112.4f) {
            return Directions8.DOWNLEFT;
        } else if (angle > 112.4f) {
            return Directions8.DOWNRIGHT;
        } else if (angle < -67.5f) {
            return Directions8.LEFT;
        } else if (angle > 67.5f) {
            return Directions8.RIGHT;
        } else if (angle < -22.5f) {
            return Directions8.UPLEFT;
        } else if (angle > 22.5f) {
            return Directions8.UPRIGHT;
        } else {
            return Directions8.UP;
        }
    }

    public static Directions8 ToDirections8(this Directions4 d) {
        switch (d) {
            case Directions4.DOWN:
                return Directions8.DOWN;
            case Directions4.LEFT:
                return Directions8.LEFT;
            case Directions4.UP:
                return Directions8.UP;
            case Directions4.RIGHT:
                return Directions8.RIGHT;
            default:
                // TODO: Log
                return Directions8.UP;
        }
    }

    public static string ToCompass(this Directions4 d) {
        switch (d) {
            case Directions4.DOWN:
                return "S";
            case Directions4.LEFT:
                return "W";
            case Directions4.UP:
                return "N";
            case Directions4.RIGHT:
                return "E";
            default:
                return "N";
        }
    }
    public static string ToCompass(this Directions8 d) {
        switch (d) {
            case Directions8.DOWN:
                return "S";
            case Directions8.DOWNLEFT:
                return "SW";
            case Directions8.LEFT:
                return "W";
            case Directions8.UPLEFT:
                return "NW";
            case Directions8.UP:
                return "N";
            case Directions8.UPRIGHT:
                return "NE";
            case Directions8.RIGHT:
                return "E";
            case Directions8.DOWNRIGHT:
                return "SE";
            default:
                return "N";
        }
    }
}
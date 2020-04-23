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

    public static Vector2 ToVector2(Directions4 d) {
        switch (d) {
            case Directions4.UP:
                return Vector2.up;
            case Directions4.LEFT:
                return Vector2.left;
            case Directions4.RIGHT:
                return Vector2.right;
            case Directions4.DOWN:
                return Vector2.right;
        }
        return Vector2.zero;
    }

    public static Vector2 ToVector2Int(Directions4 d) {
        switch (d) {
            case Directions4.UP:
                return Vector2Int.up;
            case Directions4.LEFT:
                return Vector2Int.left;
            case Directions4.RIGHT:
                return Vector2Int.right;
            case Directions4.DOWN:
                return Vector2Int.right;
        }
        return Vector2Int.zero;
    }

    // TODO: Should we normalize up + left to have a magnitude of 1?
    public static Vector2 ToVector2(Directions8 d) {
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
                return Vector2.right;
            case Directions8.DOWNLEFT:
                return Vector2.down + Vector2.left;
            case Directions8.DOWNRIGHT:
                return Vector2.down + Vector2.right;
        }
        return Vector2.zero;
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

    public static Directions8 NearestDirection8(this Vector2 v) {
        // TODO: Return diagonals
        float angle = Vector2.Angle(v, Vector2.up);
        if (angle < -135f || angle > 135f) {
            return Directions8.DOWN;
        } else if (angle < -45f) {
            return Directions8.LEFT;
        } else if (angle > 45f) {
            return Directions8.RIGHT;
        } else {
            return Directions8.UP;
        }
    }
}
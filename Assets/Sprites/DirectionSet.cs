using UnityEngine;

public abstract partial class DirectionSet {
    public OrangeSpriteManagerAnimation GetAnimation(Vector2 heading) {
        return GetAnimation(Mathf.Atan2(heading.y, heading.x) * Mathf.Rad2Deg);
    }
    public OrangeSpriteManagerAnimation GetAnimation(Vector3 heading) {
        return GetAnimation(Mathf.Atan2(heading.y, heading.x) * Mathf.Rad2Deg);
    }
    public OrangeSpriteManagerAnimation GetAnimation(Directions8 heading) {
        return GetAnimation(heading.ToVector2());
    }
    public OrangeSpriteManagerAnimation GetAnimation(Directions4 heading) {
        return GetAnimation(heading.ToVector2());
    }
    public abstract OrangeSpriteManagerAnimation GetAnimation(float angle);

    public static DirectionSet Get(OrangeSpriteDB spriteDB, string animationName) {
        var up = spriteDB.GetAnimation(animationName + "N");
        var upRight = spriteDB.GetAnimation(animationName + "NE");
        var right = spriteDB.GetAnimation(animationName + "E");
        var downRight = spriteDB.GetAnimation(animationName + "SE");
        var down = spriteDB.GetAnimation(animationName + "S");
        var downLeft = spriteDB.GetAnimation(animationName + "SW");
        var left = spriteDB.GetAnimation(animationName + "W");
        var upLeft = spriteDB.GetAnimation(animationName + "NW");
        var fallback = spriteDB.GetAnimation(animationName);
        if (up != null && upRight != null && right != null && downRight != null && down != null && downLeft != null && left != null && upLeft != null) {
            return new DirectionSet8() {
                up = up,
                upRight = upRight,
                right = right,
                downRight = downRight,
                down = down,
                downLeft = downLeft,
                left = left,
                upLeft = upLeft,
            };
        }
        if (up != null && right != null && down != null && left != null) {
            return new DirectionSetCardinal() {
                up = up,
                right = right,
                down = down,
                left = left,
            };
        }
        if (upRight != null && downRight != null && downLeft != null && upLeft != null) {
            return new DirectionSetOrdinal() {
                upRight = upRight,
                downRight = downRight,
                downLeft = downLeft,
                upLeft = upLeft,
            };
        }
        if (down != null) return new DirectionSetFallback() { fallback = down };
        if (downRight != null) return new DirectionSetFallback() { fallback = downRight };
        if (downLeft != null) return new DirectionSetFallback() { fallback = downLeft };
        if (right != null) return new DirectionSetFallback() { fallback = right };
        if (left != null) return new DirectionSetFallback() { fallback = left };
        if (upRight != null) return new DirectionSetFallback() { fallback = upRight };
        if (upLeft != null) return new DirectionSetFallback() { fallback = upLeft };
        if (up != null) return new DirectionSetFallback() { fallback = up };
        if (fallback != null) return new DirectionSetFallback() { fallback = fallback };
        return null;
    }
}

public class DirectionSet8 : DirectionSet {
    public OrangeSpriteManagerAnimation up;
    public OrangeSpriteManagerAnimation upRight;
    public OrangeSpriteManagerAnimation right;
    public OrangeSpriteManagerAnimation downRight;
    public OrangeSpriteManagerAnimation down;
    public OrangeSpriteManagerAnimation downLeft;
    public OrangeSpriteManagerAnimation left;
    public OrangeSpriteManagerAnimation upLeft;
    public override OrangeSpriteManagerAnimation GetAnimation(float angle) {
        if (angle < 0) angle += 360;
        if (angle < 22.5f) return right;
        if (angle < 67.5f) return upRight;
        if (angle < 112.5f) return up;
        if (angle < 157.5f) return upLeft;
        if (angle < 202.5f) return left;
        if (angle < 247.5f) return downLeft;
        if (angle < 292.5f) return down;
        if (angle < 337.5f) return downRight;
        return right;
    }
}
public class DirectionSetCardinal : DirectionSet {
    public OrangeSpriteManagerAnimation up;
    public OrangeSpriteManagerAnimation right;
    public OrangeSpriteManagerAnimation down;
    public OrangeSpriteManagerAnimation left;
    public override OrangeSpriteManagerAnimation GetAnimation(float angle) {
        if (angle < 0) angle += 360;
        if (angle < 45f) return right;
        if (angle < 135f) return up;
        if (angle < 225f) return left;
        if (angle < 315f) return down;
        return right;
    }

}
public class DirectionSetOrdinal : DirectionSet {
    public OrangeSpriteManagerAnimation upRight;
    public OrangeSpriteManagerAnimation downRight;
    public OrangeSpriteManagerAnimation downLeft;
    public OrangeSpriteManagerAnimation upLeft;
    public override OrangeSpriteManagerAnimation GetAnimation(float angle) {
        if (angle < 0) angle += 360;
        if (angle < 90f) return upRight;
        if (angle < 180f) return downRight;
        if (angle < 270f) return downLeft;
        if (angle < 360f) return upLeft;
        return upRight;
    }
}
public class DirectionSetFallback : DirectionSet {
    public OrangeSpriteManagerAnimation fallback;
    public override OrangeSpriteManagerAnimation GetAnimation(float angle) {
        return fallback;
    }
}

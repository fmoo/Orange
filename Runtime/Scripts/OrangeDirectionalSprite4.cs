using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrangeDirectionalSprite4 : MonoBehaviour {
    public string spriteName;
    public Sprite up;
    public Sprite down;
    public Sprite left;
    public Sprite right;

    public class Props {
        public Directions4 direction;
        public bool flipx;
    }

    public Sprite GetSpriteForVector(Vector2 v, out bool flipx) {
        return GetSpriteForDirection(v.NearestDirection4(), out flipx);
    }

    public Sprite GetSpriteForDirection(Directions4 d, out bool flipx) {
        flipx = false;
        switch (d) {
            case Directions4.UP:
                return up;
            case Directions4.DOWN:
                return down;
            case Directions4.LEFT:
                if (!left) {
                    flipx = true;
                    return right;
                }
                return left;
            case Directions4.RIGHT:
                if (!right) {
                    flipx = true;
                    return left;
                }
                return right;
        }
        return down;
    }
}
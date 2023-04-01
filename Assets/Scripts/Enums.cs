using UnityEngine;
using System.Collections;

namespace Orange {
    public enum GravityType {
        NONE = 0,
        SIDE_VIEW = 1,
    }

    public static class OrangeGravityUtils {
        public static void SetGravity(GravityType gravityType) {
            switch (gravityType) {
                case GravityType.NONE:
                    Physics.gravity = Vector3.zero;
                    Physics2D.gravity = Vector2.zero;
                    break;

                case GravityType.SIDE_VIEW:
                    Physics.gravity = Vector3.down;
                    Physics2D.gravity = Vector2.down;
                    break;
            }
        }

    }
}
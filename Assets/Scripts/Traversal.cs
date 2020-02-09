using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Orange {

    public class Traversal {

        [System.Flags]
        enum DirFlags {
            NONE = 0,
            UP = 1,
            DOWN = 2,
            LEFT = 4,
            RIGHT = 8,
            ALL = 15,
        }

        private static readonly DirFlags[] DIR_FLAGS = { DirFlags.UP, DirFlags.DOWN, DirFlags.LEFT, DirFlags.RIGHT };
        private static readonly Vector2Int[] DIRECTIONS =
            { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        private static Vector2Int GetDirectionForFlag(DirFlags flag) {
            switch (flag) {
                case DirFlags.UP:
                    return Vector2Int.up;
                case DirFlags.DOWN:
                    return Vector2Int.down;
                case DirFlags.LEFT:
                    return Vector2Int.left;
                case DirFlags.RIGHT:
                    return Vector2Int.right;
            }
            throw new UnityException("Invalid DirFlag: " + flag);
        }
        private static DirFlags GetFlagInverse(DirFlags flag) {
            switch (flag) {
                case DirFlags.UP:
                    return DirFlags.DOWN;
                case DirFlags.DOWN:
                    return DirFlags.UP;
                case DirFlags.LEFT:
                    return DirFlags.RIGHT;
                case DirFlags.RIGHT:
                    return DirFlags.LEFT;
            }
            throw new UnityException("Invalid DirFlag: " + flag);
        }

        private static float GetDefaultMovePenalty(Vector2Int position) {
            return 1f;
        }

        /**
         * Returns a mapping of movable tile to how much "move" is remaining to move there.
         */
        public static Dictionary<Vector2Int, float> TraverseMove(
            Vector2Int startPoint,
            float move,
            System.Func<Vector2Int, float> GetMovePenalty = null
        ) {
            if (GetMovePenalty == null) GetMovePenalty = GetDefaultMovePenalty;

            var result = new Dictionary<Vector2Int, float>();
            var work = new Queue<(Vector2Int, float)>();

            work.Enqueue((startPoint, move));

            while (work.Count > 0) {
                var (position, moveRemaining) = work.Dequeue();
                result[position] = moveRemaining;

                foreach (var d in DIRECTIONS) {
                    var nextPos = position + d;
                    var nextMove = moveRemaining - GetMovePenalty(nextPos);
                    // If we don't have enough move to get here, skip it.
                    if (nextMove < 0) continue;
                    // If we have a faster route here, skip it.
                    if (result.ContainsKey(nextPos) && result[nextPos] > nextMove) continue;
                    work.Enqueue((nextPos, nextMove));
                }
            }
            return result;
        }

        public static HashSet<Vector2Int> TraverseAction(
            IEnumerable<Vector2Int> startPoints,
            int minDistance,
            int maxDistance
        ) {
            var result = new HashSet<Vector2Int>();
            var work = new Queue<(Vector2Int, int, DirFlags)>();
            foreach (var startPoint in startPoints) {
                work.Enqueue((startPoint, 0, DirFlags.ALL));
            }
            while (work.Count > 0) {
                var (position, distance, availDirs) = work.Dequeue();
                if (distance >= minDistance && distance <= maxDistance) {
                    result.Add(position);
                }
                var nextDistance = distance + 1;
                if (nextDistance > maxDistance) continue;
                foreach (var df in DIR_FLAGS) {
                    if ((availDirs & df) != df) continue;
                    var d = GetDirectionForFlag(df);
                    var nextPos = position + d;
                    var nextAvail = availDirs & ~GetFlagInverse(df);
                    work.Enqueue((nextPos, nextDistance, nextAvail));
                }
            }
            return result;
        }
    }
}
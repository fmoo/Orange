using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public static readonly Vector2Int[] DIRECTIONS =
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
            Vector2Int position,
            int minDistance,
            int maxDistance
        ) {
            return TraverseAction(new Vector2Int[] { position }, minDistance, maxDistance);
        }

        public static HashSet<Vector2Int> TraverseAction(
            IEnumerable<Vector2Int> startPoints,
            int minDistance,
            int maxDistance
        ) {
            return new HashSet<Vector2Int>(
                TraverseActionWithSourceList(startPoints, minDistance, maxDistance).Keys);
        }

        public static Dictionary<Vector2Int, HashSet<Vector2Int>> TraverseActionWithSourceList(
          IEnumerable<Vector2Int> startPoints,
          int minDistance,
          int maxDistance
      ) {
            var result = new Dictionary<Vector2Int, HashSet<Vector2Int>>();
            var work = new Queue<(Vector2Int, int, DirFlags, Vector2Int)>();
            foreach (var startPoint in startPoints) {
                work.Enqueue((startPoint, 0, DirFlags.ALL, startPoint));
            }
            while (work.Count > 0) {
                var (position, distance, availDirs, startPoint) = work.Dequeue();
                if (distance >= minDistance && distance <= maxDistance) {
                    if (!result.ContainsKey(position)) {
                        result[position] = new HashSet<Vector2Int>();
                    }
                    result[position].Add(startPoint);
                }
                var nextDistance = distance + 1;
                if (nextDistance > maxDistance) continue;
                foreach (var df in DIR_FLAGS) {
                    if ((availDirs & df) != df) continue;
                    var d = GetDirectionForFlag(df);
                    var nextPos = position + d;
                    var nextAvail = availDirs & ~GetFlagInverse(df);
                    work.Enqueue((nextPos, nextDistance, nextAvail, startPoint));
                }
            }
            return result;
        }

    }

    public class RestrictedTraversal {
        private IEnumerable<Vector2Int> restrictedTiles;
        public RestrictedTraversal(IEnumerable<Vector2Int> restrictedTiles) {
            this.restrictedTiles = restrictedTiles;
        }

        private System.Func<Vector2Int, bool> GetFilterForDirection(Vector2Int source, Directions4 d) {
            switch (d) {
                case Directions4.UP:
                    return (p) => p.y > source.y;
                case Directions4.DOWN:
                    return (p) => p.y < source.y;
                case Directions4.LEFT:
                    return (p) => p.x < source.x;
                case Directions4.RIGHT:
                    return (p) => p.x > source.x;
            }
            throw new UnityException("Invalid direction: " + d);
        }

        public Vector2Int GetNearestCursorPosition(Vector2Int source) {
            try {
                return restrictedTiles
                    .OrderBy((p) => (p - source).sqrMagnitude)
                    .First();
            } catch (System.InvalidOperationException) {
                Debug.LogWarning("There are no positions available?!");
                return source;
            }
        }

        public Vector2Int GetNextCursorPosition(Vector2Int source, Directions4 d) {
            // TODO: What happens if there is no "First"???
            try {
                return restrictedTiles
                    .Where(GetFilterForDirection(source, d))
                    .OrderBy((p) => (p - source).sqrMagnitude)
                    .First();
            } catch (System.InvalidOperationException) {
                return source;
            }
        }
    }
}
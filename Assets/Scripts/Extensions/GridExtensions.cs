using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class GridExtensions {

    public static Vector3 GetCellCenterLocal(this Grid grid, Vector2Int position) {
        return grid.GetCellCenterLocal(new Vector3Int(position.x, position.y, 0));
    }

    public static Vector3 GetCellCenterWorld(this Grid grid, Vector2Int position) {
        return grid.GetCellCenterWorld(new Vector3Int(position.x, position.y, 0));
    }

    public static RectInt GetTilemapsRectInt(this Grid grid) {
        var tilemaps = grid.GetComponentsInChildren<Tilemap>();
        RectInt rect = new RectInt();
        bool first = true;
        foreach (var tilemap in tilemaps) {
            var cellBounds = tilemap.cellBounds;
            if (first == true) {
                rect.xMin = cellBounds.xMin;
                rect.xMax = cellBounds.xMax;
                rect.yMin = cellBounds.yMin;
                rect.yMax = cellBounds.yMax;
                first = false;
            } else {
                rect.xMin = Mathf.Min(rect.xMin, cellBounds.xMin);
                rect.xMax = Mathf.Max(rect.xMax, cellBounds.xMax);
                rect.yMin = Mathf.Min(rect.yMin, cellBounds.yMin);
                rect.yMax = Mathf.Max(rect.yMax, cellBounds.yMax);
            }
        }
        return rect;
    }

    public static Bounds GetTilemapsBounds(this Grid grid) {
        var rectInt = grid.GetTilemapsRectInt();
        var min = grid.CellToWorld(rectInt.min.ToVector3Int());
        var max = grid.CellToWorld(rectInt.max.ToVector3Int());
        Bounds bounds = new Bounds(min + max / 2f, Vector3.zero);
        bounds.Encapsulate(min);
        bounds.Encapsulate(max);
        return bounds;
    }

    public static IEnumerable<TileBase> GetTiles(this Grid grid, Vector3Int position) {
        return grid.GetTiles<TileBase>(position);
    }

    public static IEnumerable<T> GetTiles<T>(this Grid grid, Vector3Int position) where T : TileBase {
        foreach (var tilemap in grid.GetComponentsInChildren<Tilemap>()) {
            yield return tilemap.GetTile<T>(position);
        }
    }
}

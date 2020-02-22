using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class OrangeGridMaster : MonoBehaviour {
    public Grid grid;
    public RectInt bounds {
        get {
            if (!initialized) DoInitialize();
            return _bounds;
        }
    }

    public Tilemap metaTilemap;
    IEnumerable<Tilemap> tilemaps;

    private Vector3Int v3i = new Vector3Int();
    public Vector3 GetCellPosition(Vector2Int position) {
        return GetCellPosition(position.x, position.y);
    }
    public Vector3 GetCellPosition(int x, int y) {
        v3i.x = x;
        v3i.y = y;
        return grid.GetCellCenterWorld(v3i);
    }

    public IEnumerable<(Vector2Int, T)> GetAllMetaTiles<T>() where T : TileBase {
        foreach (var p in bounds.allPositionsWithin) {
            var t = GetMetaTile<T>(p);
            if (t != null) {
                yield return (p, t);
            }
        }
    }

    public T GetMetaTile<T>(Vector2Int position) where T : TileBase {
        v3i.x = position.x;
        v3i.y = position.y;
        return metaTilemap.GetTile<T>(v3i);
    }

    public IEnumerable<T> GetTiles<T>(Vector2Int position) where T : TileBase {
        DoInitialize();
        v3i.x = position.x;
        v3i.y = position.y;
        return tilemaps.Select(s => s.GetTile<T>(v3i)).Where(s => s != null);
    }

    public Bounds CalculateCameraBounds(Camera c) {
        Bounds orthoBounds = c.OrthographicBounds();
        Vector3 orthoSize = orthoBounds.size;
        RectInt cellBounds = bounds;
        var minWorldPos = GetCellPosition(cellBounds.xMin, cellBounds.yMin);
        minWorldPos -= grid.cellSize / 2;
        var maxWorldPos = GetCellPosition(cellBounds.xMax - 1, cellBounds.yMax - 1);
        maxWorldPos += grid.cellSize / 2;

        Bounds cameraBounds = new Bounds();
        cameraBounds.center = (minWorldPos + maxWorldPos) / 2;
        cameraBounds.size = ((maxWorldPos - minWorldPos) - orthoSize) + (Vector3.forward * 1000f);

        return cameraBounds;
    }

    public static RectInt CalculateBounds(IEnumerable<Tilemap> tilemaps) {
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
        // TODO: Memoize this and only run once on launch?  Whenever tilemaps/grid changes?
        return rect;
    }

    bool initialized = false;
    RectInt _bounds;
    void DoInitialize() {
        if (!initialized) {
            tilemaps = grid.GetComponentsInChildren<Tilemap>();
            _bounds = CalculateBounds(tilemaps);
            initialized = true;
        }
    }
}

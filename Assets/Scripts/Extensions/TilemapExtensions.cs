using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TilemapExtensions {
    public static TileBase GetTile(this Tilemap tilemap, Vector2Int position) {
        return tilemap.GetTile(position.ToVector3Int());
    }
    public static T GetTile<T>(this Tilemap tilemap, Vector2Int position) where T : TileBase {
        return tilemap.GetTile<T>(position.ToVector3Int());
    }

    public static void SetTile(this Tilemap tilemap, Vector2Int position, TileBase tile) {
        tilemap.SetTile(position.ToVector3Int(), tile);
    }
}

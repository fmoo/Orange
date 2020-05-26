using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridExtensions {

    public static Vector3 GetCellCenterLocal(this Grid grid, Vector2Int position) {
        return grid.GetCellCenterLocal(new Vector3Int(position.x, position.y, 0));
    }
    
    public static Vector3 GetCellCenterWorld(this Grid grid, Vector2Int position) {
        return grid.GetCellCenterWorld(new Vector3Int(position.x, position.y, 0));
    }
}

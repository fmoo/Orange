using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Default.SIS.asset", menuName = "Data/SpriteImportSettings", order = 9)]
public class SpriteImportSettings : ScriptableObject {
    public bool setPivot;
    public Vector2 pivot;
    
    public int spritePixelsPerUnit = 16;
    public FilterMode filterMode = FilterMode.Point;
    public int maxTextureSize = 2048;
}
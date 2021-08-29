using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Default.SIS.asset", menuName = "Data/SpriteImportSettings", order = 9)]
public class SpriteImportSettings : ScriptableObject {
    public bool setPivot;
    public Vector2 pivot;

    public int spritePixelsPerUnit = 16;
    public FilterMode filterMode = FilterMode.Point;
    public TextureWrapMode wrapMode = TextureWrapMode.Clamp;
    public int maxTextureSize = 2048;
    public bool readWriteFromScript;

    [System.Serializable]
    public class SpriteDBBatchReference {
        public string importName;
        public string textureMatch;
        public int baseIndex;
        public string frameOffsets;
        public bool loop;
        public bool flip;
        public float timePerFrame;

        public List<SpriteDBBatchGroup> groups;
    }

    [System.Serializable]
    public class SpriteDBBatchGroup {
        public string suffix;
        public string prefix;
        public int baseOffset;
    }

    public OrangeSpriteDB spriteDB;
    public List<SpriteDBBatchReference> syncAssets;
}

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
        public float timePerFrame = 0.08f;

        public List<SpriteDBBatchGroup> groups;
        public List<ReplaceRule> replaceRules;
    }

    [System.Serializable]
    public class ReplaceRule {
        public string match;
        public string replace;
    }

    [System.Serializable]
    public class SpriteDBBatchGroup {
        public string suffix;
        public string prefix;
        public int baseOffset;
        public bool flip;
    }

    public string autoCropThumbnailFrame;
    public OrangeSpriteDB spriteDB;
    public List<SpriteDBBatchReference> syncAssets;
}

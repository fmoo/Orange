using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NaughtyAttributes;
using SuperTiled2Unity;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

[CreateAssetMenu(fileName = "SpritesDB", menuName = "Data/Sprite DB", order = 3)]
public class OrangeSpriteDB : ScriptableObject {
    // [NaughtyAttributes.ReorderableList]
    public List<OrangeSpriteManagerSprite> sprites = new List<OrangeSpriteManagerSprite>();
    // [NaughtyAttributes.ReorderableList]
    public List<OrangeSpriteManagerAnimation> animations = new List<OrangeSpriteManagerAnimation>();
    private Dictionary<string, OrangeSpriteManagerSprite> namedSprites =
        new Dictionary<string, OrangeSpriteManagerSprite>();
    private Dictionary<string, OrangeSpriteManagerAnimation> namedAnimations =
        new Dictionary<string, OrangeSpriteManagerAnimation>();

    public GenericDictionary<string, string> strings;
    public GenericDictionary<string, float> variables;
    public GenericDictionary<string, bool> flags;

    public OrangeSpriteManagerSprite GetSprite(string name) {
        if (!Application.isPlaying) {
            return sprites.FirstOrDefault(s => s.name == name);
        }

        if (namedSprites.Count == 0 && sprites.Count != 0) BuildIndex();
        if (namedSprites.TryGetValue(name, out OrangeSpriteManagerSprite result)) {
            return result;
        }
        return null;
    }

    public OrangeSpriteManagerAnimation GetAnimation(string name) {
        if (!Application.isPlaying) {
            return animations.FirstOrDefault(s => s.name == name);
        }

        if (namedAnimations.Count == 0 && animations.Count != 0) BuildIndex();
        if (namedAnimations.TryGetValue(name, out OrangeSpriteManagerAnimation result)) {
            return result;
        }
        return null;
    }

    public bool HasAnimation(string name) {
        if (!Application.isPlaying) {
            return animations.FirstOrDefault(s => s.name == name) != null;
        }

        if (namedAnimations.Count == 0 && animations.Count != 0) BuildIndex();
        return namedAnimations.ContainsKey(name);
    }

    public bool HasFrame(string name) {
        if (!Application.isPlaying) {
            return sprites.FirstOrDefault(s => s.name == name) != null;
        }

        if (namedSprites.Count == 0 && sprites.Count != 0) BuildIndex();
        return namedSprites.ContainsKey(name);
    }

    private void OnValidate() {
        BuildIndex();
    }
    private void Start() {
        BuildIndex();
    }
    private void BuildIndex() {
        if (!Application.isPlaying) return;
        foreach (var s in sprites) {
            if (s.name == "" || s.sprite == null) continue;
            namedSprites[s.name] = s;
        }
        foreach (var a in animations) {
            a.initFrames(this);
            namedAnimations[a.name] = a;
        }
    }

#if UNITY_EDITOR
    public void EditorSetSprite(string name, Sprite sprite, bool flip = false) {
        bool found = false;
        foreach (var entry in sprites) {
            if (entry.name == name) {
                entry.sprite = sprite;
                entry.flip = flip;
                found = true;
                break;
            }
        }
        if (!found) {
            sprites.Add(
                new OrangeSpriteManagerSprite() {
                    name = name,
                    sprite = sprite,
                    flip = flip
                }
            );
        }
        EditorUtility.SetDirty(this);
    }

    public void EditorSetAnimation(
        string name,
        Sprite[] sprites,
        float duration,
        bool flip,
        bool loop,
        bool reverse
    ) {
        var seenSprites = new Dictionary<Sprite, string>();
        var spriteNames = new List<string>();
        foreach (var sprite in sprites) {
            if (seenSprites.ContainsKey(sprite)) {
                spriteNames.Add(seenSprites[sprite]);
                continue;
            }

            var spriteName = $"{name}_{seenSprites.Count}";
            seenSprites[sprite] = spriteName;
            EditorSetSprite(spriteName, sprite, flip: flip);
            spriteNames.Add(spriteName);
        }

        bool found = false;
        foreach (var entry in animations) {
            if (entry.name == name) {
                entry.duration = duration;
                entry.config = string.Join(",", spriteNames);
                entry.loop = loop;
                entry.reverse = reverse;
                found = true;
                break;
            }
        }
        if (!found) {
            animations.Add(
                new OrangeSpriteManagerAnimation() {
                    name = name,
                    duration = duration,
                    config = string.Join(",", spriteNames),
                    loop = loop,
                    reverse = reverse,
                }
            );
        }
        // Mark this asset dirty
        EditorUtility.SetDirty(this);
    }

    public static NaughtyAttributes.DropdownList<string> GetEditorSpriteDropdown(OrangeSpriteDB db) {
        var result = new NaughtyAttributes.DropdownList<string>();
        if (db == null) return null;
        var names = db.sprites.Select(s => s.name).ToList();
        names.Sort();
        foreach (var name in names) {
            result.Add(name, name);
        }
        return result;
    }
#endif

    public void RemovePrefix(string prefix) {
        sprites = sprites.Where(s => !s.name.StartsWith($"{prefix}_") && s.name != prefix).ToList();
        animations = animations.Where(s => !s.name.Equals(prefix)).ToList();
    }

    [BoxGroup("Sprite Import Configuration")]
    public Sprite[] importSprites;
    [BoxGroup("Sprite Import Configuration")]
    public string importAnimName;
    [BoxGroup("Sprite Import Configuration")]
    public bool renameImportedSprites = true;
    [BoxGroup("Sprite Import Configuration")]
    public bool flipImportedSprites = false;
    [BoxGroup("Sprite Import Configuration")]
    public bool flipImportedSpritesY = false;
    [BoxGroup("Sprite Import Configuration")]
    public bool loopImportedAnimation = true;
    [BoxGroup("Sprite Import Configuration")]
    public float importTimePerFrame = 0.1f;
    [NaughtyAttributes.Button("Import Sprites")]
    public void DoImportSprites() {
        int ii = 0;
        var addedList = new List<string>();
        foreach (var sprite in importSprites) {
            var name = !renameImportedSprites ? sprite.name : $"{importAnimName}_{ii}";
            addedList.Add(name);
            var existingSprite = GetSprite(name);
            if (existingSprite != null) {
                existingSprite.sprite = sprite;
                existingSprite.flip = flipImportedSprites;
                existingSprite.flipY = flipImportedSpritesY;
            } else {
                sprites.Add(new OrangeSpriteManagerSprite() {
                    sprite = sprite,
                    name = name,
                    flip = flipImportedSprites,
                    flipY = flipImportedSpritesY,
                });
            }
            ii += 1;
        }
        if (importAnimName != "") {
            string config = string.Join(",", addedList);
            var existingAnimation = GetAnimation(importAnimName);
            if (existingAnimation != null) {
                existingAnimation.config = config;
                existingAnimation.duration = importTimePerFrame * importSprites.Count();
                existingAnimation.loop = loopImportedAnimation;
            } else {
                animations.Add(new OrangeSpriteManagerAnimation() {
                    name = importAnimName,
                    config = config,
                    duration = importTimePerFrame * importSprites.Count(),
                    loop = loopImportedAnimation
                });
            }
        }
        importSprites = new Sprite[0];
        importAnimName = "";
        renameImportedSprites = true;
        flipImportedSprites = false;
        flipImportedSpritesY = false;
    }
    [NaughtyAttributes.Button("CLEAR ALL")]
    public void Clear() {
        sprites.Clear();
        animations.Clear();
        importSprites = new Sprite[] { };
    }


#if UNITY_EDITOR
    [System.Serializable]
    public class TiledAutosyncFlip {
        public string match;
        public string replace;
    }

    [NaughtyAttributes.Button("Reimport From TSX")]
    void EditorReimportFromTsx() {
        var assetPath = AssetDatabase.GetAssetPath(this);
        AssetDatabase.ImportAsset(assetPath.Replace(".asset", ".tsx"));
    }

    [NaughtyAttributes.Button("Sort Sprites")]
    void EditorSortSprites() {
        sprites.Sort((a, b) => a.name.CompareTo(b.name));
        animations.Sort((a, b) => a.name.CompareTo(b.name));
    }

    [BoxGroup("Thumbnail Extraction")]
    public string extractThumbnailFrame = "";
    [BoxGroup("Thumbnail Extraction")]
    public string extractThumbnailOutPath = "";
    public bool EditorExtractThumbnail() {
        if (extractThumbnailFrame == "") return false;
        var sprite = GetSprite(extractThumbnailFrame);
        if (sprite == null) {
            return false;
        }
        var path = extractThumbnailOutPath;
        if (path == "") {
            path = UnityEditor.AssetDatabase.GetAssetPath(this);
            path = path.Replace(".asset", ".png");
        }

        var oldThumbnailDefault = AssetDatabase.LoadAssetAtPath<DefaultAsset>(path);
        if (oldThumbnailDefault != null) {
            AssetDatabase.DeleteAsset(path);
        }

        var tex = sprite.sprite.texture;

        // Make a readable copy of the texture without using GetPixels() or GetPixels32()
        var oldRt = RenderTexture.active;
        RenderTexture.active = RenderTexture.GetTemporary(tex.width, tex.height);
        Graphics.Blit(tex, RenderTexture.active);
        var readableTex = new Texture2D(tex.width, tex.height);
        readableTex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        readableTex.Apply();
        RenderTexture.active = oldRt;

        var pixels = readableTex.GetPixels(
            (int)sprite.sprite.textureRect.x,
            (int)sprite.sprite.textureRect.y,
            (int)sprite.sprite.textureRect.width,
            (int)sprite.sprite.textureRect.height);

        var outTex = new Texture2D(
            (int)sprite.sprite.textureRect.width,
            (int)sprite.sprite.textureRect.height,
            TextureFormat.RGBA32,
            false
        );

        outTex.SetPixels(pixels);
        outTex.Apply();

        var bytes = outTex.EncodeToPNG();
        File.WriteAllBytes(path, bytes);
        AssetDatabase.Refresh();

        return true;
    }

#endif



}
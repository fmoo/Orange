using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using SuperTiled2Unity.Editor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;


public class OrangeSpriteDBAssetPostprocessor : AssetPostprocessor {
    static Dictionary<string, OrangeSpriteDB> _spriteCollections = new Dictionary<string, OrangeSpriteDB>();


    static OrangeSpriteDB LoadSpriteDB(string path) {
        if (_spriteCollections.TryGetValue(path, out var spriteDB)) {
            return spriteDB;
        }

        var asset = AssetDatabase.LoadAssetAtPath<OrangeSpriteDB>(path);
        if (asset == null) {
            return null;
        }

        _spriteCollections[asset.name] = asset;
        return asset;
    }

    public static OrangeSpriteDB ReimportTsxAssetPath(string assetPath) {
        SuperTileset tileset = AssetDatabase.LoadAssetAtPath<SuperTileset>(assetPath);

        // if tileset contains a prop, spriteDBOutputPath, use that path to construct the spriteDBAssetPath
        // otherwise, use the tileset's asset path to construct the path
        string spriteDBAssetPath = tileset.GetStringProp("spriteDBOutputPath");
        if (string.IsNullOrWhiteSpace(spriteDBAssetPath)) {
            spriteDBAssetPath = assetPath.Replace(".tsx", ".asset");
        } else {
            if (!spriteDBAssetPath.EndsWith(".asset")) {
                spriteDBAssetPath += ".asset";
            }
            // Resolve this path as a relative path to assetPath's directory.
            spriteDBAssetPath = Path.Combine(Path.GetDirectoryName(assetPath), spriteDBAssetPath);
            // Collapse any relative pathing, but don't return a full path.
            spriteDBAssetPath = Path.GetFullPath(spriteDBAssetPath).Replace(Path.GetFullPath("Assets"), "Assets");

        }

        OrangeSpriteDB spriteDB = LoadSpriteDB(spriteDBAssetPath);

        // Auto-create spriteDB if there are named sprites/animations in them.
        if (spriteDB == null) {
            bool shouldCreate = false;
            foreach (var tile in tileset.m_Tiles) {
                if (tile.GetStringProp("spriteName") != "" || tile.GetStringProp("animationName") != "") {
                    shouldCreate = true;
                    break;
                }
            }
            if (!shouldCreate) return null;

            // Create the spriteDB
            spriteDB = ScriptableObject.CreateInstance<OrangeSpriteDB>();
            AssetDatabase.CreateAsset(spriteDB, spriteDBAssetPath);
            _spriteCollections[spriteDBAssetPath] = spriteDB;
        }

        // Import named sprites (tiles with `spriteName`)
        foreach (var tile in tileset.m_Tiles) {
            var spriteName = tile.GetStringProp("spriteName");
            if (string.IsNullOrWhiteSpace(spriteName)) continue;
            spriteDB.EditorSetSprite(spriteName, tile.m_Sprite);
        }

        // Import named animation (tiles with `animationName`)
        foreach (var tile in tileset.m_Tiles) {
            var animationName = tile.GetStringProp("animationName");
            if (string.IsNullOrWhiteSpace(animationName)) continue;

            bool loop = tile.GetBoolProp("animationLoop", true);
            // bool.TryParse(tile.m_CustomProperties.Find(p => p.m_Name == "animationLoop")?.m_Value ?? "True", out oneShot);

            var sprites = tile.m_AnimationSprites;
            if (sprites == null || sprites.Length == 0) continue;

            var settings = SuperTiled2Unity.Editor.ST2USettings.GetOrCreateST2USettings();
            spriteDB.EditorSetAnimation(
                animationName,
                sprites,
                duration: sprites.Length * (1.0f / settings.AnimationFramerate),
                flip: false,
                loop: loop,
                reverse: false
            );
        }

        // Import flipped named animations
        foreach (var tile in tileset.m_Tiles) {
            var animationName = tile.GetStringProp("animationFlipName");
            if (string.IsNullOrWhiteSpace(animationName)) continue;

            bool loop = tile.GetBoolProp("animationLoop", true);

            var sprites = tile.m_AnimationSprites;
            if (sprites == null || sprites.Length == 0) continue;

            var settings = SuperTiled2Unity.Editor.ST2USettings.GetOrCreateST2USettings();
            spriteDB.EditorSetAnimation(
                animationName,
                sprites,
                duration: sprites.Length * (1.0f / settings.AnimationFramerate),
                flip: true,
                loop: loop,
                reverse: false
            );
        }
        return spriteDB;
    }

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
        _spriteCollections.Clear();
        bool didSomething = false;
        List<OrangeSpriteDB> spriteDBs = new List<OrangeSpriteDB>();
        foreach (var assetPath in importedAssets) {
            if (!assetPath.EndsWith(".tsx")) continue;
            var maybeSpriteDB = ReimportTsxAssetPath(assetPath);
            if (maybeSpriteDB != null) {
                spriteDBs.Add(maybeSpriteDB);
                didSomething = true;
            }
        }
        foreach (var assetPath in importedAssets) {
            if (IsSpriteDB(assetPath, out var spriteDB)) {
                spriteDBs.Add(spriteDB);
            }
            didSomething = didSomething || MaybeRegenerateThumbnailsForSpriteDBs(spriteDBs);
        }

        if (didSomething) AssetDatabase.SaveAssets();
        _spriteCollections.Clear();
    }

    static bool IsSpriteDB(string assetPath, out OrangeSpriteDB spriteDB) {
        spriteDB = AssetDatabase.LoadAssetAtPath<OrangeSpriteDB>(assetPath);
        return spriteDB != null;
    }

    static bool MaybeRegenerateThumbnailsForSpriteDBs(List<OrangeSpriteDB> spriteDBs) {
        if (spriteDBs.Count == 0) return false;
        bool didSomething = false;

        // Regenerate thumbnails for all spriteDBs
        foreach (var spriteDB in spriteDBs) {
            if (spriteDB.EditorExtractThumbnail()) {
                didSomething = true;
            }
        }

        return didSomething;
    }
}

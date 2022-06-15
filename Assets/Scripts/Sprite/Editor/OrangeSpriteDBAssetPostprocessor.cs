using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using SuperTiled2Unity.Editor;

public class OrangeSpriteDBAssetPostprocessor : AssetPostprocessor {

    public static void ReimportTsxAssetPath(string assetPath) {
        SuperTileset tileset = AssetDatabase.LoadAssetAtPath<SuperTileset>(assetPath);
        string spriteDBAssetPath = assetPath.Replace(".tsx", ".asset");
        OrangeSpriteDB spriteDB = AssetDatabase.LoadAssetAtPath<OrangeSpriteDB>(spriteDBAssetPath);

        // Auto-create spriteDB if there are named sprites/animations in them.
        if (spriteDB == null) {
            bool shouldCreate = false;
            foreach (var tile in tileset.m_Tiles) {
                if (tile.m_CustomProperties.Find(p => p.m_Name == "spriteName") != null || tile.m_CustomProperties.Find(p => p.m_Name == "animationName") != null) {
                    shouldCreate = true;
                    break;
                }
            }
            if (!shouldCreate) return;

            // Create the spriteDB
            spriteDB = ScriptableObject.CreateInstance<OrangeSpriteDB>();
            AssetDatabase.CreateAsset(spriteDB, spriteDBAssetPath);
        }

        // Import named sprites (tiles with `spriteName`)
        foreach (var tile in tileset.m_Tiles) {
            var spriteName = tile.m_CustomProperties.Find(p => p.m_Name == "spriteName")?.m_Value;
            if (string.IsNullOrWhiteSpace(spriteName)) continue;
            spriteDB.EditorSetSprite(spriteName, tile.m_Sprite);
        }

        // Import named animation (tiles with `animationName`)
        foreach (var tile in tileset.m_Tiles) {
            var animationName = tile.m_CustomProperties.Find(p => p.m_Name == "animationName")?.m_Value;
            if (string.IsNullOrWhiteSpace(animationName)) continue;

            bool loop = true;
            var prop = tile.m_CustomProperties.Find(p => p.m_Name == "animationLoop");
            if (prop != null) Debug.Log(prop.m_Value);
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
            var animationName = tile.m_CustomProperties.Find(p => p.m_Name == "animationFlipName")?.m_Value;
            if (string.IsNullOrWhiteSpace(animationName)) continue;

            bool loop = true;
            var prop = tile.m_CustomProperties.Find(p => p.m_Name == "animationLoop");
            if (prop != null) Debug.Log(prop.m_Value);
            // bool.TryParse(tile.m_CustomProperties.Find(p => p.m_Name == "animationLoop")?.m_Value ?? "True", out oneShot);

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
    }

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
        foreach (var assetPath in importedAssets) {
            if (!assetPath.EndsWith(".tsx")) continue;
            ReimportTsxAssetPath(assetPath);

        }
    }
}

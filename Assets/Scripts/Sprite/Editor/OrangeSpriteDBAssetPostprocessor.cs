using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using SuperTiled2Unity.Editor;

public class OrangeSpriteDBAssetPostprocessor : AssetPostprocessor {

    public static void ReimportTsxAssetPath(string assetPath) {
        SuperTileset tileset = AssetDatabase.LoadAssetAtPath<SuperTileset>(assetPath);
        OrangeSpriteDB spriteDB = AssetDatabase.LoadAssetAtPath<OrangeSpriteDB>(assetPath.Replace(".tsx", ".asset"));
        if (spriteDB == null) return;

        foreach (var tile in tileset.m_Tiles) {
            var spriteName = tile.m_CustomProperties.Find(p => p.m_Name == "spriteName")?.m_Value;
            if (spriteName == null) continue;
            spriteDB.EditorSetSprite(spriteName, tile.m_Sprite);
        }

        foreach (var tile in tileset.m_Tiles) {
            var animationName = tile.m_CustomProperties.Find(p => p.m_Name == "animationName")?.m_Value;
            if (animationName == null) continue;

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

        foreach (var tile in tileset.m_Tiles) {
            var animationName = tile.m_CustomProperties.Find(p => p.m_Name == "animationFlipName")?.m_Value;
            if (animationName == null) continue;

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

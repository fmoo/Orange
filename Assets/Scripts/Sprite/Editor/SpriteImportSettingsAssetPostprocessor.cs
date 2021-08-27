using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class SpriteImportSettingsAssetPostprocessor : AssetPostprocessor {
    void OnPreprocessTexture() {
        // Debug.Log($"OnPreprocessTexture {assetPath} {assetImporter} {context}");
        if (assetImporter is TextureImporter textureImporter) {
            var importSettings = GetImportSettings();
            if (importSettings == null) return;

            textureImporter.spritePixelsPerUnit = importSettings.spritePixelsPerUnit;
            textureImporter.filterMode = importSettings.filterMode;
            textureImporter.maxTextureSize = importSettings.maxTextureSize;
        }
    }

    void OnPostprocessTexture(Texture2D texture) {
        var importSettings = GetImportSettings();
        if (importSettings == null) return;

        TextureImporter textureImporter = (TextureImporter)assetImporter;
        textureImporter.spritePivot = importSettings.setPivot ? importSettings.pivot : textureImporter.spritePivot;
        for (int ii = 0; ii < textureImporter.spritesheet.Length; ii++) {
            var md = textureImporter.spritesheet[ii];
            md.alignment = importSettings.setPivot ? 9 : md.alignment;
            md.pivot = importSettings.setPivot ? importSettings.pivot : md.pivot;
        }

        Reserialize();
    }

    void Reserialize() {
        // AssetDatabase.ForceReserializeAssets(new string[] { assetPath });
        // AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
        // AssetDatabase.SaveAssets();
    }

    static void OnPostprocessSpritesReal(string texturePath, Texture2D texture, Sprite[] sprites) {
        Regex digitPart = new Regex(@"\d+$", RegexOptions.Compiled);
        sprites = sprites.OrderBy(x => int.Parse(digitPart.Match(x.name).Value)).ToArray();

        // Debug.LogError($"Postprocessing {texture.name} with {sprites.Length} sprites...");

        var importSettings = GetImportSettingsForPath(texturePath);
        if (importSettings == null) return;
        if (importSettings.spriteDB == null || importSettings.syncAssets.Count == 0) return;

        foreach (var syncAsset in importSettings.syncAssets) {

            // TODO: only check textureMatch for Default.SIS?
            if (string.IsNullOrWhiteSpace(syncAsset.importName) || string.IsNullOrWhiteSpace(syncAsset.frameOffsets) || string.IsNullOrWhiteSpace(syncAsset.textureMatch)) return;
            if (!texture.name.Contains(syncAsset.textureMatch)) continue;

            var groups = syncAsset.groups.ToList();
            if (groups.Count == 0) groups.Add(new SpriteImportSettings.SpriteDBBatchGroup());

            // Debug.Log($"Importing {syncAsset.importName} with {sprites.Length} sprites.");

            foreach (var group in groups) {
                // TODO: Don't pull in duplicates if index is referenced multiple times.
                var importSprites = syncAsset.frameOffsets.Split(',').Select(s => int.Parse(s)).Select(ii => sprites[ii + syncAsset.baseIndex + group.baseOffset]);
                var importName = $"{group.prefix}{syncAsset.importName}{group.suffix}";
                importSettings.spriteDB.RemovePrefix(importName);
                importSettings.spriteDB.importSprites = importSprites.ToArray();
                importSettings.spriteDB.importTimePerFrame = syncAsset.timePerFrame;
                importSettings.spriteDB.importAnimName = importName;
                importSettings.spriteDB.loopImportedAnimation = syncAsset.loop;
                importSettings.spriteDB.flipImportedSprites = syncAsset.flip;
                importSettings.spriteDB.DoImportSprites();
            }

        }


        // Debug.Log($"OnPostprocessSprites {assetPath} {texture} {sprites} {assetImporter} {context}");

        // Sprite[] newSprites = new Sprite[sprites.Length];
        // bool anyChanged = false;
        // for (int ii = 0; ii < sprites.Length; ii++) {
        //     var sprite = sprites[ii];
        //     Vector2 newPivot = importSettings.setPivot ? importSettings.pivot : sprite.pivot;
        //     bool changed = false;
        //     changed = newPivot != sprite.pivot;
        //     anyChanged = anyChanged || changed;
        //     newSprites[ii] = changed
        //         ? Sprite.Create(texture, sprite.rect, newPivot, sprite.pixelsPerUnit, importSettings.extrude, SpriteMeshType.FullRect, sprite.border, importSettings.generateFallbackPhysicsShape)
        //         : sprite;
        // }

    }

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
        foreach (var assetPath in importedAssets) {
            if (assetPath.EndsWith(".png")) {
                Texture2D texture = (Texture2D)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Texture2D));
                Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(assetPath).OfType<Sprite>().ToArray();
                OnPostprocessSpritesReal(assetPath, texture, sprites);
            }
        }
    }



    public SpriteImportSettings GetImportSettings() {
        var assetDir = Path.GetDirectoryName(assetPath);
        SpriteImportSettings importSettings = loadAt(Path.Combine(assetDir, Path.GetFileNameWithoutExtension(assetPath) + ".SIS.asset"));
        if (importSettings != null) return importSettings;
        importSettings = loadAt(Path.Combine(assetDir, "Default.SIS.asset"));

        // TODO: Check for Default in parent directories
        return importSettings;
    }

    public static SpriteImportSettings GetImportSettingsForPath(string assetPath) {
        var assetDir = Path.GetDirectoryName(assetPath);
        SpriteImportSettings importSettings = loadAt(Path.Combine(assetDir, Path.GetFileNameWithoutExtension(assetPath) + ".SIS.asset"));
        if (importSettings != null) return importSettings;
        importSettings = loadAt(Path.Combine(assetDir, "Default.SIS.asset"));

        // TODO: Check for Default in parent directories
        return importSettings;
    }

    static SpriteImportSettings loadAt(string path) {
        var result = AssetDatabase.LoadAssetAtPath<SpriteImportSettings>(path);
        // Debug.Log($"Load({path}) -> {result}");
        return result;
    }
}

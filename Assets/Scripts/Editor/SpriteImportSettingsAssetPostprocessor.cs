using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;

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
        textureImporter.spritesheet = textureImporter.spritesheet.Select(
            md => {
                return new SpriteMetaData() {
                    name = md.name,
                    alignment = importSettings.setPivot ? 9 : md.alignment,
                    rect = md.rect,
                    pivot = importSettings.setPivot ? importSettings.pivot : md.pivot,
                    border = md.border,
                };
            }
        ).ToArray();

        Reserialize();
    }

    void Reserialize() {
        AssetDatabase.ForceReserializeAssets(new string[] { assetPath });
        // AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
        // AssetDatabase.SaveAssets();
    }

    void OnPostprocessSprites(Texture2D texture, Sprite[] sprites) {
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

    public SpriteImportSettings GetImportSettings() {
        var assetDir = Path.GetDirectoryName(assetPath);
        SpriteImportSettings importSettings = loadAt(Path.Combine(assetDir, Path.GetFileNameWithoutExtension(assetPath) + ".SIS.asset"));
        if (importSettings != null) return importSettings;
        importSettings = loadAt(Path.Combine(assetDir, "Default.SIS.asset"));

        // TODO: Check for Default in parent directories
        return importSettings;
    }

    SpriteImportSettings loadAt(string path) {
        var result = AssetDatabase.LoadAssetAtPath<SpriteImportSettings>(path);
        // Debug.Log($"Load({path}) -> {result}");
        return result;
    }
}

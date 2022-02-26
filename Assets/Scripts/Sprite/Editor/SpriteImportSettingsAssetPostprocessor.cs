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

            TextureImporterSettings texSettings = new TextureImporterSettings();
            textureImporter.ReadTextureSettings(texSettings);

            texSettings.spritePixelsPerUnit = importSettings.spritePixelsPerUnit;
            texSettings.wrapMode = importSettings.wrapMode;
            texSettings.filterMode = importSettings.filterMode;

            if (importSettings.setPivot) {
                // if (importSettings.pivot.x == 1.0f && importSettings.pivot.y == 1.0f) {
                if (importSettings.pivot.x == 0.0f && importSettings.pivot.y == 0.0f) {
                    texSettings.spriteAlignment = (int)SpriteAlignment.BottomLeft;
                } else if (importSettings.pivot.x == 0.5f && importSettings.pivot.y == 0.0f) {
                    texSettings.spriteAlignment = (int)SpriteAlignment.BottomCenter;
                } else if (importSettings.pivot.x == 1.0f && importSettings.pivot.y == 0.0f) {
                    texSettings.spriteAlignment = (int)SpriteAlignment.BottomRight;
                } else if (importSettings.pivot.x == 0.0f && importSettings.pivot.y == 0.5f) {
                    texSettings.spriteAlignment = (int)SpriteAlignment.LeftCenter;
                } else if (importSettings.pivot.x == 0.5f && importSettings.pivot.y == 0.5f) {
                    texSettings.spriteAlignment = (int)SpriteAlignment.Center;
                } else if (importSettings.pivot.x == 1.0f && importSettings.pivot.y == 0.5f) {
                    texSettings.spriteAlignment = (int)SpriteAlignment.RightCenter;
                } else if (importSettings.pivot.x == 0.0f && importSettings.pivot.y == 1.0f) {
                    texSettings.spriteAlignment = (int)SpriteAlignment.TopLeft;
                } else if (importSettings.pivot.x == 0.5f && importSettings.pivot.y == 1.0f) {
                    texSettings.spriteAlignment = (int)SpriteAlignment.TopCenter;
                } else if (importSettings.pivot.x == 1.0f && importSettings.pivot.y == 1.0f) {
                    texSettings.spriteAlignment = (int)SpriteAlignment.TopRight;
                } else {
                    texSettings.spriteAlignment = (int)SpriteAlignment.Custom;
                    texSettings.spritePivot = importSettings.pivot;
                }
            }
            textureImporter.SetTextureSettings(texSettings);

            textureImporter.spritePixelsPerUnit = importSettings.spritePixelsPerUnit;
            textureImporter.wrapMode = importSettings.wrapMode;
            textureImporter.filterMode = importSettings.filterMode;
            textureImporter.maxTextureSize = importSettings.maxTextureSize;
            textureImporter.isReadable = importSettings.readWriteFromScript;
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

        // Debug.LogError($"Postprocessing {texture.name} with {sprites.Length} sprites...");

        var importSettings = GetImportSettingsForPath(texturePath);
        if (importSettings == null) return;
        if (importSettings.spriteDB == null || importSettings.syncAssets.Count == 0) return;

        Regex digitPart = new Regex(@"\d+$", RegexOptions.Compiled);
        try {
            sprites = sprites.OrderBy(x => int.Parse(digitPart.Match(x.name).Value)).ToArray();
        } catch (System.FormatException) {
            Debug.LogWarning($"{texturePath} sprites did not contain _# numeric suffix.  Import may fail.");
        }
        bool doReimportThumbnail = false;

        foreach (var syncAsset in importSettings.syncAssets) {

            // TODO: only check textureMatch for Default.SIS?
            if (string.IsNullOrWhiteSpace(syncAsset.importName)
            || (string.IsNullOrWhiteSpace(syncAsset.frameOffsets) && syncAsset.textureMatch != "*")
            || (string.IsNullOrWhiteSpace(syncAsset.textureMatch))
            ) {
                Debug.Log($"Invalid sync settings on {importSettings.name}. Aborting...", importSettings);
                return;
            }
            if (!texture.name.Contains(syncAsset.textureMatch) && syncAsset.textureMatch != "*") {
                continue;
            }

            var groups = syncAsset.groups.ToList();
            if (groups.Count == 0) groups.Add(new SpriteImportSettings.SpriteDBBatchGroup());

            // Debug.Log($"Importing {syncAsset.importName} with {sprites.Length} sprites.");

            foreach (var group in groups) {
                // TODO: Don't pull in duplicates if index is referenced multiple times.
                IEnumerable<Sprite> importSprites;
                if (syncAsset.frameOffsets != "" || syncAsset.textureMatch != "*") {
                    importSprites = syncAsset.frameOffsets.Split(',').Select(s => int.Parse(s)).Select(ii => {
                        int index = ii + syncAsset.baseIndex + group.baseOffset;
                        if (index < 0 || index >= sprites.Length) {
                            Debug.LogError($"Index {index} out of range when parsing frameOffset {ii} with baseIndex {syncAsset.baseIndex} and group offset {group.baseOffset} for {texturePath}", importSettings);
                        }
                        return sprites[index];
                    });
                } else {
                    importSprites = sprites;
                }
                var importName = syncAsset.textureMatch == "*" ? texture.name : syncAsset.importName;
                foreach (var replaceRule in syncAsset.replaceRules) {
                    importName = importName.Replace(replaceRule.match, replaceRule.replace);
                }

                bool didHaveSprite = importSettings.spriteDB.sprites.FirstOrDefault(s => s.name == importSettings.autoCropThumbnailFrame) != null;
                importName = $"{group.prefix}{importName}{group.suffix}";
                importSettings.spriteDB.RemovePrefix(importName);

                if (syncAsset.textureMatch == "*" && importSprites.Count() == 1) {
                    importSettings.spriteDB.sprites.Add(
                        new OrangeSpriteManagerSprite() {
                            name = importName,
                            sprite = importSprites.First(),
                        }
                    );
                } else {
                    importSettings.spriteDB.importSprites = importSprites.ToArray();
                    importSettings.spriteDB.importTimePerFrame = syncAsset.timePerFrame;
                    importSettings.spriteDB.importAnimName = importName;
                    importSettings.spriteDB.loopImportedAnimation = syncAsset.loop;
                    importSettings.spriteDB.flipImportedSprites = syncAsset.flip;
                    importSettings.spriteDB.DoImportSprites();
                    EditorUtility.SetDirty(importSettings.spriteDB);
                }

                if (!didHaveSprite && importSettings.spriteDB.sprites.FirstOrDefault(s => s.name == importSettings.autoCropThumbnailFrame) != null) {
                    doReimportThumbnail = true;
                }
            }

        }

        if (doReimportThumbnail && !string.IsNullOrWhiteSpace(importSettings.autoCropThumbnailFrame) && importSettings.spriteDB != null) {
            var spriteDBAssetPath = AssetDatabase.GetAssetPath(importSettings.spriteDB);
            var thumbnailAssetPath = spriteDBAssetPath.Substring(0, spriteDBAssetPath.Length - 5) + "png";

            var oldThumbnailTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(thumbnailAssetPath);
            var oldThumbnailTextureDefault = AssetDatabase.LoadAssetAtPath<DefaultAsset>(thumbnailAssetPath);

            var thumbnailSprite = importSettings.spriteDB.sprites.FirstOrDefault(s => s.name == importSettings.autoCropThumbnailFrame)?.sprite;
            if (thumbnailSprite == null) {
                Debug.LogWarning($"No thumbnail asset found for '{importSettings.autoCropThumbnailFrame}'", importSettings.spriteDB);
                return;
            }

            var oldPixels = oldThumbnailTexture == null ? null : oldThumbnailTexture.GetPixels(0, 0, oldThumbnailTexture.width, oldThumbnailTexture.height);

            var pixels = thumbnailSprite.texture.GetPixels(
                (int)thumbnailSprite.textureRect.x,
                (int)thumbnailSprite.textureRect.y,
                (int)thumbnailSprite.textureRect.width,
                (int)thumbnailSprite.textureRect.height);
            if (oldThumbnailTexture == null || !ArrayUtility.Equals(oldPixels, pixels)) {
                var thumbnailTexture = new Texture2D((int)thumbnailSprite.textureRect.width, (int)thumbnailSprite.textureRect.height);
                thumbnailTexture.SetPixels(pixels);
                thumbnailTexture.Apply();

                if (oldThumbnailTextureDefault != null) {
                    Debug.Log($"Deleting old {thumbnailAssetPath}");
                    AssetDatabase.DeleteAsset(thumbnailAssetPath);
                    AssetDatabase.SaveAssets();
                }
                Debug.Log($"Writing {thumbnailAssetPath}");
                File.WriteAllBytes(thumbnailAssetPath, ImageConversion.EncodeToPNG(thumbnailTexture));
                AssetDatabase.ImportAsset(thumbnailAssetPath);
            }
            AssetDatabase.SaveAssets();
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

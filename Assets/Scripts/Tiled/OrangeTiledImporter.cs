using UnityEngine;
using SuperTiled2Unity;
using SuperTiled2Unity.Editor;
using UnityEngine.Tilemaps;
using UnityEditor;

[AutoCustomTmxImporter()]
abstract public class OrangeTiledImporter : CustomTmxImporter {

    virtual protected void PostProcessSuperObject(SuperObject customObject, string mapName) {}
    virtual protected void PostProcessSuperLayer(SuperLayer layer, string mapName) {}

    public override void TmxAssetImported(TmxAssetImportedArgs args) {
        foreach (var customObjectSet in args.ImportedSuperMap.GetComponentsInChildren<SuperObject>()) {
            SetObjectLayerRenderOverrides(customObjectSet);
            PostProcessSuperObject(customObjectSet, args.ImportedSuperMap.name);
        }
        foreach (var layer in args.ImportedSuperMap.GetComponentsInChildren<SuperLayer>()) {
            if (IsParallaxLayer(layer)) {
                InitParallaxLayer(layer, args.AssetImporter.PixelsPerUnit);
            }
            PostProcessSuperLayer(layer, args.ImportedSuperMap.name);
        }
    }

    bool IsParallaxLayer(SuperLayer layer) {
        if (layer.m_ParallaxX != 1f || layer.m_ParallaxY != 1f) {
            return true;
        }
        if (layer is SuperImageLayer imageLayer) {
            var props = imageLayer.GetComponent<SuperCustomProperties>();
            if (props == null) return false;
            CustomProperty prop;
            if (props.TryGetCustomProperty("autoscrollX", out prop)) {
                if (prop.GetValueAsFloat() != 0f) return true;
            }
            if (props.TryGetCustomProperty("autoscrollY", out prop)) {
                if (prop.GetValueAsFloat() != 0f) return true;
            }
        }
        return false;
    }

    void InitParallaxLayer(SuperLayer layer, float pixelsPerUnit) {
        CustomProperty prop;
        var props = layer.GetComponent<SuperCustomProperties>();

        if (props.TryGetCustomProperty("noParallax", out prop)) {
            if (prop.GetValueAsBool()) {
                return;
            }
        }

        Vector2Int screenDimension = new Vector2Int(320, 180);

        bool didAddParallaxHandler = false;
        if (layer is SuperImageLayer) {
            var mapProps = layer.GetComponentInParent<SuperMap>().GetComponent<SuperCustomProperties>();

            Vector2 autoscrollSpeed = new Vector2();
            var spriteRenderer = layer.GetComponent<SpriteRenderer>();
            var rawImage = layer.gameObject.AddComponent<UnityEngine.UI.RawImage>();
            rawImage.color = spriteRenderer.color;
            rawImage.texture = spriteRenderer.sprite.texture;
            Rect uvRect = rawImage.uvRect;
            // Debug.LogWarning($"rawImage uvRect original dimensions are {rawImage.uvRect.width}x{rawImage.uvRect.height}");
            // Debug.LogWarning($"texture dimensions are {rawImage.texture.width}x{rawImage.texture.height}");
            // Debug.LogWarning($"screen dimensions are {screenDimension.x}x{screenDimension.y}");
            uvRect.width = 1f / ((float)rawImage.texture.width / (float)screenDimension.x);
            uvRect.height = 1f / ((float)rawImage.texture.height / (float)screenDimension.y);

            if (props.TryGetCustomProperty("scaleU", out prop)) {
                uvRect.width *= prop.GetValueAsFloat();
            }
            if (props.TryGetCustomProperty("scaleV", out prop)) {
                uvRect.height *= prop.GetValueAsFloat();
            }
            rawImage.uvRect = uvRect;
            // Debug.LogWarning($"final uvRect dimensions are {rawImage.uvRect.width}x{rawImage.uvRect.height}");

            var canvas = layer.gameObject.AddComponent<Canvas>();
            var backgroundUILayer = LayerMask.NameToLayer("BackgroundUI");
            if (backgroundUILayer != -1) {
                layer.gameObject.layer = backgroundUILayer;
            }
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.sortingLayerID = spriteRenderer.sortingLayerID;
            canvas.sortingOrder = spriteRenderer.sortingOrder;

            if (props.TryGetCustomProperty("autoscrollX", out prop)) {
                autoscrollSpeed.x = prop.GetValueAsFloat();
            }
            if (props.TryGetCustomProperty("autoscrollY", out prop)) {
                autoscrollSpeed.y = prop.GetValueAsFloat();
            }
            var cameraScroll = new Vector2(layer.m_ParallaxX, layer.m_ParallaxY);

            if (autoscrollSpeed != Vector2.zero || cameraScroll != Vector2.one) {
                var fx = layer.gameObject.AddComponent<ParallaxFX>();
                fx.canvas = canvas;
                fx.image = rawImage;
                fx.scrollSpeed = autoscrollSpeed;
                fx.cameraScroll = new Vector2(layer.m_ParallaxX, layer.m_ParallaxY);
                // fx.baseOffset.x = (layer.m_OffsetX / screenDimension.x) + 0.5f;
                // fx.baseOffset.y = (layer.m_OffsetY / screenDimension.y) + 0.5f;
                // fx.cameraScroll = new Vector2(1f - layer.m_ParallaxX, 1f - layer.m_ParallaxY);
                fx.baseOffset = new Vector2(-layer.m_OffsetX, layer.m_OffsetY);
                fx.pixelsPerUnit = pixelsPerUnit;

                // divide fx.baseOffset by image size in pixels
                fx.baseOffset.x /= rawImage.texture.width;
                fx.baseOffset.y /= rawImage.texture.height;

                spriteRenderer.enabled = false;
                didAddParallaxHandler = true;
            }
        }

        if (!didAddParallaxHandler) {
            var parallaxEffect = layer.gameObject.AddComponent<ParallaxLayerHandler>();
            parallaxEffect.layer = layer;
        }
    }

    void SetObjectLayerRenderOverrides(SuperObject customObject) {
        CustomProperty prop;
        GameObject obj = customObject.gameObject;

        var customProperties = customObject.GetComponent<SuperCustomProperties>();
        var sr = obj.GetComponentInChildren<SpriteRenderer>();

        if (sr != null) {
            var layer = obj.GetComponentInParent<SuperLayer>();
            var layerProperties = layer.GetComponent<SuperCustomProperties>();

            if (customProperties.TryGetCustomProperty("unity:SortingLayer", out prop)) {
                sr.sortingLayerName = prop.GetValueAsString();
            } else if (layerProperties.TryGetCustomProperty("unity:SortingLayer", out prop)) {
                sr.sortingLayerName = prop.GetValueAsString();
            }
            if (customProperties.TryGetCustomProperty("unity:SortingOrder", out prop)) {
                sr.sortingOrder = prop.GetValueAsInt();
            } else if (layerProperties.TryGetCustomProperty("unity:SortingOrder", out prop)) {
                sr.sortingOrder = prop.GetValueAsInt();
            }
        }
    }
}
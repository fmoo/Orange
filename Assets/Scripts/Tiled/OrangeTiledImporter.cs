using UnityEngine;
using SuperTiled2Unity;
using SuperTiled2Unity.Editor;
using UnityEngine.Tilemaps;
using UnityEditor;

[AutoCustomTmxImporter()]
abstract public class OrangeTiledImporter : CustomTmxImporter {

    abstract protected void PostProcessSuperObject(SuperObject customObject);

    public override void TmxAssetImported(TmxAssetImportedArgs args) {
        foreach (var customObjectSet in args.ImportedSuperMap.GetComponentsInChildren<SuperObject>()) {
            SetObjectLayerRenderOverrides(customObjectSet);
            PostProcessSuperObject(customObjectSet);
        }
        foreach (var layer in args.ImportedSuperMap.GetComponentsInChildren<SuperLayer>()) {
            if (layer is SuperImageLayer || layer.m_ParallaxX != 1f || layer.m_ParallaxY != 1f) {
                InitParallaxLayer(layer);
            }
        }
    }


    void InitParallaxLayer(SuperLayer layer) {
        CustomProperty prop;
        var props = layer.GetComponent<SuperCustomProperties>();

        if (props.TryGetCustomProperty("noParallax", out prop)) {
            if (prop.GetValueAsBool()) {
                return;
            }
        }

        Vector2Int screenDimension = new Vector2Int(320, 180);

        var parallaxEffect = layer.gameObject.AddComponent<ParallaxLayerHandler>();
        parallaxEffect.layer = layer;


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
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.sortingLayerID = spriteRenderer.sortingLayerID;
            canvas.sortingOrder = spriteRenderer.sortingOrder;

            if (props.TryGetCustomProperty("autoscrollX", out prop)) {
                autoscrollSpeed.x = prop.GetValueAsFloat();
            }
            if (props.TryGetCustomProperty("autoscrollY", out prop)) {
                autoscrollSpeed.y = prop.GetValueAsFloat();
            }

            var fx = layer.gameObject.AddComponent<ParallaxFX>();
            fx.image = rawImage;
            fx.scrollSpeed = autoscrollSpeed;
            fx.cameraScroll = new Vector2(1f - layer.m_ParallaxX, 1f - layer.m_ParallaxY);

            spriteRenderer.enabled = false;
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
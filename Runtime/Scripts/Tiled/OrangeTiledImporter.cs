using SuperTiled2Unity;
using SuperTiled2Unity.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[AutoCustomTmxImporter()]
abstract public class OrangeTiledImporter : CustomTmxImporter {

    virtual protected void PostProcessSuperObject(SuperObject customObject, string mapName) { }
    virtual protected void PostProcessSuperLayer(SuperLayer layer, string mapName) { }

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
            if (imageLayer.m_RepeatX || imageLayer.m_RepeatY) {
                return true;
            }
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
        var props = layer.GetComponent<SuperCustomProperties>();
        if (props == null || props.GetBool("noParallax")) {
            return;
        }

        var mapProps = layer.GetComponentInParent<SuperMap>().GetComponent<SuperCustomProperties>();

        var spriteRenderer = layer.GetComponent<SpriteRenderer>();

        Vector2 autoscrollSpeed = Vector2.zero;
        autoscrollSpeed.x = props.GetFloat("autoscrollX", 0f);
        autoscrollSpeed.y = props.GetFloat("autoscrollY", 0f);

        var parallaxEffect = layer.gameObject.AddComponent<ParallaxLayerHandler>();
        parallaxEffect.autoScrollSpeed = autoscrollSpeed;
        if (layer is SuperImageLayer imageLayer) {
            parallaxEffect.repeatsX = imageLayer.m_RepeatX;
            parallaxEffect.repeatsY = imageLayer.m_RepeatY;
        }
        parallaxEffect.layer = layer;
        parallaxEffect.spriteRenderer = spriteRenderer;
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
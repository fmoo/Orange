using SuperTiled2Unity;
using SuperTiled2Unity.Editor;


public static class TiledEditorExtensions {
    public static bool HasProp(this SuperTileset obj, string name) {
        return obj.m_CustomProperties.Find(x => x.m_Name == name) != null;
    }

    public static bool GetBoolProp(this SuperTileset obj, string name, bool defaultValue = false) {
        var prop = obj.m_CustomProperties.Find(x => x.m_Name == name);
        if (prop != null) {
            return prop.GetValueAsBool();
        }
        return defaultValue;
    }

    public static string GetStringProp(this SuperTileset obj, string name, string defaultValue = "") {
        var prop = obj.m_CustomProperties.Find(x => x.m_Name == name);
        if (prop != null) {
            return prop.GetValueAsString();
        }
        return defaultValue;
    }

    public static float GetFloatProp(this SuperTileset obj, string name, float defaultValue = 0f) {
        var prop = obj.m_CustomProperties.Find(x => x.m_Name == name);
        if (prop != null) {
            return prop.GetValueAsFloat();
        }
        return defaultValue;
    }

    public static int GetIntProp(this SuperTileset obj, string name, int defaultValue = 0) {
        var prop = obj.m_CustomProperties.Find(x => x.m_Name == name);
        if (prop != null) {
            return prop.GetValueAsInt();
        }
        return defaultValue;
    }

}
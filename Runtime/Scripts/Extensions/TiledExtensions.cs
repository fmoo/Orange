using SuperTiled2Unity;


public static class TiledExtensions {
    public static bool Contains(this SuperCustomProperties properties, string name) {
        return properties.TryGetCustomProperty(name, out _);
    }

    public static bool GetBool(this SuperCustomProperties properties, string name, bool defaultValue = false) {
        if (properties.TryGetCustomProperty(name, out CustomProperty property)) {
            return property.GetValueAsBool();
        }
        return defaultValue;
    }

    public static string GetString(this SuperCustomProperties properties, string name, string defaultValue = "") {
        if (properties.TryGetCustomProperty(name, out CustomProperty property)) {
            return property.GetValueAsString();
        }
        return defaultValue;
    }

    public static float GetFloat(this SuperCustomProperties properties, string name, float defaultValue = 0f) {
        if (properties.TryGetCustomProperty(name, out CustomProperty property)) {
            return property.GetValueAsFloat();
        }
        return defaultValue;
    }

    public static int GetInt(this SuperCustomProperties properties, string name, int defaultValue = 0) {
        if (properties.TryGetCustomProperty(name, out CustomProperty property)) {
            return property.GetValueAsInt();
        }
        return defaultValue;
    }

    public static bool ContainsProp(this SuperTile tile, string name) {
        return tile.m_CustomProperties.Find(x => x.m_Name == name) != null;
    }

    public static bool GetBoolProp(this SuperTile tile, string name, bool defaultValue = false) {
        var prop = tile.m_CustomProperties.Find(x => x.m_Name == name);
        if (prop != null) {
            return prop.GetValueAsBool();
        }
        return defaultValue;
    }

    public static string GetStringProp(this SuperTile tile, string name, string defaultValue = "") {
        var prop = tile.m_CustomProperties.Find(x => x.m_Name == name);
        if (prop != null) {
            return prop.GetValueAsString();
        }
        return defaultValue;
    }

    public static float GetFloatProp(this SuperTile tile, string name, float defaultValue = 0f) {
        var prop = tile.m_CustomProperties.Find(x => x.m_Name == name);
        if (prop != null) {
            return prop.GetValueAsFloat();
        }
        return defaultValue;
    }

    public static int GetIntProp(this SuperTile tile, string name, int defaultValue = 0) {
        var prop = tile.m_CustomProperties.Find(x => x.m_Name == name);
        if (prop != null) {
            return prop.GetValueAsInt();
        }
        return defaultValue;
    }

}
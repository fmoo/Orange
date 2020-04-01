using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ScriptableObjectReference : ISerializationCallbackReceiver {
    [NaughtyAttributes.ReadOnly] public string guid;
    public SerializableScriptableObject value;

    public ScriptableObjectReference(SerializableScriptableObject value) {
        this.value = value;
        this.guid = value?.guid ?? "";
    }

    static Dictionary<string, SerializableScriptableObject> scriptableObjectCache;
    public static void InitScriptableObjectCache() {
        if (scriptableObjectCache != null) return;
        // This might throw, so load before initializing the dictionary reference.
        var resources = Resources.LoadAll<SerializableScriptableObject>("");
        scriptableObjectCache = new Dictionary<string, SerializableScriptableObject>();
        foreach (var resource in resources) {
            Debug.Log($"Got Resource {resource.name} with guid {resource.guid}");
            if (resource.guid != "") {
                scriptableObjectCache[resource.guid] = resource;
            }
        }
    }
    public static T GetFromGUID<T>(string guid) where T : SerializableScriptableObject {
        InitScriptableObjectCache();
        if (scriptableObjectCache.TryGetValue(guid, out SerializableScriptableObject value)) {
            return value as T;
        }
        return null;
    }

    public static bool loggedDeserializeError = false;

    public void OnAfterDeserialize() {
        if (scriptableObjectCache == null) {
            try {
                InitScriptableObjectCache();
            } catch (UnityException e) {
                if (!loggedDeserializeError) {
                    Debug.LogWarning(e);
                    loggedDeserializeError = true;
                }
                return;
            }
        }
        if (scriptableObjectCache.TryGetValue(guid, out SerializableScriptableObject value)) {
            if (value != null) {
                this.value = value;
            } else {
                // Debug.LogWarning($"GUID cache contained null for '{guid}'?!");
            }
        } else {
            // Debug.LogWarning($"No scriptable objects found for GUID '{guid}'");
        }
    }

    public void OnBeforeSerialize() {
        guid = value?.guid ?? "";

    }

    void OnValidate() {
        guid = value?.guid ?? "";
    }
}


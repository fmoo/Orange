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
        scriptableObjectCache = new Dictionary<string, SerializableScriptableObject>();
        foreach (var resource in Resources.LoadAll<SerializableScriptableObject>("")) {
            Debug.Log($"Got Resource {resource.name} with guid {resource.guid}");
            if (resource.guid != "") {
                scriptableObjectCache[resource.guid] = resource;
            }
        }
    }

    public void OnAfterDeserialize() {
        if (scriptableObjectCache == null) {
            // Debug.LogWarning("Attempt to deserialize using null GUID cache!");
            return;
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


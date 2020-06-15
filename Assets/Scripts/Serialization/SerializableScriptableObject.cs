using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerializableScriptableObject : ScriptableObject {
    [NaughtyAttributes.ReadOnly] public string guid;

#if EDITOR
    void OnValidate() {
        UnityEditor.AssetDatabase.TryGetGUIDAndLocalFileIdentifier(this, out string theGuid, out long theLocalId);
        guid = theGuid;
    }
#endif
}
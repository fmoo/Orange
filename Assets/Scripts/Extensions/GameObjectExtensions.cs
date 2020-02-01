using UnityEngine;
using UnityEditor;
using System.Collections;

public static class GameObjectExtensions {

	public static GameObject FindObject(this GameObject o, string name) {
		return o.transform.Find(name)?.gameObject;
	}

	public static T GetOrCreateComponent<T>(this GameObject o) where T : Component  {
		T component = o.GetComponent<T> ();
		if (component == null) {
			component = o.AddComponent<T>();
		}
		return component;
	}

	public static GameObject GetOrCreateGameObject(this GameObject o, string name) {
		Transform t = o.transform.Find (name);
		if (t != null) {
			return t.gameObject;
		} else {
			GameObject o2 = new GameObject(name);
			o2.transform.SetParent(o.transform);
			o2.transform.localPosition = Vector3.zero;
			o2.transform.localScale = Vector3.one;
			return o2;
		}
    }

#if UNITY_EDITOR
    /// <summary>
    /// Get string representation of serialized property, even for non-string fields
    /// </summary>
    public static string AsStringValue(this SerializedProperty property) {
        switch (property.propertyType) {
            case SerializedPropertyType.String:
                return property.stringValue;

            case SerializedPropertyType.Character:
            case SerializedPropertyType.Integer:
                if (property.type == "char") return System.Convert.ToChar(property.intValue).ToString();
                return property.intValue.ToString();

            case SerializedPropertyType.ObjectReference:
                return property.objectReferenceValue != null ? property.objectReferenceValue.ToString() : "null";

            case SerializedPropertyType.Boolean:
                return property.boolValue.ToString();

            case SerializedPropertyType.Enum:
                return property.enumNames[property.enumValueIndex];

            default:
                return string.Empty;
        }
    }
#endif
}

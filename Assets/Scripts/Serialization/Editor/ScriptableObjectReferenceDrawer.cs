using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ScriptableObjectReference))]
[CustomPropertyDrawer(typeof(SORefTypeAttribute))]
public class ScriptableObjectReferenceDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        var type = typeof(SerializableScriptableObject);
        if (attribute is SORefTypeAttribute attributeSoTypeRef) {
            type = attributeSoTypeRef.type;
        }

        SerializedProperty valueProp = property.FindPropertyRelative("value");
        valueProp.objectReferenceValue = EditorGUI.ObjectField(position, label, valueProp.objectReferenceValue, type, false);
    }
}

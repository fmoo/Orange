using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[CustomEditor(typeof(OrangeButton))]
public class OrangeButtonEditor : UnityEditor.UI.ButtonEditor {

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		OrangeButton button = target as OrangeButton;

		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("onSelect"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("onDeselect"));

		EditorGUILayout.PropertyField(serializedObject.FindProperty("upSelectable"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("leftSelectable"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("rightSelectable"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("downSelectable"));

		EditorGUILayout.PropertyField(serializedObject.FindProperty("activateGameObjectsOnFocus"));
		serializedObject.ApplyModifiedProperties();
	}
}

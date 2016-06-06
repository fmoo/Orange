using UnityEngine;
using System.Collections;

public static class ComponentUtils {

	public static T GetOrCreateComponent<T>(this Component c) where T : Component  {
		T component = c.GetComponent<T> ();
		if (component == null) {
			component = c.gameObject.AddComponent<T>();
		}
		return component;
	}
	
	public static GameObject GetOrCreateGameObject(this Component c, string name) {
		Transform t = c.transform.Find (name);
		if (t != null) {
			return t.gameObject;
		} else {
			GameObject g = new GameObject(name);
			g.transform.SetParent(c.transform);
			g.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
			g.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
			return g;
		}
	}
	
}
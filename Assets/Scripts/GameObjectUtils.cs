using UnityEngine;
using System.Collections;

public static class GameObjectUtils {
	
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
			o2.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
			return o2;
		}
	}
}
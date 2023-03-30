using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public static class SceneExtensions {
    public static IEnumerable<T> GetComponents<T>(this Scene s) where T : Component {
        foreach (var gameObject in s.GetRootGameObjects()) {
            var components = gameObject.GetComponentsInChildren<T>();
            foreach (var component in components)
                yield return component;
        }
    }

    public static T GetComponent<T>(this Scene s) where T : Component {
        foreach (var gameObject in s.GetRootGameObjects()) {
            var component = gameObject.GetComponentInChildren<T>();
            if (component != null) return component;
        }
        return null;
    }
}

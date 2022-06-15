using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

abstract public class OrangeYarnExtension : MonoBehaviour {

    abstract public void ConfigureRunner(DialogueRunner runner);

    protected GameObject RequireObject(string objectId) {
        var result = GameObject.Find(objectId);
        Debug.Assert(result != null, $"Unable to find gameObject with name, '{objectId}'", this);
        return result;
    }

    protected bool TryFindObject(string objectId, out GameObject outObj) {
        outObj = GameObject.Find(objectId);
        return outObj != null;
    }

    protected T RequireComponent<T>(string objectId) where T : Component {
        var gameObject = RequireObject(objectId);
        if (gameObject.TryGetComponent<T>(out var component)) {
            return component;
        }
        Debug.Assert(false, $"Unable to find component of type {typeof(T).ToString()} with name, '{objectId}'", this);
        return null;
    }

    protected bool TryFindComponent<T>(string objectId, out T outValue) where T : Component {
        if (TryFindObject(objectId, out var gameObject)) {
            return gameObject.TryGetComponent<T>(out outValue);
        }
        outValue = null;
        return false;
    }
}

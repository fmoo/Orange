using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

abstract public class OrangeYarnExtension : MonoBehaviour {

    abstract public void ConfigureRunner(DialogueRunner runner);

    public bool TryFindComponent<T>(string objectId, out T value) where T : Component {
        var gameObject = GameObject.Find(objectId);
        return gameObject.TryGetComponent<T>(out value);
    }
}

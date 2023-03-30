using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideOnStart : MonoBehaviour {

    // Use this for initialization
    void Start () {
        var renderer = GetComponent<Renderer>();
        if (renderer) {
            Destroy(renderer);
        }
    }
}

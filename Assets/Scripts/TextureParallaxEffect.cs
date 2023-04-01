using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextureParallaxEffect : MonoBehaviour {

    void Update() {
        var camera = Camera.main;
        RawImage ri = GetComponent<RawImage>();
        ri.uvRect = new Rect() {
            x = (camera.transform.eulerAngles.y / 360f) - (camera.transform.position.x / 100f),
            y = 0,
            width = ri.uvRect.width,
            height = ri.uvRect.height,
            //width = Screen.width / (float)Screen.height,
            //height = 1f
        };
    }

}

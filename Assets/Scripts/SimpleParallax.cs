using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleParallax : MonoBehaviour {
    public Vector2 scrollFactor = 0.1f * Vector2.one;
    public Vector2Bool applyScroll = Vector2Bool.X;

    public Camera targetCamera;
    public RawImage rawImage;

    void OnValidate() {
        if (targetCamera == null)
            targetCamera = Camera.current ?? Camera.main;
        if (rawImage == null)
            rawImage = GetComponent<RawImage>();
    }

    void Start() {
        OnValidate();
    }

    public void Update() {
        if (rawImage == null || targetCamera == null) return;
        var uvRect = rawImage.uvRect;
        if (applyScroll.x)
            uvRect.x = scrollFactor.x * targetCamera.transform.position.x;
        if (applyScroll.y)
            uvRect.y = scrollFactor.y * targetCamera.transform.position.y;
        rawImage.uvRect = uvRect;
    }
}


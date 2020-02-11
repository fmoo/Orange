using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class OrangeCanvasHelper : MonoBehaviour {
    new public UnityEngine.U2D.PixelPerfectCamera camera;
    private CanvasScaler canvasScaler;
    private int lastHeight;
    private int lastReferenceHeight;

    void Start() {
        DoUpdateSize();
    }

    void Awake() {
        DoUpdateSize();
    }

    void Update() {
        DoUpdateSize();
    }

    void DoUpdateSize() {
        if (camera == null) return;
        var referenceHeight = camera.refResolutionY;
        if (Screen.height == lastHeight && referenceHeight == lastReferenceHeight) return;
        if (referenceHeight <= 0) return;
        Debug.Log(Screen.height + "..." + lastHeight);
        canvasScaler = gameObject.GetOrCreateComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
        canvasScaler.scaleFactor = Screen.height / referenceHeight;
        Debug.Log("Height=" + Screen.height + "  refHeight=" + referenceHeight);
        lastHeight = Screen.height;
        lastReferenceHeight = referenceHeight;
    }
}

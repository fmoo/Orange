using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrangeDisplay : MonoBehaviour {
    public Camera orthographicCamera;

    public bool IsValid = false;
    private int LastWidth;
    private int LastHeight;
    private float? LastOrthographicSize = null;

    public System.Action OnResolutionChanged = null;

    void Start() {
        IsValid = true;
    }

    // Update is called once per frame
    void Update() {
        var width = Screen.width;
        var height = Screen.height;
        var orthographicSize = orthographicCamera?.orthographicSize;
        if (LastWidth != width || LastHeight != height || LastOrthographicSize != orthographicSize) {
            OnResolutionChanged?.Invoke();
            LastWidth = width;
            LastHeight = height;
            LastOrthographicSize = orthographicSize;
        }
    }
}

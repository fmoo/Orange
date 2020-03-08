using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrangeDisplay : MonoBehaviourSingleton<OrangeDisplay> {
    [SerializeField] Camera orthographicCamera;

    private int LastWidth;
    private int LastHeight;
    private float? LastOrthographicSize = null;

    public System.Action OnResolutionChanged = null;

    // Update is called once per frame
    void Update() {
        var width = Screen.width;
        var height = Screen.height;
        if (orthographicCamera == null)
            orthographicCamera = Camera.current;
        var orthographicSize = orthographicCamera?.orthographicSize;
        if (LastWidth != width || LastHeight != height || LastOrthographicSize != orthographicSize) {
            OnResolutionChanged?.Invoke();
            LastWidth = width;
            LastHeight = height;
            LastOrthographicSize = orthographicSize;
        }
    }
}

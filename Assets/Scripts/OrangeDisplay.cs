using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrangeDisplay : MonoBehaviour {
    public int Width;
    public int Height;
    public bool IsValid = false;
    private int LastWidth;
    private int LastHeight;

    public System.Action OnResolutionChanged = null;

    void Start() {
        Width = Screen.width;
        Height = Screen.height;
        IsValid = true;
    }

    // Update is called once per frame
    void Update() {
        Width = Screen.width;
        Height = Screen.height;
        if (LastWidth != Width || LastHeight != Height) {
            OnResolutionChanged?.Invoke();
            LastWidth = Width;
            LastHeight = Height;
        }
    }
}

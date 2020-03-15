using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public static class RectTransformExtensions {

    public static void Clear(this RectTransform rectTransform) {
        foreach (RectTransform child in rectTransform) {
            Object.Destroy(child.gameObject);
        }
    }

    public static Vector3[] GetWorldCorners(this RectTransform rect) {
        Vector3[] corners = new Vector3[4];
        rect.GetWorldCorners(corners);
        return corners;
    }

    static Vector3[] _corners = new Vector3[4];
    public static Vector3 GetWorldCenter(this RectTransform rect) {
        rect.GetWorldCorners(_corners);
        return (_corners[1] + _corners[3]) / 2.0f;
    }

}

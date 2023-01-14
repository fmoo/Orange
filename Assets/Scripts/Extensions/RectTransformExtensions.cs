using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public static class RectTransformExtensions {

    public static void Clear(this RectTransform rectTransform) {
        foreach (RectTransform child in rectTransform) {
            // if (child.GetComponent<OrangeCursor>() != null) {
            //     continue;
            // }
            if (Application.isPlaying) {
                Object.Destroy(child.gameObject);
            } else {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.delayCall += () => {
                    if (child?.gameObject != null)
                        Object.DestroyImmediate(child.gameObject);
                };
#endif
            }
        }
    }

    public static Vector3[] GetWorldCorners(this RectTransform rect) {
        Vector3[] corners = new Vector3[4];
        rect.GetWorldCorners(corners);
        return corners;
    }

    public static Corners GetWorldCornersStruct(this RectTransform rect) {
        var corners = rect.GetWorldCorners();
        return new Corners(corners);
    }

    public static Vector3[] GetLocalCorners(this RectTransform rect) {
        Vector3[] corners = new Vector3[4];
        rect.GetLocalCorners(corners);
        return corners;
    }

    public static Corners GetLocalCornersStruct(this RectTransform rect) {
        var corners = rect.GetLocalCorners();
        return new Corners(corners);
    }

    public class Corners {
        public Corners(Vector3[] corners) {
            LowerLeft = corners[0];
            UpperLeft = corners[1];
            UpperRight = corners[2];
            LowerRight = corners[3];
        }
        public readonly Vector3 UpperLeft = Vector3.zero;
        public readonly Vector3 UpperRight = Vector3.zero;
        public readonly Vector3 LowerLeft = Vector3.zero;
        public readonly Vector3 LowerRight = Vector3.zero;

        public Vector3 MiddleLeft => (UpperLeft + LowerLeft) / 2f;
        public Vector3 MiddleRight => (UpperRight + LowerRight) / 2f;
        public Vector3 TopCenter => (UpperLeft + UpperRight) / 2f;
        public Vector3 BottomCenter => (LowerLeft + LowerRight) / 2f;
    }

    static Vector3[] _corners = new Vector3[4];
    public static Vector3 GetWorldCenter(this RectTransform rect) {
        rect.GetWorldCorners(_corners);
        return (_corners[1] + _corners[3]) / 2.0f;
    }

}

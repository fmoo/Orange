using UnityEngine;

public static class CameraExtensions {
    public static Bounds OrthographicBounds(this Camera camera) {
        var v1 = camera.ViewportToWorldPoint(Vector3.zero);
        var v2 = camera.ViewportToWorldPoint(Vector3.one);
        return new Bounds((v1 + v2) / 2f, (v2 - v1).WithZ(100f));
    }

    public static Bounds GetBoundsRaycasted(this Camera camera) {
        return camera.OrthographicBounds();
    }
}
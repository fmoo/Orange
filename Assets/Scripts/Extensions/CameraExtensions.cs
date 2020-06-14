using UnityEngine;

public static class CameraExtensions {
    public static Bounds OrthographicBounds(this Camera camera) {
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float cameraHeight = camera.orthographicSize * 2;
        Bounds bounds = new Bounds(
            camera.transform.position,
            new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
        return bounds;
    }

    public static Bounds GetBoundsRaycasted(this Camera camera) {
        var bottomLeft = camera.ScreenToWorldPoint(Vector3.zero);
        var topRight = camera.ScreenToWorldPoint(new Vector3(camera.pixelWidth, camera.pixelHeight));
        return new Bounds(topRight + bottomLeft / 2, topRight - bottomLeft);
    }
}
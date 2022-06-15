using UnityEngine;
using System.Collections;
using NaughtyAttributes;
using UnityEngine.U2D;

public class OrangeCameraFollow : MonoBehaviour {
    public GameObject target;
    public PixelPerfectCamera ppCamera;
    public Camera affectCamera;
    public Camera[] affectCameras;

    [SerializeField] private bool enableClamping;
    [ShowIf("enableClamping")]
    [SerializeField] private Bounds clampBounds;

    public System.Action onCameraMoved;

    public bool adjustOnUpdate = true;
    public bool adjustOnLateUpdate = true;
    public bool adjustOnFixedUpdate = false;

    void Update() {
        if (adjustOnUpdate) DoUpdate();
    }
    void LateUpdate() {
        if (adjustOnLateUpdate) DoUpdate();
    }
    void FixedUpdate() {
        if (adjustOnFixedUpdate) DoUpdate();
    }


    void DoUpdate() {
        Vector3 origPosition = affectCamera.transform.position;
        if (enableDeadZone) {
            DoUpdateDeadZone();
        } else {
            DoUpdateNoDeadZone();
        }
        if (affectCamera.transform.position != origPosition) {
            onCameraMoved?.Invoke();
        }
        foreach (var camera in affectCameras) {
            camera.transform.position = affectCamera.transform.position;
        }
    }

    public bool enableDeadZone = false;
    public Vector2 deadZoneRatio = Vector2.one * 0.7f;
    public float cameraSpeed = 2f;
    private bool doneMoving = true;

    public bool enableEasing = false;

    const float DONE_MOVING_THRESHOLD = 0.001f;
    void DoUpdateDeadZone() {
        if (target == null) return;

        // Calculate "dead zone" using the current camera position
        var deadBounds = GetOffsetCameraBounds();
        deadBounds.size = new Vector3(deadBounds.size.x * deadZoneRatio.x, deadBounds.size.y * deadZoneRatio.y, deadBounds.size.z + 100f);

        Vector3 nearestDead;
        if (deadBounds.Contains(target.transform.position)) {
            doneMoving = true;
            return;
        } else {
            // If target is outside the dead bounds, then we want to kind of move things relative to the point
            // on the edge of the dead band, so we don't suddenly stop.
            nearestDead = deadBounds.ClosestPoint(target.transform.position);
        }

        Vector3 newPosition;
        if (enableEasing) {
            var deadMidToTarget = Vector3.Lerp(nearestDead, target.transform.position, cameraSpeed * Time.deltaTime);
            newPosition = transform.position + (deadMidToTarget - nearestDead);
        } else {
            var deadMidToTarget = nearestDead - target.transform.position;
            newPosition = transform.position - deadMidToTarget;
        }

        newPosition = GetClampedPosition(newPosition, false);

        Transform cameraTransform = GetCamera().transform;
        doneMoving = (newPosition - cameraTransform.position).magnitude < DONE_MOVING_THRESHOLD;
        cameraTransform.position = newPosition.WithZ(cameraTransform.position.z);
    }

    Camera GetCamera() {
        return (affectCamera ?? Camera.current);
    }

    void SetCameraPosition(Vector3 position) {
        GetCamera().transform.position = position;
        foreach (var camera in affectCameras) {
            camera.transform.position = position;
        }
    }

    [NaughtyAttributes.Button("Snap Now")]
    public void DoUpdateNoDeadZone() {
        if (target == null) return;
        Camera c = GetCamera();
        Vector3 v = target.transform.position;
        c.transform.position = new Vector3(
            v.x,
            v.y,
            c.transform.position.z
        );
        c.transform.position = GetClampedPosition(c.transform.position) - GetCameraHudOffset();
        doneMoving = true;
    }

    public IEnumerator WaitForMovementDone() {
        doneMoving = false;
        var oldScrollRatio = deadZoneRatio;
        deadZoneRatio = Vector2.zero;
        while (!doneMoving) {
            yield return new WaitForSeconds(0.1f);
        }
        deadZoneRatio = oldScrollRatio;
    }

    public void ClampTo(Bounds bounds) {
        enableClamping = true;
        clampBounds = bounds;
    }

    public void ClampTo(SpriteRenderer sprite) {
        if (sprite != null) {
            enableClamping = true;
            clampBounds = sprite.bounds;
        }
    }

    public void ClampTo(Grid grid) {
        ClampTo(grid.GetTilemapsBounds());
    }

    public void DisableClamping() {
        enableClamping = false;
    }

    /// <summary>Slop added to the clamp bounds for checking whether an axis should be locked.
    /// Useful if your display resolution is not an even multiple of your tile size.</summary>
    public Vector2 fixedPanSlop = Vector2.zero;
    public RectOffsetFloat hudOffset = RectOffsetFloat.zero;

    Vector3 GetCameraHudOffset() {
        return new Vector3((hudOffset.left - hudOffset.right) / 2f, (hudOffset.bottom - hudOffset.top) / 2f, 0f);
    }

    Bounds GetOffsetCameraBounds() {
        var bounds = GetCamera().GetBoundsRaycasted();

        bounds.SetMinMax(
            bounds.min + new Vector3(hudOffset.left, hudOffset.bottom, 0f),
            bounds.max - new Vector3(hudOffset.right, hudOffset.top, 0f)
        );

        return bounds;
    }

    Bounds GetCameraBounds() {
        return GetCamera().GetBoundsRaycasted();
    }

    Vector3 GetClampedPosition(Vector3 wantPosition, bool offset = true) {
        // If bounds clamping is disabled, the position we want is the one we get.
        if (!enableClamping) return wantPosition;

        // First we need the bounds that the camera can see.
        Bounds cameraBounds = offset ? GetOffsetCameraBounds() : GetCameraBounds();

        // Now we need to determine the range that the camera can move *within the clamped region*.
        // To do this, we:
        // - Subtract the camera width/height from the clamp region width/height
        // - If the result for an axis (minus optional slop for reasons) is negative,
        //   we lock the camera to the center of the clamped region for that axis.
        //
        // NOTE: This is kinda expensive and could maybe be calculated on ClampTo() instead of
        // re-running every frame.
        Bounds cameraMoveBounds = new Bounds(
            clampBounds.center,
            new Vector3(
                clampBounds.size.x - fixedPanSlop.x <= cameraBounds.size.x ? 0f : clampBounds.size.x - cameraBounds.size.x,
                clampBounds.size.y - fixedPanSlop.y <= cameraBounds.size.y ? 0f : clampBounds.size.y - cameraBounds.size.y,
                cameraBounds.size.z
            )
        );

        // Now, we just clamp the x/y axis of the input Vector to the constrained camera move bounds.
        return new Vector3(
          Mathf.Clamp(wantPosition.x, cameraMoveBounds.min.x, cameraMoveBounds.max.x),
          Mathf.Clamp(wantPosition.y, cameraMoveBounds.min.y, cameraMoveBounds.max.y),
          transform.position.z
        );
    }

    void Start() {
        DoUpdateNoDeadZone();
    }

    [System.Serializable]
    public struct RectOffsetFloat {
        public float top;
        public float bottom;
        public float left;
        public float right;

        public static readonly RectOffsetFloat zero = new RectOffsetFloat() { top = 0f, bottom = 0f, left = 0f, right = 0f };
    }
}

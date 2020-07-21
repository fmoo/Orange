using UnityEngine;
using System.Collections;
using NaughtyAttributes;

public class OrangeCameraFollow : MonoBehaviour {
    public GameObject target;
    public Camera affectCamera;

    [SerializeField] private bool enableClamping;
    [ShowIf("enableClamping")]
    [SerializeField] private Bounds clampBounds;

    public FollowMode followMode = FollowMode.LAG;

    public enum FollowMode {
        LAG,
        SNAP,
        SNAP_X_ONLY,
    }

    void LateUpdate() {
        if (followMode == FollowMode.LAG) {
            DoUpdateLag();
        } else if (followMode == FollowMode.SNAP) {
            DoUpdateNaive();
        } else if (followMode == FollowMode.SNAP_X_ONLY) {
            DoUpdateNaiveX();
        }
    }

    public Vector2 deadZoneRatio = Vector2.one * 0.7f;
    public float cameraSpeed = 2f;
    private bool doneMoving = true;

    const float DONE_MOVING_THRESHOLD = 0.001f;
    void DoUpdateLag() {
        Camera c = (affectCamera ?? Camera.current);
        Transform cameraTransform = c.transform;

        if (target == null) return;

        Vector3 targetPos = target.transform.position;
        targetPos = GetClampedPosition(targetPos);

        // Calculate "dead zone" using the current camera position
        var deadBounds = c.GetBoundsRaycasted();
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

        // Calculate the point we need to move to.
        var deadMidToTarget = Vector3.Lerp(nearestDead, target.transform.position, cameraSpeed * Time.deltaTime);
        var newPosition = transform.position + (deadMidToTarget - nearestDead);

        newPosition = GetClampedPosition(newPosition);
        doneMoving = (newPosition - cameraTransform.position).magnitude < DONE_MOVING_THRESHOLD;
        cameraTransform.position = newPosition.WithZ(cameraTransform.position.z);
    }

    public void DoUpdateNaive() {
        if (target == null) return;
        Camera c = (affectCamera ?? Camera.current);
        Vector3 v = target.transform.position;
        c.transform.position = new Vector3(
            v.x,
            v.y,
            c.transform.position.z
        );
        c.transform.position = GetClampedPosition(c.transform.position);
    }
    public void DoUpdateNaiveX() {
        if (target == null) return;
        Camera c = (affectCamera ?? Camera.current);
        Vector3 v = target.transform.position;
        c.transform.position = new Vector3(
            v.x,
            c.transform.position.y,
            c.transform.position.z
        );
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

    public void DisableClamping() {
        enableClamping = false;
    }

    /// <summary>Slop added to the clamp bounds for checking whether an axis should be locked.
    /// Useful if your display resolution is not an even multiple of your tile size.</summary>
    public Vector2 fixedPanSlop = Vector2.zero;

    Vector3 GetClampedPosition(Vector3 wantPosition) {
        if (!enableClamping) return wantPosition;

        Bounds cameraBounds = affectCamera.GetBoundsRaycasted();

        Bounds cameraMoveBounds = new Bounds( //Define the camera's total safe zone for a given map boundary, as defined by mapBounds.
            clampBounds.center,
            new Vector3(
                clampBounds.size.x - fixedPanSlop.x <= cameraBounds.size.x ? 0f : clampBounds.size.x - cameraBounds.size.x,
                clampBounds.size.y - fixedPanSlop.y <= cameraBounds.size.y ? 0f : clampBounds.size.y - cameraBounds.size.y,
                cameraBounds.size.z
            )
        );

        return new Vector3(
          Mathf.Clamp(wantPosition.x, cameraMoveBounds.min.x, cameraMoveBounds.max.x),
          Mathf.Clamp(wantPosition.y, cameraMoveBounds.min.y, cameraMoveBounds.max.y),
          transform.position.z
        );
    }

    void Start() {
        DoUpdateNaive();
    }
}

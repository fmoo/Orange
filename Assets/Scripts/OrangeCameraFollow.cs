using UnityEngine;
using System.Collections;

public class OrangeCameraFollow : MonoBehaviour {
    public GameObject target;
    public Camera affectCamera;

    /// <summary>If set, don't let the camera travel outside these bounds</summary>
    public Bounds? cameraBounds = null;

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

    public float noScrollRatio = 0.7f;
    public float cameraSpeed = 2f;
    private bool doneMoving = true;
    void DoUpdateLag() {
        // This approach kind of sucks. Ideally we'd have some sort of
        // bounding rectangle and if the player moves outside of it, we move
        // to follow.
        Camera c = (affectCamera ?? Camera.current);
        Transform cameraTransform = c.transform;
        Vector3 v = target.transform.position;

        var innerBounds = c.OrthographicBounds();
        Vector3 cp;
        innerBounds.size = (innerBounds.size * noScrollRatio) + (Vector3.forward * 100f);
        if (innerBounds.Contains(v)) {
            doneMoving = true;
            return;
        } else {
            cp = innerBounds.ClosestPoint(v);
        }

        var cp2 = Vector3.Lerp(
            cp,
            v,
            cameraSpeed * Time.deltaTime);

        var newPosition = cameraTransform.position + (cp2 - cp);
        newPosition.z = cameraTransform.position.z;

        if (cameraBounds is Bounds cb && cb.size.x > innerBounds.extents.x && cb.size.y > innerBounds.extents.y) {
            if (!cb.Contains(newPosition)) {
                newPosition = cb.ClosestPoint(newPosition);
            }
        }

        doneMoving = (newPosition - cameraTransform.position).magnitude < 0.001f;
        cameraTransform.position = newPosition;
        // t.position = new Vector3(v.x, v.y, t.position.z);
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
        if (cameraBounds is Bounds nnCameraBounds) {
            c.transform.position = GetClampedPosition(c.transform.position, nnCameraBounds);
        }
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
        var oldScrollRatio = noScrollRatio;
        noScrollRatio = 0f;
        while (!doneMoving) {
            yield return new WaitForSeconds(0.1f);
        }
        noScrollRatio = oldScrollRatio;
    }

    Vector3 GetClampedPosition(Vector3 wantPosition, Bounds mapBounds) {
        Bounds cameraBounds = affectCamera.OrthographicBounds();

        Bounds cameraMoveBounds = new Bounds( //Define the camera's total safe zone for a given map boundary, as defined by mapBounds.
            mapBounds.center,
            new Vector3(
                mapBounds.size.x <= cameraBounds.size.x ? 0f : mapBounds.size.x - cameraBounds.size.x,
                mapBounds.size.y <= cameraBounds.size.y ? 0f : mapBounds.size.y - cameraBounds.size.y,
                cameraBounds.size.z
            )
        );

        return new Vector3(
          Mathf.Clamp(target.transform.position.x, cameraMoveBounds.min.x, cameraMoveBounds.max.x),
          Mathf.Clamp(target.transform.position.y, cameraMoveBounds.min.y, cameraMoveBounds.max.y),
          transform.position.z
        );
    }

}

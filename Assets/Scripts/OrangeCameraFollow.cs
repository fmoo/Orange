using UnityEngine;
using System.Collections;

public class OrangeCameraFollow : MonoBehaviour {
    public Collider2D targetCollider;
    public SpriteRenderer targetSprite;
    public Camera affectCamera;

    /// <summary>If set, don't let the camera travel outside these bounds</summary>
    public Bounds? cameraBounds = null;

    Bounds GetTargetBounds() {
        if (targetSprite != null) {
            return targetSprite.bounds;
        } else if (targetCollider != null) {
            return targetSprite.bounds;
        } else {
            return Camera.current.OrthographicBounds();
        }
    }

    void LateUpdate() {
        DoUpdateNaive();
    }


    public float noScrollRatio = 0.7f;
    public float cameraSpeed = 2f;
    void DoUpdateNaive() {
        // This approach kind of sucks. Ideally we'd have some sort of
        // bounding rectangle and if the player moves outside of it, we move
        // to follow.
        Camera c = (affectCamera ?? Camera.current);
        Transform cameraTransform = c.transform;
        Vector3 v = GetTargetBounds().center;

        var innerBounds = c.OrthographicBounds();
        Vector3 cp;
        innerBounds.size = (innerBounds.size * noScrollRatio) + (Vector3.forward * 100f);
        if (innerBounds.Contains(v)) {
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

        if (cameraBounds is Bounds cb) {
            if (cb.Contains(newPosition) != true) {
                newPosition = cb.ClosestPoint(newPosition);
            }
        }

        cameraTransform.position = newPosition;
        // t.position = new Vector3(v.x, v.y, t.position.z);
    }

    public IEnumerator WaitForMovementDone() {
        var oldScrollRatio = noScrollRatio;
        noScrollRatio = 0f;
        while (true) {
            var dist = (transform.position.xy() - GetTargetBounds().center.xy()).sqrMagnitude;
            if (dist < 0.01f) break;
            yield return new WaitForSeconds(0.1f);
        }
        noScrollRatio = oldScrollRatio;
    }
}

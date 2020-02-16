using UnityEngine;
using System.Collections;

public class OrangeCameraFollow : MonoBehaviour {
    public Collider2D targetCollider;
    public SpriteRenderer targetSprite;
    public Camera affectCamera;

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


    public float cameraSpeed = 2f;
    void DoUpdateNaive() {
        // This approach kind of sucks. Ideally we'd have some sort of
        // bounding rectangle and if the player moves outside of it, we move
        // to follow.
        Camera c = (affectCamera ?? Camera.current);
        Transform t = c.transform;
        Vector3 v = GetTargetBounds().center;
        // var cameraBounds = c.OrthographicBounds();
        // cameraBounds.size = (cameraBounds.size * 0.7f) + (Vector3.forward * 100f);
        // if (cameraBounds.Contains(v)) {
        // 	return;
        // }

        var newPosition = Vector3.Lerp(
            t.position,
            new Vector3(v.x, v.y, t.position.z),
            cameraSpeed * Time.deltaTime);

        if (cameraBounds is Bounds cb) {
            if (cb.Contains(newPosition) != true) {
                newPosition = cb.ClosestPoint(newPosition);
            }
        }

        t.position = newPosition;
        // t.position = new Vector3(v.x, v.y, t.position.z);
    }

    public IEnumerator WaitForMovementDone() {
        while (true) {
            var dist = (transform.position.xy() - GetTargetBounds().center.xy()).sqrMagnitude;
            if (dist < 0.01f) break;
            yield return new WaitForSeconds(0.1f);
        }
    }
}

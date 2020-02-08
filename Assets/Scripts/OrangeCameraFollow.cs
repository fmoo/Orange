using UnityEngine;
using System.Collections;

public class OrangeCameraFollow : MonoBehaviour {
	public Collider2D targetCollider;
	public SpriteRenderer targetSprite;

	Bounds GetBounds() {
		if (targetSprite != null) {
			return targetSprite.bounds;
		} else if (targetCollider != null) {
			return targetSprite.bounds;
		} else {
			return Camera.current.OrthographicBounds();
		}
	}

	void LateUpdate () {
		DoUpdateNaive();
	}

	void DoUpdateNaive() {
		// This approach kind of sucks. Ideally we'd have some sort of
		// bounding rectangle and if the player moves outside of it, we move
		// to follow.
		Transform t = GetComponent<Transform> ();
		Vector3 v = GetBounds().center;
		t.position = new Vector3(v.x, v.y, t.position.z);
	}
}

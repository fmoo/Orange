using UnityEngine;
using System.Collections;

public class OrangeCameraFollow : MonoBehaviour {
	public Collider2D actor;

	void Update () {
		// This approach kind of sucks. Ideally we'd have some sort of
		// bounding rectangle and if the player moves outside of it, we move
		// to follow.

		Transform t = GetComponent<Transform> ();
		Vector3 v = actor.bounds.center;
		t.position = new Vector3(v.x, v.y, t.position.z);
	}
}

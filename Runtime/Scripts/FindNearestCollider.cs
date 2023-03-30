using UnityEngine;
using System;
using System.Collections.Generic;

public class FindNearestCollider : MonoBehaviour {
	private HashSet<GameObject> _Objects = new HashSet<GameObject>();

	public GameObject Nearest;
	public float NearestDistance = Single.PositiveInfinity;

	// Use this for initialization
	void Start () {
		_AssertHasTriggerCollider ();
	}

	private void _AssertHasTriggerCollider() {
		foreach (Collider2D collider in GetComponents<Collider2D>()) {
			if (collider.isTrigger) {
				return;
			}
		}
		throw new Exception("FindNearestCollider requires at least one Collider2D with isTrigger set");
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		_Objects.Add (other.gameObject);

	}

	void OnTriggerStay2D(Collider2D other)
	{
	}

	void OnTriggerExit2D(Collider2D other)
	{
		_Objects.Remove (other.gameObject);
	}

	// Update is called once per frame
	void Update () {
		// TODO: This should probably be done in FixedUpdate instead for accuracy,
		// but here is fine for now.
		if (_Objects.Count == 0) {
			Nearest = null;
			NearestDistance = Single.PositiveInfinity;
		} else {
			// TODO: Optimize: If none of the _Objects have moved, then do nothing.
			Vector3 p0_3d = GetComponent<Collider2D>().bounds.center;
			Vector2 p0 = new Vector2(p0_3d.x, p0_3d.y);

			GameObject local_nearest = null;
			float nearest_distance = Single.PositiveInfinity;

			foreach (GameObject o in _Objects) {
				Vector3 p1_3d = o.GetComponent<Collider2D>().bounds.center;
				float distance = Vector2.Distance(
					p0,
					new Vector2(p1_3d.x, p1_3d.y)
				);

				if (distance < nearest_distance) {
					local_nearest = o;
					nearest_distance = distance;
				}
			}

			Nearest = local_nearest;
			NearestDistance = nearest_distance;
		}
	}
}

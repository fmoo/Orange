using UnityEngine;
using System.Collections;

/**
 * Sets a GameObject's spriterenderer sort order to something that kinda makes sense.
 * 
 * This is a dirty hack, but it works given general world coordinates.  Should generally work
 * as wraparound should be fairly uncommon.
 */
public class FixRenderOrder : MonoBehaviour {

	void Update () {
		Transform t = GetComponent<Transform> ();
		Collider2D c = GetComponent<Collider2D> ();
		float yvalue = (c != null ? c.bounds.center.y : t.position.y);
		var renderer = GetComponent<SpriteRenderer> ();
		renderer.sortingOrder = -(int)(yvalue * 100.0);
	}
}

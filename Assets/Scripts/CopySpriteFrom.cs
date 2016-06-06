using UnityEngine;
using System.Collections;

/**
 * Automatically sync another object's sprite to this one.
 *
 * Useful for things like shadows.
 */
public class CopySpriteFrom : MonoBehaviour {
	public Component SourceObject;

	private SpriteRenderer _SourceRenderer;
	private Transform _Transform;

  /**
   * Just grab a reference to the target component's Renderer for
   * quicker access during Update.
   */
	void Start () {
		if (SourceObject == null)
			return;
		_SourceRenderer = SourceObject.GetComponent<SpriteRenderer> ();
	}

	void Update () {
    // Don't crap out if we couldn't find a source object.
		if (_SourceRenderer == null)
			return;

    // Easy. Just copy the sprite reference.
		GetComponent<SpriteRenderer> ().sprite = _SourceRenderer.sprite;
	}
}

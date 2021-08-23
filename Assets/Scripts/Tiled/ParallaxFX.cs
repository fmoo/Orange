using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParallaxFX : MonoBehaviour {
	public Vector2 scrollSpeed;
	public RawImage image;
	public OrangeSpriteAnimator animator;
	public string animationName;
	public Vector2 cameraScroll;

	Vector2 origin;
	Vector2 autoScrollOffset = Vector2.zero;

	void Start() {
		origin = image.uvRect.center;
	}


	void LateUpdate() {
		var rect = image.uvRect;
		rect.center = origin;
		if (scrollSpeed != Vector2.zero) {
			autoScrollOffset += Time.deltaTime * scrollSpeed;
		}
		rect.center += autoScrollOffset;
		if (cameraScroll != Vector2.zero && Camera.main != null) {
			rect.center += cameraScroll * Camera.main.transform.position;
		}
		image.uvRect = rect;
	}

	void OnDisable() {
		image.enabled = false;
		if (animator != null) {
			animator.SetAnimation("");
			animator.enabled = false;
		}
	}

	void OnEnable() {
		image.enabled = true;
		if (animator != null) {
			animator.SetAnimation(animationName);
			animator.enabled = true;
		}
	}
}

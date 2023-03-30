using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ParallaxFX : MonoBehaviour {
	public Vector2 scrollSpeed;
	public Canvas canvas;
	public RawImage image;
	public OrangeSpriteAnimator animator;
	public string animationName;
	public Vector2 cameraScroll;
	public Vector2 baseOffset;

	Vector2 origin;
	Vector2 autoScrollOffset = Vector2.zero;

	public float pixelsPerUnit = 1;
	void Start() {
		origin = image.uvRect.center;
		canvas.renderMode = RenderMode.ScreenSpaceCamera;

		// Retrieve the layer mask for "BackgroundUI"
		int layerMask = LayerMask.NameToLayer("BackgroundUI");
		// If the layerMask exists, find the camera that is rendering the layerMask
		if (layerMask != -1) {
			// Find the camera that includes this layerMask
			canvas.worldCamera = Camera.allCameras.FirstOrDefault(c => (c.cullingMask & (1 << layerMask)) != 0) ?? Camera.main;
		}
	}

	Rect CalculateUVRect() {
		var rect = image.uvRect;
		rect.center = origin;
		if (scrollSpeed != Vector2.zero) {
			autoScrollOffset += Time.deltaTime * scrollSpeed;
		}
		var adjustedBase = baseOffset;

		rect.center += adjustedBase + autoScrollOffset;
		if (cameraScroll != Vector2.zero && Camera.main != null) {
			var cam = Camera.main;
			rect.center += cameraScroll * cam.transform.position / new Vector2(image.texture.width, image.texture.height) * (float)pixelsPerUnit;
		}
		return rect;
	}

	void LateUpdate() {
		image.uvRect = CalculateUVRect();
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

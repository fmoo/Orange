using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SuperTiled2Unity;

public class ParallaxLayerHandler : MonoBehaviour {
	public SuperLayer layer;

	private Vector3 v3 = new Vector3();

	public Vector3 origOffset;
	public bool reset;

	void Start() {
		origOffset = transform.localPosition;
		var props = layer.GetComponent<SuperCustomProperties>();
		if (layer is SuperImageLayer) {
			var canvas = layer.GetComponent<Canvas>();
			canvas.worldCamera = Camera.main;
		}
	}

	void LateUpdate() {
		if (reset) {
			layer.transform.localPosition = origOffset;
			return;
		}

		if (layer == null) return;
		var camera = Camera.main ?? Camera.current;
		if (camera == null) return;

		v3.x = origOffset.x + (camera.transform.position.x * (1f - layer.m_ParallaxX)) + (camera.rect.x / 2f * layer.m_ParallaxX * layer.m_OffsetX / 16f);
		v3.y = origOffset.y + (camera.transform.position.y * (1f - layer.m_ParallaxY)) + (camera.rect.y / 2f * layer.m_ParallaxY * layer.m_OffsetY / 16f);
		v3.z = layer.transform.localPosition.z;
		layer.transform.localPosition = v3;
	}

}

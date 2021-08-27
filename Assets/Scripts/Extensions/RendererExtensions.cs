using UnityEngine;
using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;


public static class RendererExtensions {
	static MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();

	public static void SetColor(this SpriteRenderer renderer, Color value) {
		renderer.color = value;
	}

	public static void ApplyPropertyBlock(this Renderer renderer, Action<MaterialPropertyBlock> applyProperties) {
		propertyBlock.Clear();
		renderer.GetPropertyBlock(propertyBlock);
		applyProperties(propertyBlock);
		renderer.SetPropertyBlock(propertyBlock);
	}

	public static void SetPropertyBlockColor(this Renderer renderer, string name, Color value) {
		renderer.ApplyPropertyBlock(prop => prop.SetColor(name, value));
	}

	public static Color GetPropertyBlockColor(this Renderer renderer, string name) {
		renderer.GetPropertyBlock(propertyBlock);
		return propertyBlock.GetColor(name);
	}

	public static void SetPropertyBlockFloat(this Renderer renderer, string name, float value) {
		renderer.ApplyPropertyBlock(prop => prop.SetFloat(name, value));
	}

	public static float GetPropertyBlockFloat(this Renderer renderer, string name) {
		renderer.GetPropertyBlock(propertyBlock);
		return propertyBlock.GetFloat(name);
	}

	public static TweenerCore<Color, Color, DG.Tweening.Plugins.Options.ColorOptions> DOPropertyBlockColor(
		this SpriteRenderer renderer,
		string colorProp,
		Color endValue,
		float duration
	) {
		var t = DOTween.To(
			() => renderer.GetPropertyBlockColor(colorProp),
			x => renderer.SetPropertyBlockColor(colorProp, x),
			endValue,
			duration);
		t.SetTarget(renderer);
		return t;
	}

	public static TweenerCore<Color, Color, ColorOptions> DOFade(this UnityEngine.Tilemaps.TilemapRenderer tilemapRenderer, float endValue, float duration) {
		return tilemapRenderer.GetComponent<UnityEngine.Tilemaps.Tilemap>().DOFade(endValue, duration);
	}

	public static TweenerCore<Color, Color, ColorOptions> DOColor(this UnityEngine.Tilemaps.Tilemap target, Color endValue, float duration) {
		TweenerCore<Color, Color, ColorOptions> t = DOTween.To(() => target.color, x => target.color = x, endValue, duration);
		t.SetTarget(target);
		return t;
	}

	public static TweenerCore<Color, Color, DG.Tweening.Plugins.Options.ColorOptions> DOFade(this UnityEngine.Tilemaps.Tilemap target, float endValue, float duration) {
		var t = DOTween.ToAlpha(() => target.color, x => target.color = x, endValue, duration);
		t.SetTarget(target);
		return t;
	}
}
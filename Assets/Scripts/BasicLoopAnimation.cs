using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Behavior that updates the SpriteRenderer that belongs to this GameObject
 * 
 * Using the specified list of `sprites` and `loopDuration`
 */
public class BasicLoopAnimation : GameBehavior {
	public List<Sprite> sprites;
	public float loopDuration = 1.0f;

	private float timeOffset = 0.0f;
	private SpriteRenderer _renderer;

	// Use this for initialization
	void Start () {
		_renderer = GetOrCreateComponent<SpriteRenderer> ();
		if (sprites == null) {
			sprites = new List<Sprite> ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (_renderer == null || sprites.Count == 0 || loopDuration <= 0) {
			return;
		}

		timeOffset += Time.deltaTime;
		while (timeOffset > loopDuration) {
			timeOffset -= loopDuration;
		}

		int spriteOffset = (int)((timeOffset / loopDuration) * sprites.Count);
		_renderer.sprite = sprites[spriteOffset];
	}
}
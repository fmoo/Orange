using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MotionController : MonoBehaviour {
	public List<Sprite> walkUp, walkDown, walkSide;
	public List<Sprite> runUp, runDown, runSide;
	public Sprite idleUp, idleDown, idleSide;

	public float loopDuration = 1.0f;

	public bool invertWalkSide = false;
	public bool invertRunSide = false;

	public float walkThreshold = 0.01f;
	public float runThreshold = 0.5f;
	public float runLoopFactor = 0.5f;

	private float timeOffset = 0.0f;
	private bool flipped = false;

	// Use this for initialization
	void Start () {
		if (runUp.Count == 0) {
			runUp = walkUp;
		}
		if (runSide.Count == 0) {
			runSide = walkSide;
		}
		if (runDown.Count == 0) {
			runDown = walkDown;
		}
		if (idleUp == null && walkUp.Count > 0 && walkUp[0] != null) {
			idleUp = walkUp [0];
		}
		if (idleSide == null && walkSide.Count > 0 && walkSide[0] != null) {
			idleSide = walkSide [0];
		}
		if (idleDown == null && walkDown.Count > 0 && walkDown[0] != null) {
			idleDown = walkDown [0];
		}
	}
	
	// Update is called once per frame
	void Update () {
		Rigidbody2D body = GetComponent<Rigidbody2D> ();

		float speed = body.velocity.magnitude;
		bool isMoving = speed > walkThreshold;
		bool isRunning = speed > runThreshold;

		if (GetComponent<Renderer>() == null || loopDuration <= 0) {
			return;
		}

		if (!isMoving) {
			this.UpdateIdle(body);
		} else {
			this.UpdateMoving(body, speed, isRunning);
		}
	}

	void UpdateIdle(Rigidbody2D body) {
		SpriteRenderer renderer = GetComponent<SpriteRenderer> ();
		Heading heading = GetComponent<Heading> ();

		// Fall back to physics velocity if the no heading is unavailable
		Vector2 hvelocity = (heading != null ? heading.velocity : body.velocity);
		float angle = Vector2.Angle (hvelocity, Vector2.up);
		bool want_flip = false;
		Sprite to_render;

		if (angle < 45) {
			// UP
			to_render = idleUp;
		} else if (angle < 135) {
			// LEFT/RIGHT
			if (hvelocity.x < 0) {
				// LEFT
				want_flip = !invertRunSide;
			} else {
				// RIGHT
				want_flip = invertRunSide;
			}
			to_render = idleSide;
		} else {
			// DOWN
			to_render = idleDown;
		}

		if (want_flip != flipped) {
			Transform t = GetComponent<Transform>();
			Vector3 v = t.localScale;
			v.x = -v.x;
			t.localScale = v;
			flipped = want_flip;
		}

		if (to_render == null) {
			return;
		}

		renderer.sprite = to_render;
	}

	void UpdateMoving(Rigidbody2D body, float speed, bool isRunning) {
		SpriteRenderer renderer = GetComponent<SpriteRenderer> ();
		Heading heading = GetComponent<Heading> ();
		List<Sprite> sprites = null;

		// Fall back to physics velocity if the no heading is unavailable
		Vector2 hvelocity = (heading != null ? heading.velocity : body.velocity);
		float angle = Vector2.Angle (hvelocity, Vector2.up);
		bool want_flip = false;

		float multiplier = (isRunning ? runLoopFactor : 1.0f);

		if (angle < 45) {
			// UP
			if (isRunning) {
				sprites = runUp;
			} else {
				sprites = walkUp;
			}
		} else if (angle < 135) {
			// LEFT/RIGHT
			if (isRunning) {
				sprites = runSide;
			} else {
				sprites = walkSide;
			}
			if (hvelocity.x < 0) {
				// LEFT
				want_flip = !invertRunSide;
			} else {
				// RIGHT
				want_flip = invertRunSide;
			}
		} else {
			// DOWN
			if (isRunning) {
				sprites = runDown;
			} else {
				sprites = walkDown;
			}
		}
		
		if (want_flip != flipped) {
			Transform t = GetComponent<Transform>();
			Vector3 v = t.localScale;
			v.x = -v.x;
			t.localScale = v;
			flipped = want_flip;
		}
		
		if (sprites == null || sprites.Count == 0) {
			return;
		}
		
		timeOffset += Time.deltaTime * (speed * multiplier) ;
		while (timeOffset > loopDuration) {
			timeOffset -= loopDuration;
		}
		
		int spriteOffset = (int)((timeOffset / loopDuration) * sprites.Count);
		renderer.sprite = sprites[spriteOffset];
	}
}

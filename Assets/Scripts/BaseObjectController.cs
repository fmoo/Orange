using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
abstract public class BaseObjectController : GameBehavior {

	public enum ShadowMode {
		NONE = 0,
		SPRITE_SKEW_SHADER = 1,
		UNITY_PERPENDICULAR = 2,
	}
	/**
	 * Apply this scaling factors to ALL characters in the scene
	 */
	protected static float GLOBAL_SCALE_FACTOR = 0.785f;
	protected static string CHARACTER_SORT_LAYER = "Sprites";
	protected static string SHADOW_SORT_LAYER = "Shadows";	

	public float scaleFactor = 1.0f;
	public ShadowMode shadowMode = ShadowMode.NONE;
		
	private GameObject shadow;

	virtual protected Sprite GetDefaultSprite() { return null; }
	virtual protected bool HasCircleCollider() { return false; }
	virtual protected void InitCircleCollider(CircleCollider2D cc) { }

	/**
	 * Different sprites require different offsets.  0.35 seems to be a sweet spot?
	 * Override this on your character 
	 */
	virtual protected float GetShadowOffset() {
		return 0.35f;
	}
    

	virtual protected void _InitForEditor()  {
		// Adjust the scale according to these settings
		this.transform.localScale = new Vector3(
			this.scaleFactor * GLOBAL_SCALE_FACTOR,
			this.scaleFactor * GLOBAL_SCALE_FACTOR,
			1.0f
		);

		// Add all required components
		var sr = GetOrCreateComponent<SpriteRenderer> ();
		sr.sortingLayerName = CHARACTER_SORT_LAYER;

		// Set the default_sprite.
		var default_sprite = GetDefaultSprite ();
		if (default_sprite != null) {
			sr.sprite = default_sprite;
		}

		// Add the shadow subobject
		if (this.shadowMode == ShadowMode.SPRITE_SKEW_SHADER) {
			GameObject shadow = GetOrCreateGameObject ("shadow");
			var copyfrom = shadow.GetOrCreateComponent<CopySpriteFrom>();
			copyfrom.SourceObject = this.gameObject.transform;
			var r = shadow.GetOrCreateComponent<SpriteRenderer>();
			r.sprite = sr.sprite;
			r.material = Resources.Load<Material> ("Materials/ShadowMaterial");
			r.sortingLayerName = SHADOW_SORT_LAYER;

			MaterialPropertyBlock pb = new MaterialPropertyBlock ();
			r.GetPropertyBlock (pb);
			pb.SetFloat ("_Fudge", GetShadowOffset ());
			r.SetPropertyBlock (pb);
		} else if (this.shadowMode == ShadowMode.UNITY_PERPENDICULAR) {
			GameObject shadow = GetOrCreateGameObject ("shadow-caster");
			shadow.transform.localRotation = Quaternion.AngleAxis(-90, Vector3.right);

			var copyfrom = shadow.GetOrCreateComponent<CopySpriteFrom>();
			copyfrom.SourceObject = gameObject.transform;

			var r = shadow.GetOrCreateComponent<SpriteRenderer>();
			r.sprite = sr.sprite;
			r.material = Resources.Load<Material> ("Materials/BumpDiffuseWithShadow");
			r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
			// r.sortingLayerName = SHADOW_SORT_LAYER;

		
		}
	}

	virtual protected void _InitOtherComponents() {
		var rb = GetOrCreateComponent<Rigidbody2D> ();
		rb.freezeRotation = true;

		GetOrCreateComponent<Heading> ();
		GetOrCreateComponent<FixRenderOrder> ();

		if (HasCircleCollider ()) {
			var cc = GetOrCreateComponent<CircleCollider2D> ();
			InitCircleCollider (cc);
		}
	}

	// Use this for initialization
	protected void Start () {
		_InitForEditor ();

		// Early out so we don't add a bunch of extra crap in edit mode.
		if (IsInEditorMode()) {
			return;
		}

		_InitOtherComponents ();
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrangeSpriteAnimator : MonoBehaviour {
    public bool active = true;
    public OrangeSpriteManager sprites;
    public new SpriteRenderer renderer;
    public new string name;

    private new OrangeSpriteManagerAnimation animation;
    private float timeElapsed = 0f;

    // Start is called before the first frame update
    void Start() {
        if (!active) {
            return;
        }
        animation = sprites.GetAnimation(name);
    }

    void OnValidate() {
        if (!active) {
            return;
        }
        var anim = sprites.GetAnimation(name);
        if (anim == null) {
            return;
        }
        anim.GetSpriteForIndex(0).SetRendererSprite(renderer);
    }

    // Update is called once per frame
    void Update() {
        if (!active) {
            return;
        }
        timeElapsed += Time.deltaTime;
        animation.GetSpriteForTime(timeElapsed).SetRendererSprite(renderer);
    }
}

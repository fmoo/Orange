using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrangeSpriteAnimator : MonoBehaviour {
    public new string name;
    public bool active = true;
    public OrangeSpriteManager sprites;
    public new SpriteRenderer renderer;
    public UnityEngine.UI.Image image;

    private new OrangeSpriteManagerAnimation animation;
    private float timeElapsed = 0f;

    public void SetAnimation(string name) {
        this.name = name;
        animation = sprites.GetAnimation(name);
    }

    // Start is called before the first frame update
    void Start() {
        if (!active) {
            return;
        }
        SetAnimation(name);
    }

    void OnValidate() {
        if (!active) return;
        if (sprites == null) return;

        var anim = sprites.GetAnimation(name);
        if (anim == null) return;

        if (renderer != null)
            anim.GetSpriteForIndex(0).SetRendererSprite(renderer);
        if (image != null)
            anim.GetSpriteForIndex(0).SetUIImageSprite(image);
    }

    // Update is called once per frame
    void Update() {
        if (!active) {
            return;
        }
        timeElapsed += Time.deltaTime;
        if (renderer != null)
            animation.GetSpriteForTime(timeElapsed).SetRendererSprite(renderer);
        if (image != null)
            animation.GetSpriteForTime(timeElapsed).SetUIImageSprite(image);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrangeSpriteAnimator : MonoBehaviour {
    public string animationName;
    public bool active = true;
    public OrangeSpriteManager sprites;
    public SpriteRenderer spriteRenderer;
    public UnityEngine.UI.Image image;

    private OrangeSpriteManagerAnimation animator;
    private float timeElapsed = 0f;

    public void SetAnimation(string animationName) {
        this.animationName = animationName;
        animator = sprites.GetAnimation(animationName);
    }

    void Start() {
        SetAnimation(animationName);
    }

    void OnValidate() {
        if (!active) return;
        if (sprites == null) return;

        var anim = sprites.GetAnimation(animationName);
        if (anim == null) return;

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            anim.GetSpriteForIndex(0).SetRendererSprite(spriteRenderer);

        if (image == null)
            image = GetComponent<UnityEngine.UI.Image>();
        if (image != null)
            anim.GetSpriteForIndex(0).SetUIImageSprite(image);
        SetAnimation(animationName);
    }

    // Update is called once per frame
    void Update() {
        if (!active) {
            return;
        }
        if (animator is null) {
            SetAnimation(animationName);
            if (animator is null) {
                Debug.LogWarning($"Animator is not set for {animationName} on {name}");
            }
        }
        timeElapsed += Time.deltaTime;
        if (spriteRenderer != null)
            animator.GetSpriteForTime(timeElapsed).SetRendererSprite(spriteRenderer);
        if (image != null)
            animator.GetSpriteForTime(timeElapsed).SetUIImageSprite(image);
    }
}

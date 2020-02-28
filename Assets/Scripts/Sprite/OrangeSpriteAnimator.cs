using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrangeSpriteAnimator : MonoBehaviour {
    public string animationName;
    public bool active = true;
    public OrangeSpriteManager sprites;
    public SpriteRenderer spriteRenderer;
    public UnityEngine.UI.Image image;
    public bool destroyOnDone = true;

    private OrangeSpriteManagerAnimation animator;
    private float timeElapsed = 0f;

    public void SetAnimation(string animationName) {
        timeElapsed = 0f;
        this.animationName = animationName;
        animator = sprites.GetAnimation(animationName);
        if (animator == null)
            Debug.LogError($"Animation not found for {name}'s '{animationName}' from {sprites?.name}");
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
                Debug.LogError($"Animator is not set for {animationName} on {name}");
            }
        }
        timeElapsed += Time.deltaTime;
        var sprite = animator.GetSpriteForTime(timeElapsed);

        if (sprite != null) {
            if (spriteRenderer != null) {
                sprite.SetRendererSprite(spriteRenderer);
            }
            if (image != null)
                sprite.SetUIImageSprite(image);
        }

        if (sprite == null && destroyOnDone) {
            if (animator.loop == true) {
                Debug.LogError("Null sprite returned for looping animation!");
                return;
            }
            Destroy(gameObject);
        }
    }

    public IEnumerator WaitForDone() {
        if (animator == null) Start();
        if (animator.loop) {
            Debug.LogError($"{name}'s .WaitForDone() called for looping animation '{animator.name}'!");
            yield break;
        }
        var timeToSleep = animator.duration - timeElapsed;
        if (timeToSleep <= 0f) yield break;
        yield return new WaitForSeconds(timeToSleep);
    }
}

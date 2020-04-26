using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrangeSpriteAnimator : MonoBehaviour {
    public string animationName;
    public bool active = true;
    public OrangeSpriteDB sprites;
    public SpriteRenderer spriteRenderer;
    public UnityEngine.UI.Image image;
    public bool destroyOnDone = true;
    public System.Action onAnimationDone;

    private OrangeSpriteManagerAnimation animator;
    private float timeElapsed = 0f;
    private float stallTime = 0f;

    public void AddStallTime(float addTime) {
        stallTime += addTime;
    }

    public void SetAnimation(string animationName) {
        enabled = true;
        animator = sprites.GetAnimation(animationName);
        if (animationName == this.animationName) return;
        onAnimationDone?.Invoke();
        onAnimationDone = null;
        timeElapsed = 0f;
        stallTime = 0f;
        this.animationName = animationName;
        if (animator == null)
            Debug.LogError($"Animation not found for {name}'s '{animationName}' from {sprites?.name}");
    }

    public void StopAnimation(string spriteName) {
        var sprite = sprites.GetSprite(spriteName);
        if (sprite == null) {
            Debug.LogError($"No sprite for {spriteName}!");
        } else {
            if (spriteRenderer != null) sprite.SetRendererSprite(spriteRenderer);
            if (image != null) sprite.SetUIImageSprite(image);
        }
        enabled = false;
        animationName = "";
        stallTime = 0f;
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
        float remainingTime = Time.deltaTime;
        if (stallTime > 0f) {
            if (stallTime > remainingTime) {
                stallTime -= remainingTime;
                remainingTime = 0f;
            } else {
                remainingTime -= stallTime;
                stallTime = 0f;
            }
        }
        timeElapsed += remainingTime;
        var sprite = animator.GetSpriteForTime(timeElapsed);

        if (sprite != null) {
            if (spriteRenderer != null) {
                sprite.SetRendererSprite(spriteRenderer);
            }
            if (image != null)
                sprite.SetUIImageSprite(image);
        }

        if (sprite == null) {
            if (animator.loop == false) {
                onAnimationDone?.Invoke();
                onAnimationDone = null;
            }
            if (destroyOnDone) {
                if (animator.loop == true) {
                    Debug.LogError("Null sprite returned for looping animation!");
                    return;
                }
                Destroy(gameObject);
            }
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
        // TODO: Handle stalls!!
    }
}

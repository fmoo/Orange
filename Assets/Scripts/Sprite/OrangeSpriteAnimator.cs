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
    public float playbackSpeed = 1f;
    public System.Action onAnimationDone;
    public bool resetTimeOnChange = true;

    private OrangeSpriteManagerAnimation currentAnimation;
    private float timeElapsed = 0f;
    private float stallTime = 0f;

    public void AddStallTime(float addTime) {
        stallTime += addTime;
    }

    public bool HasAnimation(string animationName) {
        return sprites != null && sprites.GetAnimation(animationName) != null;
    }

    public void SetAnimation(string animationName) {
        enabled = true;
        currentAnimation = sprites.GetAnimation(animationName);
        if (animationName == this.animationName) return;
        var lastDone = onAnimationDone;
        onAnimationDone = null;
        lastDone?.Invoke();
        if (resetTimeOnChange || !currentAnimation.loop) {
            timeElapsed = 0f;
        }
        stallTime = 0f;
        this.animationName = animationName;
        if (currentAnimation == null)
            Debug.LogError($"Animation not found for {name}'s '{animationName}' from {sprites?.name}", this);
        var sprite = currentAnimation.GetSpriteForIndex(0);
        if (sprite != null)
            SetSprite(sprite);
    }

    public void ResetAnimation(string animationName) {
        SetAnimation(animationName);
        ResetAnimation();
    }

    public void ResetAnimation() {
        timeElapsed = 0f;
        stallTime = 0f;
    }

    private bool dirty = false;

    void LateUpdate() {
        if (dirty) {
            dirty = false;
            if (image != null)
                image.SetNativeSize();
        }
    }

    public void StopAnimation(string spriteName) {
        var sprite = sprites.GetSprite(spriteName);
        if (sprite == null) {
            Debug.LogError($"No sprite for {spriteName}!", this);
        } else {
            SetSprite(sprite);
        }
        enabled = false;
        animationName = "";
        timeElapsed = 0f;
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

        if (image == null)
            image = GetComponent<UnityEngine.UI.Image>();
        SetAnimation(animationName);
    }

    void SetSprite(OrangeSpriteManagerSprite sprite) {
        if (spriteRenderer != null) {
            sprite.SetRendererSprite(spriteRenderer);
        }
        if (image != null) {
            if (sprite.sprite != image) {
                sprite.SetUIImageSprite(image);
                dirty = true;
            }
        }
    }

    // Update is called once per frame
    void Update() {
        if (!active) {
            return;
        }
        if (currentAnimation is null) {
            SetAnimation(animationName);
            if (currentAnimation is null) {
                Debug.LogError($"Animation was not found for {animationName} on {name}", this);
            }
        }
        float remainingTime = Time.deltaTime * playbackSpeed;
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
        var sprite = currentAnimation.GetSpriteForTime(timeElapsed);

        if (sprite != null) {
            SetSprite(sprite);
        }

        if (sprite == null) {
            if (currentAnimation.loop == false) {
                var lastAnimationDone = onAnimationDone;
                onAnimationDone = null;
                lastAnimationDone?.Invoke();
            }
            if (destroyOnDone) {
                if (currentAnimation.loop == true) {
                    Debug.LogError($"Null sprite returned for looping animation!", this);
                    return;
                }
                Destroy(gameObject);
            }
        }
    }

    public IEnumerator WaitForDone() {
        if (currentAnimation == null) Start();
        if (currentAnimation.loop) {
            Debug.LogError($"{name}'s .WaitForDone() called for looping animation '{currentAnimation.name}'!", this);
            yield break;
        }
        var timeToSleep = currentAnimation.duration - timeElapsed;
        if (timeToSleep <= 0f) yield break;
        yield return new WaitForSeconds(timeToSleep);
        // TODO: Handle stalls!!
    }
}

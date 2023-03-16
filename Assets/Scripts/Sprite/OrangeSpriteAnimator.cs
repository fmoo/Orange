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
    private OrangeSpriteDB currentAnimationSprites;
    private float timeElapsed = 0f;
    private float stallTime = 0f;

    public void AddStallTime(float addTime) {
        stallTime += addTime;
    }

    public bool HasAnimation(string animationName) {
        return sprites != null && sprites.HasAnimation(animationName);
    }

    public bool HasFrame(string frameName) {
        return sprites != null && sprites.HasFrame(frameName);
    }

    void InvokeAnimationDone() {
        var lastDone = onAnimationDone;
        onAnimationDone = null;
        lastDone?.Invoke();
    }

    bool lastSetAnimationResult = false;
    public bool SetAnimation(string animationName, bool skipReset = false) {
        // Debug.Log($"[{Time.realtimeSinceStartupAsDouble}] SetAnimation called on {name} with {animationName} skipReset={skipReset}", this);
        enabled = true;
        if (sprites == null) {
            Debug.LogError($"No sprites set on {name}'s animator", this);
            return false;
        }
        currentAnimation = sprites.GetAnimation(animationName);
        currentAnimationSprites = sprites;
        if (animationName == this.animationName) return lastSetAnimationResult;
        // Debug.Log($"SetAnimation called with {animationName} skipReset={skipReset}", this);

        if (!skipReset && (resetTimeOnChange || !(currentAnimation?.loop ?? false))) {
            InvokeAnimationDone();
            timeElapsed = 0f;
        }
        stallTime = 0f;
        this.animationName = currentAnimation != null ? animationName : "";
        OrangeSpriteManagerSprite sprite = null;
        if (currentAnimation == null) {
            Debug.LogError($"Animation not found for {name}'s '{animationName}' from {sprites?.name}", (Object)sprites ?? this);
            lastSetAnimationResult = false;
        } else {
            lastSetAnimationResult = true;
            sprite = currentAnimation.GetSpriteForTime(timeElapsed);
            if (sprite == null) {
                sprite = currentAnimation.GetLastSprite();
            }
        }
        if (sprite != null)
            SetSprite(sprite);
        return lastSetAnimationResult;
    }

    public void ResetAnimation(string animationName) {
        SetAnimation(animationName);
        ResetAnimation();
    }

    public void ResetAnimation() {
        if (currentAnimation != null && currentAnimationSprites != sprites) {
            ResetAnimation(currentAnimation.name);
        }
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

    public void SetSprite(string spriteName) {
        var sprite = sprites.GetSprite(spriteName);
        if (sprite == null) {
            Debug.LogError($"No sprite for {spriteName}!", this);
        } else {
            SetSprite(sprite);
        }
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
        if (currentAnimation is null && animationName != "") {
            SetAnimation(animationName);
            if (currentAnimation is null) {
                Debug.LogError($"Animation '{animationName}' was not found on {name}", (Object)sprites ?? this);
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
        OrangeSpriteManagerSprite sprite = null;
        if (currentAnimation != null)
            sprite = currentAnimation.GetSpriteForTime(timeElapsed);

        if (sprite != null) {
            SetSprite(sprite);
        }

        if (sprite == null) {
            if ((currentAnimation?.loop ?? false) == false) {
                var lastAnimationDone = onAnimationDone;
                onAnimationDone = null;
                lastAnimationDone?.Invoke();
            }
            if (destroyOnDone) {
                if ((currentAnimation?.loop ?? false) == true) {
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

    public void Refresh() {
        ResetAnimation(animationName);
    }
}

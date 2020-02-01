using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NamedSpriteManager : MonoBehaviour {
    private Dictionary<string, Sprite> namedSprites = new Dictionary<string, Sprite>();
    private Dictionary<string, NamedSpriteAnimation> namedAnimations = new Dictionary<string, NamedSpriteAnimation>();

    private void OnValidate() {
        BuildIndex();
    }
    private void Start() {
        BuildIndex();
    }
    private void BuildIndex() {
        foreach (var namedSprite in GetComponentsInChildren<NamedSprite>()) {
            if (namedSprite.spriteName == "" || namedSprite.spriteData == null) continue;
            namedSprites[namedSprite.spriteName] = namedSprite.spriteData;
        }
        foreach (var namedSpriteAnimation in GetComponentsInChildren<NamedSpriteAnimation>()) {
            if (namedSpriteAnimation.animationName == "" || namedSpriteAnimation.frames.Count == 0) continue;
            namedAnimations[namedSpriteAnimation.animationName] = namedSpriteAnimation;
        }
    }

    public Sprite GetNamedSprite(string name) {
        BuildIndex();
        namedSprites.TryGetValue(name, out Sprite result);
        return result;
    }

    public NamedSpriteAnimation GetNamedSpriteAnimation(string name) {
        BuildIndex();
        namedAnimations.TryGetValue(name, out NamedSpriteAnimation result);
        return result;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrangeSpriteManager : MonoBehaviour {
    public List<OrangeSpriteManagerSprite> sprites = new List<OrangeSpriteManagerSprite>();
    public List<OrangeSpriteManagerAnimation> animations = new List<OrangeSpriteManagerAnimation>();
    private Dictionary<string, OrangeSpriteManagerSprite> namedSprites =
        new Dictionary<string, OrangeSpriteManagerSprite>();
    private Dictionary<string, OrangeSpriteManagerAnimation> namedAnimations =
        new Dictionary<string, OrangeSpriteManagerAnimation>();

    public OrangeSpriteManagerSprite GetSprite(string name) {
        if (namedSprites.TryGetValue(name, out OrangeSpriteManagerSprite result)) {
            return result;
        }
        return null;
    }
    public OrangeSpriteManagerAnimation GetAnimation(string name) {
        if (namedAnimations.TryGetValue(name, out OrangeSpriteManagerAnimation result)) {
            return result;
        }
        return null;
    }

    public void SetRendererSprite(SpriteRenderer renderer, string name) {
        var ns = GetSprite(name);
        if (ns == null) {
            return;
        }
        renderer.flipX = ns.flip;
        renderer.sprite = ns.sprite;
    }

    private void OnValidate() {
        BuildIndex();
    }
    private void Start() {
        BuildIndex();
    }
    private void BuildIndex() {
        foreach (var s in sprites) {
            if (s.name == "" || s.sprite == null) continue;
            namedSprites[s.name] = s;
        }
        foreach (var a in animations) {
            a.initFrames(this);
            namedAnimations[a.name] = a;
        }
    }

    void Update() {

    }
}


[System.Serializable]
public class OrangeSpriteManagerSprite {
    public string name;
    public Sprite sprite;
    public bool flip = false;

    public void SetRendererSprite(SpriteRenderer renderer) {
        renderer.flipX = flip;
        renderer.sprite = sprite;
    }
}

[System.Serializable]
public class OrangeSpriteManagerAnimation {
    public string name;
    public string config;
    public float duration = 0.5f;

    public OrangeSpriteManagerSprite GetSpriteForTime(float timeElapsed) {
        if (frames.Count < 1) {
            return null;
        }
        float f = (timeElapsed % duration) / duration;
        return GetSpriteForIndex((int)(frames.Count * f));
    }

    public OrangeSpriteManagerSprite GetSpriteForIndex(int frameIndex) {
        return frames[frameIndex];
    }

    public void initFrames(OrangeSpriteManager m) {
        frames.Clear();
        foreach (var p in config.Split(',')) {
            frames.Add(m.GetSprite(p));
        }
    }
    List<OrangeSpriteManagerSprite> frames = new List<OrangeSpriteManagerSprite>();
}
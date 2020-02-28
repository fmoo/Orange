using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class OrangeSpriteManagerSprite {
    public string name;
    public Sprite sprite;
    public bool flip = false;

    public void SetRendererSprite(SpriteRenderer renderer) {
        renderer.flipX = flip;
        renderer.sprite = sprite;
    }
    public void SetUIImageSprite(UnityEngine.UI.Image uiImage) {
        // TODO: How to flip UI Image?
        // uiImage.flipX = flip;
        uiImage.sprite = sprite;
    }
}

[System.Serializable]
public class OrangeSpriteManagerAnimation {
    public string name;
    public string config;
    public float duration = 0.5f;
    public bool reverse = false;
    public bool loop = true;

    public OrangeSpriteManagerSprite GetSpriteForTime(float timeElapsed) {
        if (frames.Count < 1) {
            return null;
        }
        if (!loop && timeElapsed >= duration) {
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
        IEnumerable<string> split = config.Split(',');
        if (reverse) split = split.Reverse();
        foreach (var p in split) {
            frames.Add(m.GetSprite(p));
        }
    }
    List<OrangeSpriteManagerSprite> frames = new List<OrangeSpriteManagerSprite>();
}
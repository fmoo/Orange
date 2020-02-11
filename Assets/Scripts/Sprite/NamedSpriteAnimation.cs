using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete("Use OrangeSpriteManager instead.")]
public class NamedSpriteAnimation : MonoBehaviour {
    public string animationName;
    public List<Sprite> frames = new List<Sprite>();
    //public List<float> frameWeightOverride = new List<float>();
    public bool loop = true;

    // Value from 0.0 to 1.0
    public Sprite GetFrameForRatio(float weight) {
        if (weight < 0f) return null;
        int index = (int)(weight * frames.Count);
        if (index >= frames.Count) {
            if (!loop) {
                return null;
            }
            index %= frames.Count;
        }
        return frames[index];
    }

    public Sprite GetFrameForTime(float timeElapsed, float animationDuration) {
        return GetFrameForRatio(timeElapsed / animationDuration);
    }

    public Sprite GetFrameForTimePerFrame(float timeElapsed, float timePerFrame) {
        return GetFrameForRatio(timeElapsed / (timePerFrame * frames.Count));
    }
}

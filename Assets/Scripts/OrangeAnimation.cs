using System.Collections.Generic;
using UnityEngine;

public class OrangeAnimation : MonoBehaviour {
    public string animationName;
    public List<Sprite> frames = new List<Sprite>();
    public float period;

    public Sprite GetSpriteForTime(float time) {
        float n = time / period;
        n -= (int)n;
        int index = (int)(n * frames.Count);
        return frames[index];
    }
}
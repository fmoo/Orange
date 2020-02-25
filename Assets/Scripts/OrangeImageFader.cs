using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class OrangeImageFader : MonoBehaviour {
    public UnityEngine.UI.Image image;
    [ReadOnly]
    public float timeElapsed = 0.0f;
    [ReadOnly]
    public float duration = 0.0f;
    [ReadOnly]
    public bool done = true;
    [ReadOnly]
    public Color startColor = new Color(0f, 0f, 0f, 1f);
    [ReadOnly]
    public Color endColor = new Color(0f, 0f, 0f, 0f);

    public void SetColor(Color color) {
        image.color = color;
    }
    public void FinishFade() {
        SetColor(endColor);
        timeElapsed = duration;
        done = true;
    }
    public IEnumerator StartFade(Color startColor, Color endColor, float duration) {
        if (duration <= 0f) {
            SetColor(endColor);
            done = true;
            yield break;
        }
        SetColor(startColor);
        duration = 0f;
        done = false;
        while (timeElapsed < duration) {
            yield return null;
            timeElapsed += Time.deltaTime;
            SetColor(Color.Lerp(startColor, endColor, timeElapsed / duration));
        }
    }
}

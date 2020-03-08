using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Orange;

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

    private int fadeID = 0;

    void OnValidate() {
        image.raycastTarget = image.color.a > 0f;
    }

    public void SetColor(Color color) {
        image.color = color;
        // Block clicks if there is alpha
        image.raycastTarget = color.a > 0f;
    }

    public void FinishFade() {
        SetColor(endColor);
        duration = 0f;
        timeElapsed = duration;
        fadeID++;
        done = true;
    }
    public IEnumerator WaitForFadeOrInput(Color startColor, Color endColor, float duration, params InputButton[] buttons) {
        StartCoroutine(StartFade(startColor, endColor, duration));
        // yield return StartFade(startColor, endColor, duration);
        yield return WaitFor.SecondsOrInput(duration, buttons);
        FinishFade();
    }

    public IEnumerator StartFade(Color startColor, Color endColor, float duration) {
        fadeID++;
        var thisFade = fadeID;
        if (duration <= 0f) {
            SetColor(endColor);
            done = true;
            yield break;
        }
        SetColor(startColor);
        timeElapsed = 0f;
        done = false;
        while (timeElapsed < duration) {
            yield return null;
            if (fadeID != thisFade) {
                yield break;
            }
            timeElapsed += Time.deltaTime;
            SetColor(Color.Lerp(startColor, endColor, timeElapsed / duration));
        }
    }
}

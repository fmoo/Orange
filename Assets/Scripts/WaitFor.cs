using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WaitFor {
    public static IEnumerator Seconds(float seconds) {
        yield return new WaitForSeconds(seconds);
    }
    public static IEnumerator NextFrame() {
        yield return null;
    }

    /// <summary>Invokes `callback` every frame for `duration` seconds, passing the ratio
    /// from 0.0 to 1.0) of the time elapsed.</summary>
    public static IEnumerator TimeRatio(float duration, System.Action<float> callback) {
        if (duration == 0f) {
            callback(1.0f);
            yield break;
        }

        float timeElapsed = 0f;
        while (timeElapsed < duration) {
            yield return null;
            timeElapsed += Time.deltaTime;
            // clamp
            callback(timeElapsed < duration ? timeElapsed / duration : 1.0f);
        }
    }


    public static IEnumerator SecondsOrInput(float seconds, params InputButton[] buttons) {
        var t0 = Time.time;
        if (seconds <= 0f) yield break;
        while (true) {
            if (Time.time - t0 >= seconds) yield break;
            foreach (var button in buttons) {
                if (button.Down) {
                    yield return null;
                    yield break;
                }
            }
            yield return null;
        }
    }
}

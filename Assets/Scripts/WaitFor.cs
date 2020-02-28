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

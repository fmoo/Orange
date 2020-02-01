using UnityEngine;
using System.Collections;

public static class MonoBehaviourExtensions {
    public static Coroutine StartLater(this MonoBehaviour c, float inSeconds, System.Action action) {
        return c.StartCoroutine(LaterCoroutine(inSeconds, action));
    }

    public static Coroutine StartLater(this MonoBehaviour c, float inSeconds, System.Func<IEnumerator> action) {
        return c.StartCoroutine(LaterCoroutine(inSeconds, action));
    }

    private static IEnumerator LaterCoroutine(float inSeconds, System.Action action) {
        yield return new WaitForSeconds(inSeconds);
        action();
    }

    private static IEnumerator LaterCoroutine(float inSeconds, System.Func<IEnumerator> action) {
        yield return new WaitForSeconds(inSeconds);
        yield return action();
    }
}

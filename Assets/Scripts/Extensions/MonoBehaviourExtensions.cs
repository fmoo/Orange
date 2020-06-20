using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public static class MonoBehaviourExtensions {
    public static Coroutine StartCoroutine(this MonoBehaviour b, IEnumerator coroutine, System.Action onDone) {
        return b.StartCoroutine(CoroutineWithFinishAction(coroutine, onDone));
    }

    public static Coroutine StartCoroutinesSerial(this MonoBehaviour b, IEnumerator[] coroutines, System.Action onDone = null) {
        return b.StartCoroutine(CoroutinesSerial(coroutines, onDone));
    }

    public static Coroutine StartLater(this MonoBehaviour b, float inSeconds, System.Action onDone) {
        return b.StartCoroutine(LaterCoroutine(inSeconds, onDone));
    }

    public static Coroutine StartLater(this MonoBehaviour b, float inSeconds, IEnumerator coroutine) {
        return b.StartCoroutine(LaterCoroutine(inSeconds, coroutine));
    }

    public static Coroutine StartLater(this MonoBehaviour b, float inSeconds, System.Func<IEnumerator> action) {
        return b.StartCoroutine(LaterCoroutine(inSeconds, action));
    }

    private static IEnumerator CoroutinesSerial(IEnumerator[] coroutines, System.Action onDone) {
        foreach (var coroutine in coroutines) {
            yield return coroutine;
        }
        onDone();
    }

    private static IEnumerator CoroutineWithFinishAction(IEnumerator coroutine, System.Action onDone) {
        yield return coroutine;
        onDone?.Invoke();
    }

    private static IEnumerator LaterCoroutine(float inSeconds, System.Action onDone) {
        yield return new WaitForSeconds(inSeconds);
        onDone();
    }

    private static IEnumerator LaterCoroutine(float inSeconds, IEnumerator coroutine) {
        yield return new WaitForSeconds(inSeconds);
        yield return coroutine;
    }

    private static IEnumerator LaterCoroutine(float inSeconds, System.Func<IEnumerator> action) {
        yield return new WaitForSeconds(inSeconds);
        yield return action();
    }

    public static IEnumerator UnloadSceneAsync(this MonoBehaviour b, int sceneBuildIndex) {
        yield return SceneManager.UnloadSceneAsync(sceneBuildIndex);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UILayerManager : MonoBehaviour {
    public OrangeCanvasHelper canvas;

    public OrangeImageFader overlay;
    public OrangeImageFader underlay;

    public Coroutine StartSceneChange(int targetSceneBuildIndex, System.Action<Scene> onSceneLoaded = null, float fadeOutDuration = 1f) {
        return StartCoroutine(WaitForSceneChange(targetSceneBuildIndex, onSceneLoaded));
    }

    public IEnumerator WaitForSceneChange(int targetSceneBuildIndex, System.Action<Scene> onSceneLoaded = null, float fadeOutDuration = 1f) {
        canvas.HideCursor();

        yield return overlay.StartFade(Color.clear, Color.black, fadeOutDuration);
        var currentScene = SceneManager.GetActiveScene().buildIndex;
        yield return SceneManager.LoadSceneAsync(targetSceneBuildIndex, LoadSceneMode.Additive);

        var nextScene = SceneManager.GetSceneByBuildIndex(targetSceneBuildIndex);
        var newUI = nextScene.GetComponent<UILayerManager>();
        newUI.overlay.SetColor(Color.black);
        onSceneLoaded?.Invoke(nextScene);
        yield return newUI.StartCoroutine(
            newUI.UnloadSceneAsync(currentScene),
            () => {
                newUI.StartCoroutine(newUI.overlay.StartFade(Color.black, Color.clear, 1.0f));
            }
        );
    }
}

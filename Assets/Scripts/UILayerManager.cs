using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UILayerManager : MonoBehaviour {
    public OrangeCanvasHelper canvas;
    public OrangeImageFader overlay;
    public OrangeImageFader underlay;

    [SerializeField] OrangeAudioBank audioBank;
    [SerializeField] AudioSource audioSource;

    private const string CONFIRM_SOUND = "confirm";
    private const string CANCEL_SOUND = "cancel";
    private const string HOVER_SOUND = "selectionChanged";

    public void PlayConfirmSound() {
        PlaySound(CONFIRM_SOUND);
    }
    public void PlayCancelSound() {
        PlaySound(CANCEL_SOUND);
    }
    public void PlayHoverSound() {
        PlaySound(HOVER_SOUND);
    }
    public System.Action WithConfirmSound(System.Action action) {
        return WithAudio("confirm", action);
    }
    public System.Action WithCancelSound(System.Action action) {
        return WithAudio("cancel", action);
    }
    public System.Action WithHoverSound(System.Action action) {
        return WithAudio("selectionChanged", action);
    }
    private void PlaySound(string clipName) {
        audioBank.PlaySound(audioSource, clipName);
    }
    private System.Action WithAudio(string clipName, System.Action action) {
        return () => {
            PlaySound(clipName);
            action();
        };
    }
    public Coroutine StartSceneChange(int targetSceneBuildIndex, System.Action<Scene> onSceneLoaded = null, float fadeOutDuration = 1f) {
        return StartCoroutine(WaitForSceneChange(targetSceneBuildIndex, onSceneLoaded));
    }

    public IEnumerator WaitForSceneChange(int targetSceneBuildIndex, System.Action<Scene> onSceneLoaded = null, float fadeOutDuration = 1f) {
        if (canvas != null) {
            canvas.HideCursor();
        }

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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UILayerManager : MonoBehaviour {
    public OrangeCanvasHelper canvas;
    public OrangeImageFader overlay;
    public OrangeImageFader underlay;

    [SerializeField] OrangeAudioBank bgmBank;
    [SerializeField] OrangeAudioBank soundBank;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioSource audioLoopSource;

    [SerializeField] string confirmSoundName = "confirm";
    [SerializeField] string cancelSoundName = "cancel";
    [SerializeField] string hoverSoundName = "selectionChanged";
    [SerializeField] string errorSoundName = "error";

    public void PlayBGM(string clipName) {
        bgmBank.PlayLoopable(audioSource, audioLoopSource, clipName);
    }
    public void PlayConfirmSound() {
        PlaySFX(confirmSoundName);
    }
    public void PlayCancelSound() {
        PlaySFX(cancelSoundName);
    }
    public void PlayHoverSound() {
        PlaySFX(hoverSoundName);
    }
    public void PlayErrorSound() {
        PlaySFX(errorSoundName);
    }
    public System.Action WithConfirmSound(System.Action action) {
        return WithSound("confirm", action);
    }
    public System.Action WithCancelSound(System.Action action) {
        return WithSound("cancel", action);
    }
    public System.Action WithHoverSound(System.Action action) {
        return WithSound("selectionChanged", action);
    }
    public void PlaySFX(string clipName) {
        if (soundBank == null) {
            Debug.LogError($"soundBank is null on UILayerManager", this);
            return;
        }
        soundBank.PlayEffect(audioSource, clipName);
    }
    private System.Action WithSound(string clipName, System.Action action) {
        return () => {
            PlaySFX(clipName);
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

        StartAudioFade(0f, fadeOutDuration);
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
                // TODO: Fade in the Audio?
            }
        );
    }

    public void StartAudioFade(float targetVolume, float duration) {
        if (audioSource != null) StartCoroutine(audioSource.WaitForAudioFade(targetVolume, duration));
        if (audioLoopSource != null) StartCoroutine(audioLoopSource.WaitForAudioFade(targetVolume, duration));
    }

    public void PushLayer(UILayer layer) {
        // Debug.Log($"PushLayer: {layer}");
        PlayConfirmSound();
        var ctx = new UILayerPushContext();
        var selectedObject = EventSystem.current.currentSelectedGameObject;
        if (selectedObject != null) ctx.selection = selectedObject.GetComponent<Selectable>();
        ctx.visibleLayers.AddRange(GetComponentsInChildren<UILayer>().Where(l => l.shown));
        ctx.visibleLayers.ForEach(l => l.Hide());
        navigationStack.Push(ctx);
        layer.Show();
    }

    public void PopLayer(bool useConfirmSound = false) {
        if (navigationStack.IsNullOrEmpty()) return;
        if (useConfirmSound) PlayConfirmSound(); else PlayCancelSound();
        foreach (var layer in GetComponentsInChildren<UILayer>().Where(layer => layer.shown)) {
            layer.Hide();
        }
        var popped = navigationStack.Pop();
        // Debug.Log($"PopLayer: {popped}");
        popped.visibleLayers.ForEach(layer => layer.Show(false));
        if (popped.selection != null) {
            popped.selection.Select();
        }
    }

    private Stack<UILayerPushContext> navigationStack = new Stack<UILayerPushContext>();
    private class UILayerPushContext {
        public List<UILayer> visibleLayers = new List<UILayer>();
        public Selectable selection = null;
    }
}

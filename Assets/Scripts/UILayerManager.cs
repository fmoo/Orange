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

    string GetSoundName(UISound sound) {
        if (sound == UISound.CONFIRM) {
            return confirmSoundName;
        } else if (sound == UISound.CANCEL) {
            return cancelSoundName;
        } else if (sound == UISound.ERROR) {
            return errorSoundName;
        } else if (sound == UISound.HOVER) {
            return hoverSoundName;
        } else if (sound == UISound.NONE) {
            return "";
        }
        Debug.LogError($"Unknown Sound {sound}");
        return "";
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
    public void PlaySFX(UISound sound) {
        PlaySFX(GetSoundName(sound));
    }
    public void PlaySFX(string clipName) {
        if (clipName == "") return;
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

    public void PushLayer(UILayer layer, UISound sound = UISound.CONFIRM) {
        System.Action doPush = () => {
            // Debug.Log($"PushLayer: {layer}");
            PlaySFX(GetSoundName(sound));
            var ctx = new UILayerPushContext();
            var selectedObject = EventSystem.current.currentSelectedGameObject;
            if (selectedObject != null) ctx.selection = selectedObject.GetComponent<Selectable>();
            ctx.visibleLayers.AddRange(GetComponentsInChildren<UILayer>().Where(l => l.shown));
            ctx.visibleLayers.ForEach(l => l.Hide());
            navigationStack.Push(ctx);
            layer.Show();
        };
        if (navStackDepth > 0) {
            doPush();
        } else {
            this.StartCoroutine(GenUIStartTransition(), doPush);
        }
    }

    public void PopLayer(UISound sound = UISound.CANCEL) {
        if (navigationStack.IsNullOrEmpty()) return;
        System.Action doPop = () => {
            PlaySFX(GetSoundName(sound));
            foreach (var layer in GetComponentsInChildren<UILayer>().Where(layer => layer.shown)) {
                layer.Hide();
            }
            var popped = navigationStack.Pop();
            // Debug.Log($"PopLayer: {popped}");
            popped.visibleLayers.ForEach(layer => layer.Show(false));
            if (popped.selection != null) {
                popped.selection.Select();
            }
        };
        if (navStackDepth > 1) {
            doPop();
        } else {
            this.StartCoroutine(GenUIEndTransition(), doPop);
        }
    }

    IEnumerator GenUIStartTransition() {
        if (underlay == null) yield break;
        yield return underlay.StartFade(Color.clear, Color.black.WithAlpha(0.5f), 0.3f);
    }

    IEnumerator GenUIEndTransition() {
        if (underlay == null) yield break;
        yield return underlay.StartFade(Color.black.WithAlpha(0.5f), Color.clear, 0.3f);
    }

    public int navStackDepth {
        get {
            return navigationStack.Count;
        }
    }

    public enum UISound {
        NONE = 0,
        CONFIRM = 1,
        CANCEL = 2,
        HOVER = 3,
        ERROR = 4,
    }

    private Stack<UILayerPushContext> navigationStack = new Stack<UILayerPushContext>();
    private class UILayerPushContext {
        public List<UILayer> visibleLayers = new List<UILayer>();
        public Selectable selection = null;
    }
}

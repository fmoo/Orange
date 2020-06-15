using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using UnityEngine.EventSystems;

[ExecuteInEditMode]
public class OrangeCanvasHelper : MonoBehaviour {
    [SerializeField] PixelPerfectCamera pixelPerfectCamera;
    [SerializeField] Camera targetCamera;
    [SerializeField] CanvasScaler canvasScaler;

    [SerializeField] OrangeCursor uiCursor;
    [SerializeField] OrangeImageFader blackoutLayer;

    public RectTransform overlayContentContainer;

    private IEnumerable<Selectable> GetUISelectables() {
        foreach (var selectable in gameObject.GetComponentsInChildren<Button>(false  /* include_inactive */)) {
            if (selectable.IsInteractable() && selectable.navigation.mode != Navigation.Mode.None) {
                var rt = selectable.GetComponent<RectTransform>();
                if (rt == null) continue;
                if (rt.rect.width == 0f || rt.rect.height == 0f) continue;
                yield return selectable;
            }
        }
    }
    private bool GetUIHasSelectables() {
        var buttons = GetUISelectables();
        return buttons.Any();
    }
    private bool GetAnyUIVisible() {
        var elements = gameObject.GetComponentsInChildren<RectTransform>(false /* include_inactive */);
        return elements.Length > 1;
    }

    public void SetSelectable(Selectable selectable, bool isSelectable) {
        if (isSelectable) {
            EnableSelectable(selectable);
        } else {
            DisableSelectable(selectable);
        }
    }

    public void DisableSelectable(Selectable selectable) {
        selectable.interactable = false;
        if (selectable == EventSystem.current.currentSelectedGameObject) {
            var selectables = GetUISelectables();
            if (selectables.Any()) {
                EventSystem.current.SetSelectedGameObject(selectables.First().gameObject);
            } else {
                HideCursor();
            }
        }
    }
    public void EnableSelectable(Selectable selectable) {
        selectable.interactable = true;
        if (selectable.gameObject.activeInHierarchy) {
            ShowCursor();
        }
    }

    public void ShowCursor() {
        // var currentSelectable = EventSystem.current.currentSelectedGameObject?.GetComponent<Selectable>();
        // if (currentSelectable == null || !currentSelectable.IsInteractable()) {
        //     var selectables = GetUISelectables();
        //     if (selectables.Any()) {
        //         currentSelectable = selectables.First();
        //         Debug.Log("ShowCursor!");
        //         SelectImmediately(currentSelectable);
        //     }
        // }
        // TODO: if the cursor was *not* visible, jump the cursor *immediately* to the currentSelectedGameObject
        // var wasActive = uiCursor.gameObject.activeSelf;
        if (uiCursor != null) {
            uiCursor.gameObject.SetActive(true);
        }
        // DoFocusIfNone();
        // if (!wasActive) {
        //     uiCursor.SnapToTarget(currentSelectable);
        // }
    }
    public void HideCursor() {
        if (uiCursor != null) {
            uiCursor.gameObject.SetActive(false);
        }
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void ShowUIPanel(RectTransform uiPanel) {
        uiPanel.gameObject.SetActive(true);
        var cg = uiPanel.GetOrCreateComponent<CanvasGroup>();
        cg.blocksRaycasts = true;
        cg.alpha = 1f;
        cg.interactable = true;

        if (GetUIHasSelectables()) {
            ShowCursor();
        }
        // DoFocusIfNone();
    }
    public void ShowUIPanel(Component component) {
        // TODO: Instead of Component, should this be an intermediate class that provides
        // an API to get the RectTransform?
        var rt = component.GetComponent<RectTransform>();
        if (rt == null) {
            Debug.LogError($"Attempt to ShowUIPanel on non UI Element, '{component.name}'!");
            return;
        }
        ShowUIPanel(rt);
    }
    public void HideUIPanel(RectTransform uiPanel) {
        var cg = uiPanel.GetOrCreateComponent<CanvasGroup>();
        cg.blocksRaycasts = false;
        cg.alpha = 0f;
        cg.interactable = false;
        // uiPanel.gameObject.SetActive(false);
        if (!GetUIHasSelectables()) {
            HideCursor();
        }
        // DoFocusIfNone();
    }
    public void HideUIPanel(Component component) {
        var rt = component.GetComponent<RectTransform>();
        if (rt == null) {
            Debug.LogError($"Attempt to ShowUIPanel on non UI Element, '{component.name}'!");
            return;
        }
        HideUIPanel(rt);
    }

    void Update() {
        // DoFocusIfNone(true);
        DoFocusIfNone();
        UpdateScaler();
        UpdateContainer();
    }

    void OnValidate() {
        if (canvasScaler == null)
            canvasScaler = GetComponent<CanvasScaler>();
        if (targetCamera == null)
            targetCamera = Camera.main ?? Camera.current;
        if (pixelPerfectCamera == null)
            pixelPerfectCamera = targetCamera.GetComponent<PixelPerfectCamera>();
    }

    void Start() {
        // uiCursor must render on top of menus.  Put it last.
        if (uiCursor != null)
            uiCursor.transform.SetAsLastSibling();
        // Unless we have a blackout/fader layer.  Then put THAT last.
        if (blackoutLayer != null)
            blackoutLayer.transform.SetAsLastSibling();
    }

    void UpdateScaler() {
        if (canvasScaler == null || pixelPerfectCamera == null) return;
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
        canvasScaler.scaleFactor = Mathf.Min(Screen.height / pixelPerfectCamera.refResolutionY, Screen.width / pixelPerfectCamera.refResolutionX);
    }

    void UpdateContainer() {
        if (overlayContentContainer == null || targetCamera == null) return;
        overlayContentContainer.anchorMin = targetCamera.rect.min;
        overlayContentContainer.anchorMax = targetCamera.rect.max;
        overlayContentContainer.offsetMin = Vector2.zero;
        overlayContentContainer.offsetMax = Vector2.zero;
    }

    void DoFocusIfNone(bool onlyIfNull = false) {
        if (uiCursor == null) return;
        if (!uiCursor.gameObject.activeInHierarchy) return;
        var selection = EventSystem.current?.currentSelectedGameObject;
        if (selection == null) {
            var selectables = GetUISelectables();
            if (selectables.Any()) {
                SelectImmediately(selectables.First());
            }
        } else if (!onlyIfNull) {
            var selectable = selection.GetComponent<Selectable>();
            if (!selectable.gameObject.activeInHierarchy || !selectable.IsInteractable()) {
                var selectables = GetUISelectables();
                if (selectables.Any()) {
                    SelectImmediately(selectables.First());
                }
            }
        }
    }

    public void SelectImmediately(Selectable selectable) {
        selectable.Select();
        uiCursor.SnapToTarget(selectable);
    }
}

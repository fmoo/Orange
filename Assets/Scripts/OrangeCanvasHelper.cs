using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[ExecuteInEditMode]
public class OrangeCanvasHelper : MonoBehaviour {
    [SerializeField] OrangeDisplay display;
    [SerializeField] UnityEngine.U2D.PixelPerfectCamera ppCamera;
    [SerializeField] OrangeCursor uiCursor;
    [SerializeField] OrangeImageFader blackoutLayer;
    private CanvasScaler canvasScaler;

    private IEnumerable<Selectable> GetUISelectables() {
        foreach (var selectable in gameObject.GetComponentsInChildren<Selectable>(false  /* include_inactive */)) {
            if (selectable.interactable) {
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
        var currentSelectable = EventSystem.current.currentSelectedGameObject?.GetComponent<Selectable>();
        if (currentSelectable?.interactable != true) {
            var selectables = GetUISelectables();
            if (selectables.Any()) {
                currentSelectable = selectables.First();
                SelectImmediately(currentSelectable);
            }
        }
        // TODO: if the cursor was *not* visible, jump the cursor *immediately* to the currentSelectedGameObject
        var wasActive = uiCursor.gameObject.activeSelf;
        uiCursor.gameObject.SetActive(true);
        DoFocusIfNone();
        if (!wasActive) {
            uiCursor.SnapToTarget(currentSelectable);
        }
    }
    public void HideCursor() {
        uiCursor.gameObject.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void ShowUIPanel(RectTransform uiPanel) {
        uiPanel.gameObject.SetActive(true);
        if (GetUIHasSelectables()) {
            ShowCursor();
        }
        DoFocusIfNone();
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
        uiPanel.gameObject.SetActive(false);
        if (!GetUIHasSelectables()) {
            HideCursor();
        }
        DoFocusIfNone();
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
        DoFocusIfNone(true);
    }

    void Start() {
        // uiCursor must render on top of menus.  Put it last.
        if (uiCursor != null)
            uiCursor.transform.SetAsLastSibling();
        // Unless we have a blackout/fader layer.  Then put THAT last.
        if (blackoutLayer != null)
            blackoutLayer.transform.SetAsLastSibling();
        display = OrangeDisplay.Instance;
        DoUpdateSize();
        display.OnResolutionChanged += DoUpdateSize;
    }

    void DoUpdateSize() {
        var referenceHeight = ppCamera.refResolutionY;
        var referenceWidth = ppCamera.refResolutionX;
        if (this == null || gameObject == null) return;
        canvasScaler = gameObject.GetOrCreateComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
        canvasScaler.scaleFactor = Mathf.Min(Screen.height / referenceHeight, Screen.width / referenceWidth);
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
            if (!selectable.gameObject.activeInHierarchy || !selectable.interactable) {
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

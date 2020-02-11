using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[ExecuteInEditMode]
public class OrangeCanvasHelper : MonoBehaviour {
    new public UnityEngine.U2D.PixelPerfectCamera camera;
    private CanvasScaler canvasScaler;
    private int lastHeight;
    private int lastReferenceHeight;

    public OrangeCursor uiCursor;
    // TODO: maybe this should be event driven?
    public List<GameObject> disableObjectsForUI = new List<GameObject>();

    private IList<Selectable> GetUISelectables() {
        var result = new List<Selectable>();
        foreach (var selectable in gameObject.GetComponentsInChildren<Selectable>(false)) {
            if (selectable.interactable) {
                result.Add(selectable);
            }
        }
        return result;
    }
    private bool GetUIHasSelectables() {
        var buttons = GetUISelectables();
        return buttons.Count > 0;
    }
    private bool GetAnyUIVisible() {
        var elements = gameObject.GetComponentsInChildren<RectTransform>(false);
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
            if (selectables.Count > 0) {
                EventSystem.current.SetSelectedGameObject(selectables[0].gameObject);
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
            if (selectables.Count > 0) {
                currentSelectable = selectables[0];
                currentSelectable.Select();
            }
        }
        // TODO: if the cursor was *not* visible, jump the cursor *immediately* to the currentSelectedGameObject
        var wasActive = uiCursor.gameObject.activeSelf;
        uiCursor.gameObject.SetActive(true);
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
        } else {
            Debug.Log("UI Panel shown without any interactable selectables??");
        }
        if (GetAnyUIVisible()) {
            // TODO: Hide all of disableObjectsForUI ?
        }
    }
    public void HideUIPanel(RectTransform uiPanel) {
        uiPanel.gameObject.SetActive(false);
        if (!GetUIHasSelectables()) {
            HideCursor();
        }
        if (GetAnyUIVisible()) {
            // TODO: Show all of disableObjectsForUI ?
        }
    }

    void Start() {
        DoUpdateSize();
    }

    void Awake() {
        DoUpdateSize();
    }

    void Update() {
        DoUpdateSize();
        DoFocusIfNone();
    }

    void DoUpdateSize() {
        if (camera == null) return;
        var referenceHeight = camera.refResolutionY;
        if (Screen.height == lastHeight && referenceHeight == lastReferenceHeight) return;
        if (referenceHeight <= 0) return;
        canvasScaler = gameObject.GetOrCreateComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
        canvasScaler.scaleFactor = Screen.height / referenceHeight;
        lastHeight = Screen.height;
        lastReferenceHeight = referenceHeight;
    }

    void DoFocusIfNone() {
        if (uiCursor == null) return;
        if (!uiCursor.gameObject.activeSelf) return;
        if (EventSystem.current.currentSelectedGameObject != null) return;
        var selectables = GetUISelectables();
        if (selectables.Count > 0) {
            selectables[0].Select();
        }
    }
}

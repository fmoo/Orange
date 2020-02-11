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

    private Selectable[] GetUISelectables() {
        return gameObject.GetComponentsInChildren<Selectable>(false);
    }
    private bool GetUIHasSelectables() {
        var buttons = GetUISelectables();
        return buttons.Length > 0;
    }
    private bool GetAnyUIVisible() {
        var elements = gameObject.GetComponentsInChildren<RectTransform>(false);
        return elements.Length > 1;
    }

    public void DisableSelectable(Selectable selectable) {
        selectable.interactable = false;
        if (selectable == EventSystem.current.currentSelectedGameObject) {
            var selectables = GetUISelectables();
            if (selectables.Length > 0) {
                EventSystem.current.SetSelectedGameObject(selectables[0].gameObject);
            } else {
                EventSystem.current.SetSelectedGameObject(null);
                uiCursor.gameObject.SetActive(false);
            }
        }
    }
    public void EnableSelectable(Selectable selectable) {
        selectable.interactable = true;
        uiCursor.gameObject.SetActive(true);
    }

    public void ShowUIPanel(RectTransform uiPanel) {
        uiPanel.gameObject.SetActive(true);
        if (GetUIHasSelectables()) {
            var wasActive = uiCursor.gameObject.activeSelf;
            uiCursor.gameObject.SetActive(true);
            if (!wasActive) {
                GetUISelectables()[0].Select();
            }
        }
        if (GetAnyUIVisible()) {
            // TODO: Hide all of disableObjectsForUI ?
        }
    }
    public void HideUIPanel(RectTransform uiPanel) {
        uiPanel.gameObject.SetActive(false);
        if (!GetUIHasSelectables()) {
            uiCursor.gameObject.SetActive(false);
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
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
}

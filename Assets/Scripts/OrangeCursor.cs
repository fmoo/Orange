using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OrangeCursor : MonoBehaviour {
    private RectTransform rectTransform;
    private Vector2 scaleCoeff;

    public Vector2 padding;

    void OnValidate() {
        var renderer = GetComponent<Image>();
        renderer.raycastTarget = false;
        rectTransform = this.GetOrCreateComponent<RectTransform>();
        scaleCoeff = new Vector2(1f / rectTransform.localScale.x, 1f / rectTransform.localScale.y);
    }

    void Start() {
        OnValidate();
    }

    // Update is called once per frame
    private List<RaycastResult> pointerResults = new List<RaycastResult>();
    private Vector3 lastMousePosition = Vector3.zero;
    void Update() {
        var mousePosition = Input.mousePosition;
        if (mousePosition != lastMousePosition) {
            // TODO: Can this be memoized/reused?
            PointerEventData pointerData = new PointerEventData(EventSystem.current) {
                pointerId = -1,
            };
            pointerData.position = Input.mousePosition;
            EventSystem.current.RaycastAll(pointerData, pointerResults);
            foreach (var pointerResult in pointerResults) {
                var selectable = pointerResult.gameObject.GetComponent<Selectable>();
                if (selectable != null && selectable.gameObject != EventSystem.current.currentSelectedGameObject && selectable.interactable) {
                    selectable.Select();
                }
            }
            lastMousePosition = mousePosition;
        }

        var currentSelection = EventSystem.current.currentSelectedGameObject;
        if (currentSelection == null) {
            return;
        }
        // TODO: Add a flag to support *animating* movement / size to the new values
        rectTransform.position = currentSelection.transform.position;
        rectTransform.sizeDelta = padding + currentSelection.GetComponent<RectTransform>().sizeDelta * scaleCoeff;
    }

    public void SnapToTarget(Selectable target) {
        if (target == null) return;
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        var targetTransform = target.GetComponent<RectTransform>();
        rectTransform.position = targetTransform.position;
        rectTransform.sizeDelta = padding + targetTransform.sizeDelta * scaleCoeff;
    }
}

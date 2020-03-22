using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OrangeCursor : MonoBehaviour {
    [SerializeField] private OrangeAudioBank audioBank;
    [NaughtyAttributes.Dropdown("GetAudioDropdown")]
    [SerializeField] private string selectionChangedSound;
    [NaughtyAttributes.Dropdown("GetAudioDropdown")]
    [SerializeField] private AudioSource audioSource;
    private GameObject lastSelection;

    private RectTransform rectTransform;
    private Vector2 scaleCoeff;

    public Vector2 padding;

    public bool reparentWithSelection = false;
    [SerializeField] private Transform defaultParent;

    public bool animateMotion = false;
    public float scaleSpeed = 800f;
    public float moveSpeed = 800f;

    public NaughtyAttributes.DropdownList<string> GetAudioDropdown() {
        if (audioBank == null) return new NaughtyAttributes.DropdownList<string>();
        return audioBank.GetDropdown();
    }

    void OnValidate() {
        if (defaultParent == null) defaultParent = transform;
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

    void UpdateForMouse() {
        if (MouseButton.LEFT.Pressed) return;
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
                if (selectable != null && selectable.gameObject != EventSystem.current.currentSelectedGameObject && selectable.enabled && selectable.IsInteractable()) {
                    selectable.Select();
                }
            }
            lastMousePosition = mousePosition;
        }
    }
    void Update() {
        UpdateForMouse();

        var currentSelection = EventSystem.current.currentSelectedGameObject;
        if (currentSelection != lastSelection && currentSelection != null) {
            if (audioSource != null && audioBank != null) {
                if (currentSelection.GetComponent<OrangeSilentButton>() == null) {
                    // Debug.Log($"<> Selection Change: {lastSelection} -> {currentSelection}");
                    audioBank.PlayEffect(audioSource, selectionChangedSound);
                }
                // Autoscroll parent scroll rect containers so that the selection is visible.
                // This logic probably doesn't work with nested scrollrects.  Don't nest scrollrects. 
                var scroller = currentSelection.GetComponentInParent<ScrollRect>();
                if (scroller != null && currentSelection.GetComponent<Scrollbar>() == null) {
                    var corners = scroller.GetComponent<RectTransform>().GetWorldCorners();
                    var scrollTop = corners[3].y;
                    var scrollBottom = corners[1].y;
                    currentSelection.GetComponent<RectTransform>().GetWorldCorners(corners);
                    var itemTop = corners[3].y;
                    var itemBottom = corners[1].y;

                    if (scrollTop > itemTop) {
                        scroller.content.transform.position += Vector3.up * (scrollTop - itemTop);
                    } else if (scrollBottom < itemBottom) {
                        scroller.content.transform.position -= Vector3.up * (itemBottom - scrollBottom);
                    }
                }
                if (reparentWithSelection) {
                    transform.SetParent(currentSelection.transform.parent);
                    transform.SetAsFirstSibling();
                    transform.SetSiblingIndex(currentSelection.transform.GetSiblingIndex() - 1);
                }
            }
        }
        lastSelection = currentSelection;
        if (currentSelection == null) {
            return;
        }

        RectTransform targetRect = currentSelection.GetComponent<RectTransform>();
        var targetSize = padding + targetRect.rect.size * scaleCoeff;

        if (animateMotion) {
            rectTransform.position = Vector3.MoveTowards(rectTransform.position, targetRect.GetWorldCenter(), Time.deltaTime * moveSpeed);
            rectTransform.sizeDelta = Vector2.MoveTowards(rectTransform.sizeDelta, targetSize, Time.deltaTime * scaleSpeed);
        } else {
            rectTransform.position = targetRect.GetWorldCenter();
            rectTransform.sizeDelta = targetSize;
        }
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

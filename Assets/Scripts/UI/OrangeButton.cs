using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class OrangeButton : Button {
    public Selectable upSelectable;
    public Selectable downSelectable;
    public Selectable leftSelectable;
    public Selectable rightSelectable;

    public UnityEvent onSelect;
    public UnityEvent onDeselect;
    public GameObject[] activateGameObjectsOnFocus;

    public override Selectable FindSelectableOnUp() {
        return upSelectable != null && upSelectable.isActiveAndEnabled && IsInteractable() ? upSelectable : base.FindSelectableOnUp();
    }

    public override Selectable FindSelectableOnDown() {
        return downSelectable != null && downSelectable.isActiveAndEnabled && IsInteractable() ? downSelectable : base.FindSelectableOnDown();
    }

    public override Selectable FindSelectableOnLeft() {
        return leftSelectable != null && leftSelectable.isActiveAndEnabled && IsInteractable() ? leftSelectable : base.FindSelectableOnLeft();
    }

    public override Selectable FindSelectableOnRight() {
        return rightSelectable != null && rightSelectable.isActiveAndEnabled && IsInteractable() ? rightSelectable : base.FindSelectableOnRight();
    }

    public override void OnSubmit(BaseEventData eventData) {
        // TODO: if we want to bypass/abort for other reasons, do it here
        base.OnSubmit(eventData);
    }

    public override void OnSelect(BaseEventData eventData) {
        base.OnSelect(eventData);
        foreach (var go in activateGameObjectsOnFocus) {
            go.SetActive(true);
        }
        onSelect?.Invoke();
    }

    public override void OnDeselect(BaseEventData eventData) {
        base.OnDeselect(eventData);
        foreach (var go in activateGameObjectsOnFocus) {
            go.SetActive(false);
        }
        onDeselect?.Invoke();
    }

    public override void OnPointerClick(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        // TODO: if we want to bypass/abort for other reasons, do it here
        base.OnPointerClick(eventData);
    }
}

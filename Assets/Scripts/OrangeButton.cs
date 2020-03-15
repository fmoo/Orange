using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class OrangeButton : UnityEngine.UI.Button {
    
    private Graphic[] graphics;

    public System.Action onSelect = null;

    public override void OnSelect(BaseEventData eventData) {
        base.OnSelect(eventData);
        onSelect?.Invoke();
    }

    protected override void DoStateTransition(SelectionState state, bool instant) {
        Color color;
        switch (state) {
            case Selectable.SelectionState.Normal:
                color = this.colors.normalColor;
                break;
            case Selectable.SelectionState.Highlighted:
                color = this.colors.highlightedColor;
                break;
            case Selectable.SelectionState.Pressed:
                color = this.colors.pressedColor;
                break;
            case Selectable.SelectionState.Disabled:
                color = this.colors.disabledColor;
                break;
            case Selectable.SelectionState.Selected:
                color = this.colors.selectedColor;
                break;
            default:
                color = Color.black;
                break;
        }
        if (base.gameObject.activeInHierarchy) {
            switch (this.transition) {
                case Selectable.Transition.ColorTint:
                    ColorTween(color * this.colors.colorMultiplier, instant);
                    break;
            }
        }
    }

    private void ColorTween(Color targetColor, bool instant) {
        if (graphics == null) {
            graphics = GetComponentsInChildren<Graphic>();
        }
        foreach (var graphic in graphics) {
            graphic.CrossFadeColor(targetColor, (!instant) ? this.colors.fadeDuration : 0f, true, true);
        }
    }
}

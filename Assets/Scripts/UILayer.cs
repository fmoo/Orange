using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILayer : MonoBehaviour {
    [SerializeField] protected UILayerManager ui;
    public OrangePanel autofocusPanel = null;
    public System.Action onBeforeShow = null;
    public System.Action onBeforeHide = null;

    virtual protected void OnValidate() {
        if (ui == null)
            ui = GetComponentInParent<UILayerManager>();
    }

    public bool shown {
        get {
            return this.gameObject.activeInHierarchy && this.GetComponent<CanvasGroup>()?.interactable == true;
        }
    }

    virtual protected void BeforeShow() { }
    virtual protected void BeforeHide() { }

    public void Show() {
        onBeforeShow?.Invoke();
        BeforeShow();
        var wasVisible = this.shown;
        ui.canvas.ShowUIPanel(this);
        if (!wasVisible && autofocusPanel != null) {
            autofocusPanel.SelectFirstElement<UnityEngine.UI.Button>();
        }
    }

    public void Hide() {
        BeforeHide();
        onBeforeHide?.Invoke();
        ui.canvas.HideUIPanel(this);
    }
}
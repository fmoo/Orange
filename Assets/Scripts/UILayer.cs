using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILayer : MonoBehaviour {
    [SerializeField] protected UILayerManager ui;
    public System.Action onBeforeShow = null;
    public System.Action onBeforeHide = null;

    virtual protected void OnValidate() {
        if (ui == null)
            ui = GetComponentInParent<UILayerManager>();
    }

    virtual protected void BeforeShow() { }
    virtual protected void BeforeHide() { }

    public void Show() {
        onBeforeShow?.Invoke();
        BeforeShow();
        ui.canvas.ShowUIPanel(this);
    }

    public void Hide() {
        BeforeHide();
        onBeforeHide?.Invoke();
        ui.canvas.HideUIPanel(this);
    }
}
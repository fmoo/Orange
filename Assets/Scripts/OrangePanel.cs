using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrangePanel : MonoBehaviour {
    public RectTransform rectTransform;
    public Image backgroundImage;

    public void Show() {
        rectTransform.gameObject.SetActive(true);
    }
    public void ShowWithColor(Color color) {
        Show();
        if (backgroundImage != null)
            backgroundImage.color = color;
    }

    public void Hide() {
        rectTransform.gameObject.SetActive(false);
    }

    void OnValidate() {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
    }

    void Start() {
        OnValidate();
    }

    void Update() {

    }
}

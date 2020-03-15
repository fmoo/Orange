using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrangePanel : MonoBehaviour {
    [SerializeField] RectTransform rectTransform;
    [SerializeField] Image backgroundImage;
    [SerializeField] Color defaultColor = new Color(Color.blue.r, Color.blue.g, Color.blue.b, 0.75f);
    [SerializeField] CanvasGroup canvasGroup;

    public void Show() {
        rectTransform.gameObject.SetActive(true);
    }
    public void ShowWithColor(Color color) {
        Show();
        if (backgroundImage != null)  {
            defaultColor = color;
            backgroundImage.color = color;
        }
    }

    public void Hide() {
        rectTransform.gameObject.SetActive(false);
    }

    void OnValidate() {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
        if (backgroundImage != null)
            backgroundImage.color = defaultColor;
        else
            Debug.Log($"Panel {name} missing backgroundImage reference!");
    }

    void Start() {
        OnValidate();
    }
}

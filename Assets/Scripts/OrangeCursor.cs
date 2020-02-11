using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrangeCursor : MonoBehaviour {
    private RectTransform rectTransform;
    private Vector2 scaleCoeff;

    public Vector2 padding;

    // Start is called before the first frame update
    void Start() {
        rectTransform = gameObject.GetOrCreateComponent<RectTransform>();
        scaleCoeff = new Vector2(1f / rectTransform.localScale.x, 1f / rectTransform.localScale.y);
    }

    // Update is called once per frame
    void Update() {
        var currentSelection = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        if (currentSelection == null) {
            return;
        }
        rectTransform.position = currentSelection.transform.position;
        rectTransform.sizeDelta = padding + currentSelection.GetComponent<RectTransform>().sizeDelta * scaleCoeff ;
    }
}

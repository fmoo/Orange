using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
        var currentSelection = EventSystem.current.currentSelectedGameObject;
        if (currentSelection == null) {
            return;
        }
        // TODO: Add a flag to support *animating* movement / size to the new values
        rectTransform.position = currentSelection.transform.position;
        rectTransform.sizeDelta = padding + currentSelection.GetComponent<RectTransform>().sizeDelta * scaleCoeff ;
    }
}

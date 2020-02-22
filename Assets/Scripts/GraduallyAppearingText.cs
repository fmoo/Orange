using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/**
 * Attach this behavior to an object with a UIText to cause the text to
 * gradually appear instead of appearing all at once.
 */
public class GraduallyAppearingText : MonoBehaviour {
    private float timeElapsed = 0.0f;
    private bool done = false;

    /**
	 * Override the text to render.  
	 */
    public string renderText = "";

    /**
	 * The number of seconds per character displayed.
	 */
    public float charDisplaySpeed = 0.01f;

    public Text uiText;
    public TMP_Text tmpText;

    /**
	 * Helper to set new text for rendering.  Resets the internal state.
	 */
    public void SetRenderText(string text) {
        renderText = text;
        Reset();
		SetUITextContents("");
    }

    /**
	 * Reset the current state
	 */
    public void Reset() {
        done = false;
        timeElapsed = 0.0f;
    }

    public bool IsFinished() {
        return done;
    }

    /**
	 * Helper function for handling button presses from external sources
	 */
    public void SkipToEnd() {
        done = true;
        UpdateText();
    }

    void OnValidate() {
        if (uiText == null)
            uiText = GetComponent<Text>();
        if (tmpText == null)
            tmpText = GetComponent<TMP_Text>();
    }

    /**
	 * Initialize the UIText settings (enable rich-text) and local render text.
	 */
    void Start() {
		OnValidate();
        if (uiText != null) {
            uiText.supportRichText = true;
            if (renderText == "") {
                renderText = uiText.text;
            }
        } else if (tmpText != null) {
            if (renderText == "") {
                renderText = tmpText.text;
            }
        }
    }

    // Update is called once per frame
    void Update() {
        // If we finished rendering, clear the finish flag.
        if (done) {
            return;
        }

        // Update internal timer, recalculate visible chars, and update UI.Text with partially
        // transparent rich-text as appropriate.
        timeElapsed += Time.deltaTime;
        UpdateText();
    }

    void UpdateText() {
        int numCharsToDisplay = (int)(timeElapsed / charDisplaySpeed);
        if (numCharsToDisplay >= renderText.Length || done) {
            SetUITextContents(renderText);
            done = true;
        } else {
            SetUITextContents(
                renderText.Substring(0, numCharsToDisplay) +
                "<color=#0000>" +
                renderText.Substring(numCharsToDisplay, renderText.Length - numCharsToDisplay) +
                "</color>"
            );
        }
    }

	public IEnumerator WaitForTextAppear() {
		while (!IsFinished()) {
			yield return new WaitForSeconds(charDisplaySpeed);
		}
	}

    void SetUITextContents(string contents) {
        if (uiText != null) {
            uiText.text = contents;
        }
        if (tmpText != null) {
            tmpText.text = contents;
        }
    }
}

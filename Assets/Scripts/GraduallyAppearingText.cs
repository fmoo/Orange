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

    /// <summary>Current text to render. DO NOT USE this directly.  Call SetRenderText() instead.</summary>
    public string renderText = "";

    /// <sumamry>The number of seconds per character displayed.</summary>
    public float charDisplaySpeed = 0.01f;

    /// <sumamry>Current reference to a uiTextComponent.  If not set, one will be checked on this component's gameObject.</summary>
    public Text uiText;
    /// <sumamry>Current reference to a TMP Text component.  If not set, one will be checked on this component's gameObject.</summary>
    public TMP_Text tmpText;

    /// <summary>Set new text for rendering, restarting any in-progress progressive rendering.</summary>
    public void SetRenderText(string text) {
        renderText = text;
        Reset();
        SetUITextContents("");
    }

    /// <summary>Returns true if the current message has been fully displayed</summary>
    public bool IsFinished() {
        return done;
    }

    /// <summary>Complete gradual rendering instantly.</summary>
    public void SkipToEnd() {
        done = true;
        UpdateText();
    }

    /// <summary>For use in Coroutines.  Returns once all text has appeared</summary>
    public IEnumerator WaitForTextAppear() {
        while (!IsFinished()) {
            yield return new WaitForSeconds(charDisplaySpeed);
        }
    }

    /// <summary>Reset the current displaying text back to the beginning</summary>
    public void Reset() {
        done = false;
        timeElapsed = 0.0f;
    }

    void OnValidate() {
        if (uiText == null)
            uiText = GetComponent<Text>();
        if (uiText != null)
            uiText.supportRichText = true;
        if (tmpText == null)
            tmpText = GetComponent<TMP_Text>();
    }

    void Start() {
        OnValidate();
        if (uiText != null) {
            if (renderText == "") {
                renderText = uiText.text;
            }
        } else if (tmpText != null) {
            if (renderText == "") {
                renderText = tmpText.text;
            }
        }
    }

    void Update() {
        if (done) return;

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

    void SetUITextContents(string contents) {
        if (uiText != null) {
            uiText.text = contents;
        }
        if (tmpText != null) {
            tmpText.text = contents;
        }
    }
}

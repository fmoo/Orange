using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/**
 * Attach this behavior to an object with a UIText to cause the text to
 * gradually appear instead of appearing all at once.
 */
public class GraduallyAppearingText : FinishableBehavior {
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

	/**
	 * Helper for retrieving the UI.Text component associated with this component
	 */
	private Text uiText {
		// TODO: Get or Create component?
		get { return GetComponent<Text>(); }
	}

	/**
	 * Helper to set new text for rendering.  Resets the internal state.
	 */
	public void SetRenderText(string text) {
		renderText = text;
		Reset();
	}

	/**
	 * Reset the current state
	 */
	public void Reset() {
		done = false;
		timeElapsed = 0.0f;
	}

	override public bool IsFinished() {
		return done;
	}

	/**
	 * Helper function for handling button presses from external sources
	 */
	public void SkipToEnd() {
		done = true;
		UpdateText();
	}

	/**
	 * Initialize the UIText settings (enable rich-text) and local render text.
	 */
	void Start () {
		uiText.supportRichText = true;
		if (renderText == "") {
			renderText = uiText.text;
		}
	}
	
	// Update is called once per frame
	void Update () {
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
			uiText.text = renderText;
			done = true;
		} else {
			uiText.text = (
				renderText.Substring(0, numCharsToDisplay) +
				"<color=#00000000>" + 
				renderText.Substring(numCharsToDisplay, renderText.Length - numCharsToDisplay) +
				"</color>"
			);
		}
	}

}

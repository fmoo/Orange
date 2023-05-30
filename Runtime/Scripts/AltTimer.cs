using UnityEngine;

public class AltTimer {
	
	public float timeScale = 1.0f;

	public float deltaTime {
		get {
			return Time.unscaledDeltaTime * timeScale;
		}
	}
}
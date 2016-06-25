using UnityEngine;
using UnityEngine.UI;

public static class ColorUtils {
	public static Color Color(byte r, byte g, byte b, byte a) {
		return new Color(
			r / 255.0f,
			g / 255.0f,
			b / 255.0f,
			a / 255.0f
		);
	}

	public static Color Color(byte r, byte g, byte b) {
		return Color(r, g, b, 255);
	}
}

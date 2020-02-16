using UnityEngine;
using UnityEngine.UI;

public static class ColorUtils {
	public static Color WithAlpha(Color source, float a) {
		return new Color(source.r, source.g, source.b, a);
	}

	public static Color FromBytes(byte r, byte g, byte b, byte a) {
		return new Color(
			r / 255.0f,
			g / 255.0f,
			b / 255.0f,
			a / 255.0f
		);
	}

	public static Color FromBytes(byte r, byte g, byte b) {
		return FromBytes(r, g, b, 255);
	}
}

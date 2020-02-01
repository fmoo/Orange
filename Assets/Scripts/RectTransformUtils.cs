using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public static class RectTransformUtils {
    
	public static void ExpandToFillParent(this RectTransform rt) {
		rt.anchorMin = Vector2.zero;
		rt.anchorMax = Vector2.one;
		rt.sizeDelta = Vector2.one;
	}

	public class PivotPoints {
		public Vector2 UpperLeft;
		public Vector2 TopCenter;
		public Vector2 UpperRight;
		public Vector2 MiddleLeft;
		public Vector2 CenterMiddle;
		public Vector2 MiddleRight;
		public Vector2 LowerLeft;
		public Vector2 CenterBottom;
		public Vector2 LowerRight;
	}

	public static readonly PivotPoints Pivots = new PivotPoints() {
		UpperLeft = new Vector2(0.0f, 1.0f),
		TopCenter = new Vector2(0.5f, 1.0f),
		UpperRight = new Vector2(1.0f,1.0f),
		MiddleLeft = new Vector2(0.0f, 0.5f),
		CenterMiddle = new Vector2(0.5f, 0.5f),
		MiddleRight = new Vector2(1.0f, 0.5f),
		LowerLeft = new Vector2(0.0f, 0.0f),
		CenterBottom = new Vector2(0.5f, 0.0f),
		LowerRight = new Vector2(1.0f, 0.0f),
	};

	public static (Vector3 LowerLeft, Vector3 UpperLeft, Vector3 UpperRight, Vector3 LowerRight) GetLocalCornersStruct(this RectTransform rt) {
		Vector3[] fourCornersArray = new Vector3[4];
		rt.GetLocalCorners(fourCornersArray);

		return (
            LowerLeft: fourCornersArray[0],
			UpperLeft: fourCornersArray[1],
			UpperRight: fourCornersArray[2],
			LowerRight: fourCornersArray[3]
		);
	}

	public static Vector3 CenterMiddle(this RectTransform rt) {
		var cs = rt.GetLocalCornersStruct();
		return (cs.UpperLeft + cs.LowerRight) / 2f;
	}

	public static Vector3 CenterBottom(this RectTransform rt) {
		var cs = rt.GetLocalCornersStruct();
		return (cs.LowerLeft + cs.LowerRight) / 2f;
	}

	public static Vector3 CenterTop(this RectTransform rt) {
		var cs = rt.GetLocalCornersStruct();
		return (cs.UpperLeft + cs.UpperRight) / 2f;
	}

	public static Vector3 CenterMiddle(this RectTransform rt) {
		var cs = rt.GetLocalCornersStruct();
		return Vector3.Lerp(cs.UpperLeft, cs.LowerRight, 0.5f);
	}

	public static Vector3 CenterBottom(this RectTransform rt) {
		var cs = rt.GetLocalCornersStruct();
		return Vector3.Lerp(cs.LowerLeft, cs.LowerRight, 0.5f);
	}

	public static Vector3 CenterTop(this RectTransform rt) {
		var cs = rt.GetLocalCornersStruct();
		return Vector3.Lerp(cs.UpperLeft, cs.UpperRight, 0.5f);
	}

	public static Vector3 MiddleLeft(this RectTransform rt) {
		var cs = rt.GetLocalCornersStruct();
		return (cs.UpperLeft + cs.LowerLeft) / 2f;
	}

	public static Vector3 MiddleRight(this RectTransform rt) {
		var cs = rt.GetLocalCornersStruct();
		return (cs.UpperRight + cs.LowerRight) / 2f;
	}
}

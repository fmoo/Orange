using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public static class RectTransformUtils {
    
	public static void ExpandToFillParent(this RectTransform rt) {
		rt.anchorMin = Vector2.zero;
		rt.anchorMax = Vector2.one;
		rt.sizeDelta = Vector2.one;
	}

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

	public static Vector3 MiddleLeft(this RectTransform rt) {
		var cs = rt.GetLocalCornersStruct();
		return (cs.UpperLeft + cs.LowerLeft) / 2f;
	}

	public static Vector3 MiddleRight(this RectTransform rt) {
		var cs = rt.GetLocalCornersStruct();
		return (cs.UpperRight + cs.LowerRight) / 2f;
	}
}

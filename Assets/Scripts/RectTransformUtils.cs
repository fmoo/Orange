using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public static class RectTransformUtils {
	public class RectCorners {
		public Vector3 UpperLeft;
		public Vector3 UpperRight;
		public Vector3 LowerLeft;
		public Vector3 LowerRight;
	}

	public static RectCorners GetLocalCornersStruct(this RectTransform rt) {
		Vector3[] fourCornersArray = new Vector3[4];
		rt.GetLocalCorners(fourCornersArray);

		return new RectCorners() {
			LowerLeft = fourCornersArray[0],
			UpperLeft = fourCornersArray[1],
			UpperRight = fourCornersArray[2],
			LowerRight = fourCornersArray[3],
		};
	}

	public static Vector3 MiddleLeft(this RectTransform rt) {
		var cs = rt.GetLocalCornersStruct();
		return Vector3.Lerp(cs.UpperLeft, cs.LowerLeft, 0.5f);
	}

	public static Vector3 MiddleRight(this RectTransform rt) {
		var cs = rt.GetLocalCornersStruct();
		return Vector3.Lerp(cs.UpperRight, cs.LowerRight, 0.5f);
	}
}

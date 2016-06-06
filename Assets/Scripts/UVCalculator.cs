using UnityEngine;
using System.Collections;

public class UVCalculator
{
	private int rows, cols;
	public UVCalculator (int rows, int cols) {
		this.rows = rows;
		this.cols = cols;
	}

	public Vector2 _calcPoint(int row, int col) {
		return new Vector2(
			(float)col / cols,
			(float)row / rows
		);
	}

	public Vector2 UL(int row, int col) {
		return _calcPoint(row, col);
	}
	public Vector2 UR(int row, int col) {
		return _calcPoint(row, col+1);
	}
	public Vector2 BL(int row, int col) {
		return _calcPoint(row+1, col);
	}
	public Vector2 BR(int row, int col) {
		return _calcPoint(row+1, col+1);
	}
}
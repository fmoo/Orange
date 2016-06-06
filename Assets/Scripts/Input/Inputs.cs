using UnityEngine;
using System.Collections;

public abstract class InputBase {
	public readonly string Name;

	internal InputBase(string name) {
		Name = name;
	}
}

public sealed class InputAxis : InputBase {
	public static InputAxis HORIZONTAL = new InputAxis("Horizontal");
	public static InputAxis VERTICAL = new InputAxis("Vertical");

	public InputAxis(string name) : base(name) { }

	public float Get {
		get { return Input.GetAxis(Name); }
	}
	public float Raw {
		get { return Input.GetAxisRaw(Name); }
	}
}

public sealed class InputButton : InputBase {
	public static InputButton FIRE1 = new InputButton("Fire1");
	public static InputButton FIRE2 = new InputButton("Fire2");
	public static InputButton FIRE3 = new InputButton("Fire3");
	public static InputButton JUMP = new InputButton("Jump");
	public static InputButton SUBMIT = new InputButton("Submit");
	public static InputButton CANCEL = new InputButton("Cancel");

	public InputButton(string name) : base(name) { }

	public bool Pressed {
		get { return Input.GetButton(Name); }
	}
	public IEnumerable GenPressed() {
		while (true) {
			if (Pressed) {
				yield return null;
			} else {
				break;
			}
		}

		while (true) {
			if (Pressed) {
				yield break;
			} else {
				yield return null;
			}
		}
	}
	public bool Down {
		get { return Input.GetButtonDown(Name); }
	}
	public IEnumerator GenDown() {
		while (true) {
			if (Down || Pressed) {
				yield return null;
			} else {
				break;
			}
		}

		while (true) {
			if (Down || Pressed) {
				yield break;
			} else {
				yield return null;
			}
		}

	}

	public bool Up {
		get { return Input.GetButtonUp(Name); }
	}
	public IEnumerable GenUp() {
		while (true) {
			if (Up || !Pressed) {
				yield return null;
			} else {
				break;
			}
		}
		while (true) {
			if (Up || !Pressed) {
				yield break;
			} else {
				yield return null;
			}
		}
	}
}
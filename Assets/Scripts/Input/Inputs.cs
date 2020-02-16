using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class InputBase {
    public readonly string Name;

    internal InputBase(string name) {
        Name = name;
    }
}

public sealed class InputAxis : InputBase {
    public static InputAxis HORIZONTAL = new InputAxis("Horizontal");
    public static InputAxis VERTICAL = new InputAxis("Vertical");

    public static Vector2 GetVector2() {
        return new Vector2(InputAxis.HORIZONTAL.Smoothed, InputAxis.VERTICAL.Smoothed);
    }
    public static Vector2 GetVector2Raw() {
        return new Vector2(InputAxis.HORIZONTAL.Raw, InputAxis.VERTICAL.Raw);
    }

    public InputAxis(string name) : base(name) { }

    public float Smoothed {
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
    public IEnumerator GenPressed() {
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
    public IEnumerator GenUp() {
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


public sealed class InputKey : InputBase {
    public static Dictionary<KeyCode, InputKey> KEYS = new Dictionary<KeyCode, InputKey>();

    public static InputKey For(KeyCode key) {
        KEYS.TryGetValue(key, out InputKey value);
        if (value == null) {
            value = new InputKey(key);
            KEYS[key] = value;
        }
        return value;
    }

    public bool Down {
        get {
            return Input.GetKeyDown(KeyCode);
        }
    }
    public bool Up {
        get {
            return Input.GetKeyUp(KeyCode);
        }
    }
    public bool Pressed {
        get {
            return Input.GetKey(KeyCode);
        }
    }

    private KeyCode KeyCode;
    public InputKey(KeyCode keyCode) : base(keyCode.ToString()) { KeyCode = keyCode; }
}
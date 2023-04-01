
[System.Serializable]
public struct Vector2Bool {
    public bool x;
    public bool y;

    public static Vector2Bool TRUE = new Vector2Bool() { x = true, y = true };
    public static Vector2Bool FALSE = new Vector2Bool() { x = false, y = false };
    public static Vector2Bool X = new Vector2Bool() { x = true };
    public static Vector2Bool Y = new Vector2Bool() { y = true };
}


[System.Serializable]
public struct Vector3Bool {
    public bool x;
    public bool y;
    public bool z;

    public static Vector3Bool TRUE = new Vector3Bool() { x = true, y = true, z = true };
    public static Vector3Bool FALSE = new Vector3Bool() { x = false, y = false, z = false };
    public static Vector3Bool X = new Vector3Bool() { x = true };
    public static Vector3Bool Y = new Vector3Bool() { y = true };
    public static Vector3Bool Z = new Vector3Bool() { z = true };
}

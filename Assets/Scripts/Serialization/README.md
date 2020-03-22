# Orange/Serialization

This is a WIP, but I've verified the basic functionality works.

## Why?

When using JsonUtility to serialize a System.Serializable blob of Game State, unity
will serialize ScriptableObject references using a numeric localReferenceId, which is
not consistent between runs, and breaks serialization.  Normal GameObjects don't
suffer from this as its yaml/binary encoding is a bit smarter.

To solve this, we:
* Create a container/wrapper class for `ScriptableObject` called `ScriptableObjectReference` that implements Unity's `ISerializationCallbackReceiver` protocol to retrieve the asset using its GUID instead of the default unstable local identifier.
* Create a new base class alternative for `ScriptableObject`, called `SerializableScriptableObject` that burns the asset GUID into the serialized `.asset` file for the non-editor runtime to be able to access.
* Provide an field annotation, `[SORefType(typeof MyScriptableObject)]` to annotate your `ScriptableObjectReference` fields with, so that the editor restricts Inspector assigment to the proper types, and hides the unnecessary GUID/struct internals.

## How do I use it?

1. Make all Scriptable Objects that are referenced by your save state subclass `SerializableScriptableObject` instead of `ScriptableObject`
2. Make sure all Scriptable Object .asset files referenced by your save state exist under a Resources/ subfolder anywhere (can be in a parent)
3. Before loading your JSON from disk, call `ScriptableObjectReference.InitScriptableObjectCache()` once.
4. Instead of directly referencing your ScriptableObject type from your save state like this:
```cs
  public MyFoo foo;
```
do this:
```cs
  [SORefType(typeof(MyFoo))]
  [SerializeField] private ScriptableObjectReference fooRef; 
  public MyFoo foo {
    get {
      return this.fooRef.value as MyFoo;
    }
    set {
      this.fooRef = new ScriptableObjectReference(value);
    }
  }
```

That's it!

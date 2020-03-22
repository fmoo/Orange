# Orange/Serialization

This is a WIP, but I've verified the basic functionality works.

## Why?

When using JsonUtility to serialize a System.Serializable blob of Game State, unity
will serialize ScriptableObject references using a numeric localReferenceId, which is
not consistent between runs, and breaks serialization.  Normal GameObjects don't
suffer from this as its yaml/binary encoding is a bit smarter.

# How do I use it?

1. Make all Scriptable Objects that are referenced by your save state subclass `SerializableScriptableObject` instead of `ScriptableObject`
2. Make sure all Scriptable Object .asset files referenced by your save state exist under a Resources/ subfolder anywhere (can be in a parent)
3. Before loading your JSON from disk, call ScriptableObjectReference.InitScriptableObjectCache() once.
4. Instead of directly referencing ScriptableObject from your save state like this:
```
  public MyFoo foo;
```
do this:
```
  [SORefType(typeof(MyFoo))]
  public ScriptableObjectReference fooRef; 
  // Optional but RECOMMENDED for type safety:
  public MyFoo foo {
    get {
      return this.fooRef.value as MyFoo;
    }
    set {
      this.fooRef = new ScriptableObjectReference(value);
    }
  }
```
5. If you did not include the typed property setter in step 4, any callsites setting `foo` will need to be updated to something like this:
```
  saveState.fooRef = new ScriptableObjectReference(myFooAsset);
```

That's it!

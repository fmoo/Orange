# Orange/Serialization

## Why?

When using `JsonUtility` to serialize a `System.Serializable` blob of game State, Unity
serializes `ScriptableObject` using a numeric local reference ID, which is not consistent
between runs, and therefore breaks serialization beteween sessions.  Normal GameObjects when
using the editor Asset Database don't suffer from this as its yaml/binary encoding uses the asset
guid to reference foreign objects, which is stable.

To solve this, we:
* Create a container/wrapper class for `ScriptableObject` called `ScriptableObjectReference` that implements Unity's `ISerializationCallbackReceiver` protocol to retrieve the asset using its GUID instead of the default unstable local identifier.
* Create a new base class alternative for `ScriptableObject`, called `SerializableScriptableObject` that burns the asset GUID into the serialized `.asset` file for the non-editor runtime to be able to access.
* Provide an field annotation, `[SORefType(typeof MyScriptableObject)]` to annotate your `ScriptableObjectReference` fields with, so that the editor restricts Inspector assigment to the proper types, and hides the unnecessary GUID/struct internals.
* Include an explicit API, `ScriptableObjectReference.InitScriptableObjectCache()` to prepare the ScriptableObjectReference GUID lookup using all your `SerializableScriptableObject` .asset instances that live in `Resources` folders.

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

## Ew, Why's it so Ugly?!

Unfortunately, Unity 2019 and older do not support either serialization of or property inspection of Generics other than `List<T>`.
Once Unity adds support for this, we should be able to do something like this instead for step 4:
```cs
  [SerializeField] private ScriptableObjectReference<MyFoo> foo;
```
Though this would also require updating all callers to use `foo.value` instead of `foo` directly as well.

## Alternatives

- [OdinSerializer](https://github.com/TeamSirenix/odin-serializer) - Really powerful serializer.  Does **deep** serialization of your game state heirarchy, which is fast, but may result in excessively large or unexpectedly duplicate/aliased data.  May be customizable to support this feature, but it was not obvious from the docs, and the compiler warnings on 2019.3 annoyed me.
- **Manual type mappings** - With this approach, you either don't include ScriptableObjects in your save state blob, or you complement them with an `enum` type that you can use to reconstruct the references with manual logic after load.  This approach requires you to have extra logic to recreate your object references, so adding a new one means updating 2-3 different locations, which is a lot of overhead that ScriptableObjects are supposed to be saving you.

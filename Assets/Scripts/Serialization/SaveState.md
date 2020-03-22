# Managing Save Games in Unity with ScriptableObject

In general, it is *Good Practice* to separate your saveable "game state" from the components and game objects in your Unity scene graph.

On the other hand, it is *A Pain In The Ass* to regenerate GameObjects and Components from data structures managed outside of the scene graph.

ScriptableObject provides a middle ground as convenient way to centralize, deduplicate, and isolate your game state from your scene graph, and encourages patterns that minimize creation of extra GameObjects/Components for tracking data.

I've documented this technique below, providing a ton of context on Serialization and State in general in Unity along the way.

Hope you enjoy this doc!

## Can't I just put my Save State in a ScriptableObject and be done?

While this works from inside the editor in play mode, modifications to ScriptableObject .asset files **are not persisted**
when running a standalone builds of your application.  All your game progress will get lost every time you restart the app.

## What could I do?

In general, you should be only have **read-only data** in your ScriptableObjects.  Things like base stats for your
characters, weapons, or items, sprite/audio name indexes and mappings, etc.

For mutable data that you'd like to persist, use a `public class` or `public struct` with the `[System.Serializable]`
annotation.  If you want your save state to directly reference a ScriptableObject, consult [this doc](https://github.com/fmoo/Orange/tree/master/Assets/Scripts/Serialization)
for more details on the challenges here.

## But this article is titled "Managing Save Games in Unity with ScriptableObject"!

I'll get to that in a bit.  Want to cover some fundamentals first.

## JsonUtility

`JsonUtility` is [Unity's built-in JSON encoder](https://docs.unity3d.com/ScriptReference/JsonUtility.html).  JSON is an
extremely common way of converting objects to strings, which you can easily save and load from users' devices.

## System.Serializable

`[System.Serializable]` is an annotation you can put on a (non-MonoBehaviour/ScriptableObject) `public class` or `public struct`
that tells other C# programs that its instances should be convertable to strings.  The Unity property inspector also supports
expanding and editing objects of this type in the property inspector.  For example, if you define a class:
```
[System.Serializable]
public class SaveState {
[System.Serializable]
public class SGSaveState {
    public bool isValid = false;
    public float lastLoadTime = 0;
    public int money = 1000;
    public bool[] flags;
    public int[] variables;
}
```

Any references to this class in a `MonoBehavior` or `ScriptableObject` will enumerate these fields in their property inspector.

## `public` vs. `[SerializeField] private`

This is just a random preference thing, but nice to comment on.

- Prefixing your fields with `public` allows programmatic access to modify this field, which may be undesirable for data consistency and encapsulation reasons
- Prefixing with `[SerializeField] private` makes it so that this field can ONLY be set by the Unity editor.  This has an unfortunate side effect of causing warning spew on every build.

## Referencing (deduplicating) data

While `[System.Serializable]` is super useful in a lot of ways, it can be a nightmare to copy all these fields around.

This is where `ScriptableObject` is super useful: You can put the shared readonly data on a ScriptableObject,
and make all your mutable things refer to it.

Consider the following:
```
public class ArmorBasis : ScriptableObject {
  int defense;
  int maxDurability;
}

[System.Serializable] public class ArmorEntry {
  float condition;
  ArmorBasis basis;
}

[System.Serializable] public class Inventory {
  ArmorEntry[] armor;
}
```

In this, your `Inventory` contains a collection of different `Armor`s, each with its own current condition (e.g., 0.0 - 1.0), and
a reference to their base stats, `basis`.  With this approach, if you want to change an armor's stats, you only need to
update its ScriptableObject `.asset` file in one place, and all your saves and characters get updated immediately.

As I mentioned before, if you want your save state to directly reference a ScriptableObject, **consult [this doc](https://github.com/fmoo/Orange/tree/master/Assets/Scripts/Serialization)
for more details on the challenges here**.

## What about using a ScriptableObject for the save itself already?

Back to the beginning: `ScriptableObject` is **great** for deduplicating references to shared data.  We can take advantage of
the editability of `[Serializable]` and they are trivially assignable on your components that need a reference to your
global game state.  Here's the concrete example:
```
TBD
```

## Saving to Disk

TBD

## Anti-tamper
JSON files are trivially tamperable.  To make them more difficult to tamper with, either include a fixed length
plaintext header with an HMAC signature of the save file.

# How To Save Game State in Unity

When talking about saving your app, there are three main parts to the problem:

1. Organizing your game data
2. Encoding and decoding the data
3. Saving and Restoring the encoded files

In general this means:

1. Create shared instances of `public class`es with the `[System.Serializable]` annotation
2. Convert those shared instances to and from JSON with `JsonUtility`
3. Write them to a file on the device.

## 1. Organizing Your Game Data

### Using `[System.Serializable]` on Structures

`[System.Serializable]` is an annotation you can put on a (**non-MonoBehaviour/ScriptableObject**) `public class` or `public struct`
that tells other C# code that its instances and fields are "encodable".  This includes telling the Unity property inspector that it can
visualize and enable editing of objects of these type.  For example, if you define a class:
```
[System.Serializable]
public class SaveState {
    public bool isValid = false;
    public float lastLoadTime = 0;
    public int money = 1000;
    public bool[] flags;
    public int[] variables;
}
```

Any references to this class in a `MonoBehavior` or `ScriptableObject` will enumerate these fields in their property inspector.

TBD: Screenshot

#### `public` vs. `[SerializeField] private`

When using `[System.Serializable]` there are two ways to mark a field for serialization: marking the field as `public` or annotating a private or protected field with `[SerializeField]`

- Prefixing your fields with `public` allows programmatic access to modify this field, which may be undesirable for data consistency and encapsulation reasons
- Prefixing with `[SerializeField] private` makes it so that this field can ONLY be set by the Unity editor.  This has an unfortunate side effect of causing warning spew on every build.


### Deduplicating Read-Only Data with ScriptableObject

While `[System.Serializable]` is super useful in a lot of ways, it can be a nightmare to copy all these fields around.

`ScriptableObject` can be a really useful tool: it provides a way to centralize, deduplicate, and isolate your game state from your scene graph, and encourages patterns that minimize creation of extra GameObjects/Components for tracking data.

You can put any shared, readonly data on a `ScriptableObject`, and make all your mutable things refer to it.

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
With this, you can create `.asset` files for all of your different Armors (e.g., Iron Armor, Cloth Cloak, etc) base stats.
Your game state will maintain its `Inventory` as a collection of unique instances, each with theor own condition (from 0.0 - 1.0), and
a reference to its base stats, `basis`.  With this approach, if you want to change an armor's stats, you only need to
update the `.asset` file once place, and all your references get updated immediately.

There are some gotchas here, so, **consult [this doc](https://github.com/fmoo/Orange/tree/master/Assets/Scripts/Serialization)
for more details on the challenges here**.


## 2. Encoding and decoding the data

### JsonUtility

`JsonUtility` is [Unity's built-in JSON encoder](https://docs.unity3d.com/ScriptReference/JsonUtility.html).  JSON is an
extremely common way of converting objects to strings, which you can easily save and load from users' devices.

If your Game State references ScriptableObject instances for deduplication, you will have problems!  See ... for details, or consider using Odin's [open source serializer]().

### OdinSerializer

TBD

## 3. Saving and restoring the encoded files

### Using a ScriptableObject

**You can't do this directly!**  While this works from inside the editor in play mode, modifications to ScriptableObject .asset files **are not persisted** when running a standalone builds of your application.  All your game progress will get lost every time you restart the app.

To do this, you must include to sync the ScriptableObject to and from a file on disk, such as using the mechanism described below.

Doing this with json might look something like this:
```
TBD
```

### Using the Filesystem
A straightforward way to do this is:
```
var writer = new System.IO.StreamWriter(Application.persistentDataPath + "/saves.json");
writer.Write(data);
writer.Flush();
writer.Close();
```

Using `Applicaiton.persistentDataPath` is critical because it uses a permission-appropriate location, and adheres to
OS-specific backup/restore policies.

### Using [PlayerPrefs](https://docs.unity3d.com/ScriptReference/PlayerPrefs.html)?

PlayerPrefs is a simple Key/Value store that lets you store strings in the Windows Registry.  While it is meant to be
used for settings like language, volume, or input overrides, you can also jam your serialized JSON in here.

You should write this as a file on disk instead.

### Anti-tamper
JSON files are trivially tamperable.  To make them more difficult to tamper with, either include a fixed length
plaintext header with an HMAC signature of the save file.

TBD: concrete examples

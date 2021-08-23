using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "SpritesDB", menuName = "Data/Sprite DB", order = 3)]
public class OrangeSpriteDB : ScriptableObject {
    // [NaughtyAttributes.ReorderableList]
    public List<OrangeSpriteManagerSprite> sprites = new List<OrangeSpriteManagerSprite>();
    // [NaughtyAttributes.ReorderableList]
    public List<OrangeSpriteManagerAnimation> animations = new List<OrangeSpriteManagerAnimation>();
    private Dictionary<string, OrangeSpriteManagerSprite> namedSprites =
        new Dictionary<string, OrangeSpriteManagerSprite>();
    private Dictionary<string, OrangeSpriteManagerAnimation> namedAnimations =
        new Dictionary<string, OrangeSpriteManagerAnimation>();

    public OrangeSpriteManagerSprite GetSprite(string name) {
        if (namedSprites.Count == 0) BuildIndex();
        if (namedSprites.TryGetValue(name, out OrangeSpriteManagerSprite result)) {
            return result;
        }
        return null;
    }
    public OrangeSpriteManagerAnimation GetAnimation(string name) {
        if (namedAnimations.Count == 0) BuildIndex();
        if (namedAnimations.TryGetValue(name, out OrangeSpriteManagerAnimation result)) {
            return result;
        }
        return null;
    }

    private void OnValidate() {
        BuildIndex();
    }
    private void Start() {
        BuildIndex();
    }
    private void BuildIndex() {
        foreach (var s in sprites) {
            if (s.name == "" || s.sprite == null) continue;
            namedSprites[s.name] = s;
        }
        foreach (var a in animations) {
            a.initFrames(this);
            namedAnimations[a.name] = a;
        }
    }

#if UNITY_EDITOR
    public static NaughtyAttributes.DropdownList<string> GetEditorSpriteDropdown(OrangeSpriteDB db) {
        var result = new NaughtyAttributes.DropdownList<string>();
        if (db == null) return null;
        var names = db.sprites.Select(s => s.name).ToList();
        names.Sort();
        foreach (var name in names) {
            result.Add(name, name);
        }
        return result;
    }
#endif

    public void RemovePrefix(string prefix) {
        sprites = sprites.Where(s => !s.name.StartsWith($"{prefix}_")).ToList();
        animations = animations.Where(s => !s.name.Equals(prefix)).ToList();
    }

    [BoxGroup("Sprite Import Configuration")]
    public Sprite[] importSprites;
    [BoxGroup("Sprite Import Configuration")]
    public string importAnimName;
    [BoxGroup("Sprite Import Configuration")]
    public bool renameImportedSprites = true;
    [BoxGroup("Sprite Import Configuration")]
    public bool flipImportedSprites = false;
    [BoxGroup("Sprite Import Configuration")]
    public bool loopImportedAnimation = true;
    [BoxGroup("Sprite Import Configuration")]
    public float importTimePerFrame = 0.1f;
    [Button("Import Sprites")]
    public void DoImportSprites() {
        int ii = 0;
        var addedList = new List<string>();
        foreach (var sprite in importSprites) {
            var name = !renameImportedSprites ? sprite.name : $"{importAnimName}{ii}";
            addedList.Add(name);
            sprites.Add(new OrangeSpriteManagerSprite() {
                sprite = sprite,
                name = name,
                flip = flipImportedSprites,
            });
            ii += 1;
        }
        if (importAnimName != "") {
            string config = string.Join(",", addedList);
            animations.Add(new OrangeSpriteManagerAnimation() {
                name = importAnimName,
                config = config,
                duration = importTimePerFrame * importSprites.Count(),
                loop = loopImportedAnimation
            });
        }
        importSprites = new Sprite[0];
        importAnimName = "";
        renameImportedSprites = true;
        flipImportedSprites = false;
    }
    [Button("CLEAR ALL")]
    public void Clear() {
        sprites.Clear();
        animations.Clear();
        importSprites = new Sprite[] {};
    }

}
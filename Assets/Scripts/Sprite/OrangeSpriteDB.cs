using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "SpritesDB", menuName = "Data/Sprite DB", order = 3)]
public class OrangeSpriteDB : ScriptableObject {
    public List<OrangeSpriteManagerSprite> sprites = new List<OrangeSpriteManagerSprite>();
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


    [NaughtyAttributes.BoxGroup("Sprite Import Configuration")]
    public Sprite[] importSprites;
    [NaughtyAttributes.BoxGroup("Sprite Import Configuration")]
    public string importAnimName;
    [NaughtyAttributes.BoxGroup("Sprite Import Configuration")]
    public bool renameImportedSprites = true;
    [NaughtyAttributes.Button("Import Sprites")]
    public void DoImportSprites() {
        int ii = 0;
        var addedList = new List<string>();
        foreach (var sprite in importSprites) {
            var name = !renameImportedSprites ? sprite.name : $"{importAnimName}{ii}";
            addedList.Add(name);
            sprites.Add(new OrangeSpriteManagerSprite() {
                sprite = sprite,
                name = name,
            });
            ii += 1;
        }
        if (importAnimName != "") {
            string config = string.Join(",", addedList);
            animations.Add(new OrangeSpriteManagerAnimation() {
                name = importAnimName,
                config = config,
            });
        }
        importSprites = new Sprite[0];
        importAnimName = "";
        renameImportedSprites = true;
    }
}
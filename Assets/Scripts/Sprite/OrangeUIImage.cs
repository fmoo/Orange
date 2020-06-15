using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class OrangeUIImage : MonoBehaviour {
    public OrangeSpriteDB sprites;
    public UnityEngine.UI.Image spriteImage;

    [NaughtyAttributes.Dropdown("GetEditorDropdown")]
    [SerializeField] private string spriteName;

    public void SetSpriteName(string spriteName) {
        this.spriteName = spriteName;
        OnValidate();
    }

    protected void OnValidate() {
        if (spriteImage == null) spriteImage = GetComponent<UnityEngine.UI.Image>();
        if (spriteImage != null && sprites != null && spriteName != "") {
            var sprite = sprites.GetSprite(spriteName);
            if (sprite != null) sprite.SetUIImageSprite(spriteImage);
        }
    }

    protected void Start() {
        OnValidate();
    }

#if EDITOR
    public NaughtyAttributes.DropdownList<string> GetEditorDropdown() {
        return OrangeSpriteDB.GetEditorSpriteDropdown(sprites);
    }
#endif
}

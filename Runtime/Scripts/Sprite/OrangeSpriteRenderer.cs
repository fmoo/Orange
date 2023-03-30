using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class OrangeSpriteRenderer : MonoBehaviour {
    public OrangeSpriteDB sprites;
    public SpriteRenderer spriteRender;

    public string spriteName;

    void OnValidate() {
        if (spriteRender == null) spriteRender = GetComponent<SpriteRenderer>();
        Flip();
    }

    public void Flip() {
        if (spriteRender != null && sprites != null && spriteName != "") {
            var sprite = sprites.GetSprite(spriteName);
            if (sprite != null) sprite.SetRendererSprite(spriteRender);
        }
    }

    void Start() {
        OnValidate();
    }
}
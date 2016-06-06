using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Do you need to dynamically load sprites from a spritesheet?  Use this!  Sprites and
 * SpriteCollections are memoized to minimize unnecessary memory allocations.
 * 
 *   Sprite s = SpriteCollection.Cached("tilesheet1").GetSprite("brick_27");
 */
public class SpriteCollection
{
	private static Dictionary<string, SpriteCollection> instances = new Dictionary<string, SpriteCollection>();

	private Dictionary<string, Sprite> sprites;

	public static SpriteCollection Cached(string name) {
		SpriteCollection result;
		if (instances.TryGetValue (name, out result)) {
			return result;
		}

		result = new SpriteCollection (name);
		instances [name] = result;
		return result;
	}

	private SpriteCollection(string directory) {
		this.sprites = new Dictionary<string, Sprite> ();

		var loaded = Resources.LoadAll<Sprite> (directory);
		//Debug.Log ("Load<" + directory + "> loaded " + loaded.Length + " sprites");
		if (loaded == null || loaded.Length == 0) {
			throw new UnityException("Error loading assets from directory, " + directory);
		}
		foreach (var sprite in loaded) {
			this.sprites[sprite.name] = sprite;
		}
	}
	
	public Sprite GetSprite(string name)
	{
		return sprites[name];
	}

	public Sprite GetSpriteOffs(string name, int offset) {
		return sprites[name + '_' + offset];
	}
}

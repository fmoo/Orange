using UnityEngine;
using System.Collections;

public class GameBehavior : MonoBehaviour {

	protected T GetOrCreateComponent<T>() where T : Component  {
		return ComponentExtensions.GetOrCreateComponent<T> (this);
	}

	protected GameObject GetOrCreateGameObject(string name) {
		return ComponentExtensions.GetOrCreateGameObject (this, name);
	}

	protected bool IsInEditorMode() {
		return Application.isEditor && !Application.isPlaying;
	}
}
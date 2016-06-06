using UnityEngine;
using System.Collections;

public class GameBehavior : MonoBehaviour {

	protected T GetOrCreateComponent<T>() where T : Component  {
		return ComponentUtils.GetOrCreateComponent<T> (this);
	}

	protected GameObject GetOrCreateGameObject(string name) {
		return ComponentUtils.GetOrCreateGameObject (this, name);
	}

	protected bool IsInEditorMode() {
		return Application.isEditor && !Application.isPlaying;
	}
}
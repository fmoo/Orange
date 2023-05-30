using UnityEngine;
using System.Collections;

abstract public class MapInteractable : MonoBehaviour {
	abstract public void Interact(InteractionFinished finish);
}

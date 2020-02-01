using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public delegate void InteractionFinished();

public class MapInteractor : MonoBehaviour {
	public List<MonoBehaviour> DisableBehaviors = new List<MonoBehaviour>();

	private FindNearestCollider _Finder;
	private bool _SkipUpdate = false;

	void Start () {
		_Finder = GetComponent<FindNearestCollider> ();
		if (_Finder == null) {
			throw new UnityException("You cannot Interact with objects without a FindNearestCollider behavior");
		}
	}
	
	void Update () {
		if (_SkipUpdate) {
			_SkipUpdate = false;
			return;
		}

		if (Input.GetButtonDown ("Fire2")) {
			if (_Finder.Nearest == null) {
				return;
			}

			Debug.Log ("Interaction disabled");
			this.enabled = false;
			foreach (MonoBehaviour b in DisableBehaviors) {
				b.enabled = false;
			}

			_Finder.Nearest.GetComponent<MapInteractable>().Interact(
				_EnableInteraction
			);
		}
	}

	private void _EnableInteraction() {
		Debug.Log ("Interaction re-enabled!");
		_SkipUpdate = true;
		this.enabled = true;
		foreach (MonoBehaviour b in DisableBehaviors) {
			b.enabled = true;
		}
	}
}

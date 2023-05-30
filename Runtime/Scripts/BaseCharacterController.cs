using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
abstract public class BaseCharacterController : BaseObjectController {

	public bool isPlayerControlled = false;

	private MotionController motionController;

	abstract protected void InitMotionController (MotionController m); 

	override protected void _InitOtherComponents() {
	    base._InitOtherComponents();

		if (this.isPlayerControlled) {
			var pc = GetOrCreateComponent<PlayerControlled> ();
			pc.speed = 50;
			pc.minAxisThreshold = 0.05f;
			pc.maxSpeed = 0.5f;
			pc.maxRunSpeed = 2.0f;
		}
		motionController = this.GetOrCreateComponent<MotionController> ();
		InitMotionController (this.motionController);
	}
}

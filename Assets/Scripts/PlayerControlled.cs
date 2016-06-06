using UnityEngine;
using System.Collections;

public class PlayerControlled : MonoBehaviour {
	public float speed = 1.0f;
	public float minAxisThreshold = 0.5f;
	public float maxSpeed = 0.5f;
	public float maxRunSpeed = 1.0f;

	// Use this for initialization
	void Start () {
	
	}

	void FixedUpdate() {
		Rigidbody2D body = GetComponent<Rigidbody2D> ();
		bool isRunning = Input.GetButton ("Fire1");
		float effMaxSpeed = (isRunning ? maxRunSpeed : maxSpeed);

		float dx = Input.GetAxisRaw ("Horizontal");
		float dy = Input.GetAxisRaw ("Vertical");

		// If our axes are neutral, stop the player
		if (Mathf.Abs (dx) < minAxisThreshold && Mathf.Abs (dy) < minAxisThreshold) {
			body.velocity = new Vector2(0.0f, 0.0f);
			return;
		}

		body.AddForce(new Vector2(dx * speed, dy * speed));
		if (body.velocity.magnitude > effMaxSpeed) {
			body.velocity = Vector2.ClampMagnitude (body.velocity, effMaxSpeed);
		}
	}

	// Update is called once per frame
	void Update () {
	
	}
}

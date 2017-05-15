using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteLookDirection : MonoBehaviour {

	[SerializeField]
	private Rigidbody2D rigidBody;

	void FixedUpdate() {
		float angle = Mathf.Atan2(rigidBody.velocity.x, rigidBody.velocity.y) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis(-angle, Vector3.forward);
	}
}

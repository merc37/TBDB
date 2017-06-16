using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player {
	public class PlayerWalk : MonoBehaviour {

		[SerializeField]
	    private float walkSpeed;
        private Rigidbody2D rigidBody;

        void Start() {
            rigidBody = GetComponent<Rigidbody2D>();
        }

        // Use for physics updates
        void FixedUpdate() {
			rigidBody.AddForce(Input.GetAxis("VerticalMovement") * walkSpeed * transform.up);
			rigidBody.AddForce(Input.GetAxis("HorizontalMovement") * walkSpeed * transform.right);
	    }
	}
}
using UnityEngine;

namespace Player {
	public class PlayerWalk : MonoBehaviour {

		[SerializeField]
	    private float walkSpeed = 10;
        private new Rigidbody2D rigidbody;

        void Start() {
            rigidbody = GetComponent<Rigidbody2D>();
        }
        
        void FixedUpdate() {
            rigidbody.AddForce(Input.GetAxis("VerticalMovement") * walkSpeed * transform.up);
            rigidbody.AddForce(Input.GetAxis("HorizontalMovement") * walkSpeed * transform.right);
        }
	}
}
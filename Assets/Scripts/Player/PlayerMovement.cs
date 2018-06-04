using UnityEngine;

namespace Player {
	public class PlayerMovement : MonoBehaviour {

		[SerializeField]
	    private float maxWalkSpeed = 10;
        private new Rigidbody2D rigidbody;
        private float speed;

        void Start() {
            rigidbody = GetComponent<Rigidbody2D>();
            speed = maxWalkSpeed * rigidbody.drag;
        }
        
        void FixedUpdate() {
            float horizInput = Input.GetAxis("HorizontalMovement");
            float vertInput = Input.GetAxis("VerticalMovement");
            Vector2 direction = Vector2.zero;
            if(horizInput != 0) {
                direction += horizInput * Vector2.right;
            }
            if(vertInput != 0) {
                direction += vertInput * Vector2.up;
            }
            rigidbody.AddForce(direction.normalized * speed);

        }
	}
}
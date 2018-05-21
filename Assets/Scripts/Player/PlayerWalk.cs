using UnityEngine;

namespace Player {
	public class PlayerWalk : MonoBehaviour {

		[SerializeField]
	    private float maxWalkSpeed = 8;
        private new Rigidbody2D rigidbody;
        private float speed;

        void Start() {
            rigidbody = GetComponent<Rigidbody2D>();
            speed = maxWalkSpeed * rigidbody.drag;
        }
        
        void FixedUpdate() {
            float horizInput = Input.GetAxis("HorizontalMovement");
            float vertInput = Input.GetAxis("VerticalMovement");
            if(horizInput != 0) {
                rigidbody.AddForce(horizInput * speed * Vector2.right);
            }
            if(vertInput != 0) {
                rigidbody.AddForce(vertInput * speed * Vector2.up);
            }
            
        }
	}
}
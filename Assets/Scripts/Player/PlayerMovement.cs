using UnityEngine;

namespace Player {
	public class PlayerMovement : MonoBehaviour {

		[SerializeField]
	    private float maxSpeed = 13;
        [SerializeField]
        private float acceleration = 10;
        private new Rigidbody2D rigidbody;

        private float _speed;
        private float Speed {
            get { return _speed; }
            set {
                if(value > maxSpeed) {
                    _speed = maxSpeed;
                    return;
                }
                if(value < 0) {
                    _speed = 0;
                    return;
                }

                _speed = value;
            }
        }

        void Start() {
            rigidbody = GetComponent<Rigidbody2D>();
        }
        
        void FixedUpdate() {
            if(!GetComponent<PlayerRoll>().Rolling) {
                float horizInput = Input.GetAxis("HorizontalMovement");
                float vertInput = Input.GetAxis("VerticalMovement");
                Vector2 direction = Vector2.zero;
                if(horizInput != 0) {
                    direction += horizInput * Vector2.right;
                }
                if(vertInput != 0) {
                    direction += vertInput * Vector2.up;
                }
                if(direction != Vector2.zero) {
                    Speed += acceleration * Time.fixedDeltaTime;
                } else {
                    Speed -= acceleration * Time.fixedDeltaTime;
                }
                rigidbody.velocity = direction.normalized * Speed;
            }

        }
	}
}
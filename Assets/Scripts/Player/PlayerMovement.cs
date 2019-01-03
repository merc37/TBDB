using UnityEngine;

namespace Player
{
	public class PlayerMovement : MonoBehaviour
	{
		[SerializeField] private float maxSpeed = 13;
        [SerializeField] private float acceleration = 10;
	    
        private new Rigidbody2D rigidbody;
		private PlayerRoll playerRoll;

        private float _speed;
        private float Speed
        {
            get { return _speed; }
            set
            {
                _speed = Mathf.Clamp(value, 0, maxSpeed);
            }
        }

        void Start()
        {
            rigidbody = GetComponent<Rigidbody2D>();

	        playerRoll = GetComponent<PlayerRoll>();
        }
        
        void FixedUpdate()
        {
            //if(!playerRoll.Rolling)
            //{
                float horzInput = Input.GetAxisRaw("HorizontalMovement");
                float vertInput = Input.GetAxisRaw("VerticalMovement");
                Vector2 direction = new Vector2(horzInput, vertInput);
                
                if(direction != Vector2.zero)
                {
                    Speed += acceleration * Time.fixedDeltaTime;
                    //rigidbody.velocity = direction.normalized * Speed;

                } else {
                    Speed -= acceleration * Time.fixedDeltaTime;
                    //Speed = 0;
                    //rigidbody.velocity = Vector2.zero;

                }

                rigidbody.velocity = direction.normalized * Speed;
            //}
        }
	}
}
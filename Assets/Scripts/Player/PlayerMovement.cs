using EventManagers;
using UnityEngine;
using UnityEngine.Events;

namespace Player {
	public class PlayerMovement : MonoBehaviour {
        
        [SerializeField]
        private float maxRunSpeed = 30;
        [SerializeField]
        private float maxWalkSpeed = 10;
        [SerializeField]
        private float runAcceleration = 1;
        [SerializeField]
        private float walkAcceleration = .3f;

        private new Rigidbody2D rigidbody;
        private GameObjectEventManager eventManager;

        private float currentMaxSpeed;
        private float currentAcceleration;

        private Vector2 directionVector = Vector2.zero;
        private Vector2 newVelocity = Vector2.zero;

        private bool walking;
        private bool rolling;

        void Awake() {
            eventManager = GetComponent<GameObjectEventManager>();
            rigidbody = GetComponent<Rigidbody2D>();
            rolling = false;
            eventManager.StartListening("OnRollStart", new UnityAction<ParamsObject>(OnRollStart));
            eventManager.StartListening("OnRollEnd", new UnityAction<ParamsObject>(OnRollEnd));
        }

        void Start() {
            eventManager.TriggerEvent("SendMovementSpeed", new ParamsObject(maxRunSpeed));
        }

        void Update() {
            directionVector.Set(Input.GetAxisRaw("HorizontalMovement"), Input.GetAxisRaw("VerticalMovement"));
            walking = Input.GetButton("Walk");
        }
        
        void FixedUpdate() {
            currentMaxSpeed = walking ? maxWalkSpeed : maxRunSpeed;
            currentAcceleration = walking ? walkAcceleration : runAcceleration;
            if(!rolling) {
                newVelocity = rigidbody.velocity;
                if(directionVector.x != 0 && directionVector.x == -Mathf.Sign(newVelocity.x)) {
                    newVelocity.x = 0;
                }
                if(directionVector.y != 0 && directionVector.y == -Mathf.Sign(newVelocity.y)) {
                    newVelocity.y = 0;
                }
                if(directionVector.x == 0) {
                    if(Mathf.Abs(newVelocity.x) <= currentAcceleration) {
                        newVelocity.Set(0, newVelocity.y);
                    }
                    if(Mathf.Abs(newVelocity.x) > currentAcceleration) {
                        directionVector.Set(-Mathf.Sign(newVelocity.x), directionVector.y);
                    }
                }
                if(directionVector.y == 0) {
                    if(Mathf.Abs(newVelocity.y) <= currentAcceleration) {
                        newVelocity.Set(newVelocity.x, 0);
                    }
                    if(Mathf.Abs(newVelocity.y) > currentAcceleration) {
                        directionVector.Set(directionVector.x, -Mathf.Sign(newVelocity.y));
                    }
                }
                newVelocity += directionVector.normalized * currentAcceleration;
                newVelocity = Vector2.ClampMagnitude(newVelocity, currentMaxSpeed);

                rigidbody.velocity = newVelocity;
            }
        }

        private void OnRollStart(ParamsObject paramsObj) {
            rolling = true;
        }

        private void OnRollEnd(ParamsObject paramsObj) {
            rolling = false;
        }
    }
}
using EventManagers;
using UnityEngine;
using UnityEngine.Events;
using Events;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {

        [SerializeField]
        private float maxRunSpeed = 15;
        [SerializeField]
        private float maxWalkSpeed = 5;

        private new Rigidbody2D rigidbody;
        private GameObjectEventManager eventManager;

        private float currentMaxSpeed;

        private Vector2 directionVector = Vector2.zero;

        private bool walking;
        private bool movementEnabled;

        void Awake()
        {
            eventManager = GetComponent<GameObjectEventManager>();
            rigidbody = GetComponent<Rigidbody2D>();
            movementEnabled = true;
            eventManager.StartListening(PlayerEvents.OnDisableMovement, new UnityAction<ParamsObject>(OnDisableMovement));
            eventManager.StartListening(PlayerEvents.OnEnableMovement, new UnityAction<ParamsObject>(OnEnableMovement));
        }

        void Start()
        {
            eventManager.TriggerEvent(PlayerEvents.OnSendMovementSpeed, new ParamsObject(maxRunSpeed));
        }

        void FixedUpdate()
        {
            if(movementEnabled)
            {
                currentMaxSpeed = PlayerInput.GetButton("Walk") ? maxWalkSpeed : maxRunSpeed;

                directionVector.Set(PlayerInput.GetAxisRaw("HorizontalMovement"), PlayerInput.GetAxisRaw("VerticalMovement"));

                rigidbody.velocity = Vector2.ClampMagnitude(directionVector.normalized * currentMaxSpeed, currentMaxSpeed);
            }
        }

        private void OnDisableMovement(ParamsObject paramsObj)
        {
            movementEnabled = false;
        }

        private void OnEnableMovement(ParamsObject paramsObj)
        {
            movementEnabled = true;
        }
    }
}
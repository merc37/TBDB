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
        private bool rolling;

        void Awake()
        {
            eventManager = GetComponent<GameObjectEventManager>();
            rigidbody = GetComponent<Rigidbody2D>();
            rolling = false;
            eventManager.StartListening(PlayerEvents.OnRollStart, new UnityAction<ParamsObject>(OnRollStart));
            eventManager.StartListening(PlayerEvents.OnRollEnd, new UnityAction<ParamsObject>(OnRollEnd));
        }

        void Start()
        {
            eventManager.TriggerEvent(PlayerEvents.OnSendMovementSpeed, new ParamsObject(maxRunSpeed));
        }

        void FixedUpdate()
        {
            if(!rolling)
            {
                currentMaxSpeed = Input.GetButton("Walk") ? maxWalkSpeed : maxRunSpeed;

                directionVector.Set(Input.GetAxisRaw("HorizontalMovement"), Input.GetAxisRaw("VerticalMovement"));

                rigidbody.velocity = Vector2.ClampMagnitude(directionVector.normalized * currentMaxSpeed, currentMaxSpeed);
            }
        }

        private void OnRollStart(ParamsObject paramsObj)
        {
            rolling = true;
        }

        private void OnRollEnd(ParamsObject paramsObj)
        {
            rolling = false;
        }
    }
}
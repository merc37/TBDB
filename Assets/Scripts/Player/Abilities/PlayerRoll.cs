using EventManagers;
using UnityEngine;
using UnityEngine.Events;
using Events;

namespace Player
{
    public class PlayerRoll : PlayerAbility
    {
        [SerializeField]
        private float rollDistance = 5;
        [SerializeField]
        private float velocityThreshold = .1f;

        private new Rigidbody2D rigidbody;

        private Vector2 rollStartPos;
        private Vector2 rollVelocity;
        private float movementSpeed;

        private UnityAction<ParamsObject> onSendMovementSpeedUnityAction;

        protected override void Awake()
        {
            base.Awake();
            rigidbody = GetComponentInParent<Rigidbody2D>();
            onSendMovementSpeedUnityAction = new UnityAction<ParamsObject>(OnSendMovementSpeed);
            eventManager.StartListening(PlayerEvents.OnSendMovementSpeed, onSendMovementSpeedUnityAction);
        }

        protected override bool StartAbility()
        {
            if (rigidbody.velocity.magnitude > velocityThreshold)
            {
                eventManager.TriggerEvent(PlayerEvents.OnDisableMovement);
                rollStartPos = rigidbody.position;
                rollVelocity = rigidbody.velocity.normalized * movementSpeed * 3;
                rigidbody.velocity = rollVelocity;
                //GetComponent<Animator>().SetTrigger("Roll");
                return true;
            }

            return false;
        }

        protected override void Update()
        {
            base.Update();
            if (IsAbilityActive())
            {
                if (Vector2.Distance(rollStartPos, rigidbody.position) >= rollDistance)
                {
                    rigidbody.velocity = Vector2.zero;
                    eventManager.TriggerEvent(PlayerEvents.OnEnableMovement);
                    AbilityEnd();
                }
                if (!rollVelocity.Equals(rigidbody.velocity))
                {
                    eventManager.TriggerEvent(PlayerEvents.OnEnableMovement);
                    AbilityEnd();
                }
            }
        }

        private void OnSendMovementSpeed(ParamsObject paramsObj)
        {
            movementSpeed = paramsObj.Float;
            eventManager.StopListening(PlayerEvents.OnSendMovementSpeed, onSendMovementSpeedUnityAction);
        }
    }
}
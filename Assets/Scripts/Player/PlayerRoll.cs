using EventManagers;
using UnityEngine;
using UnityEngine.Events;
using Events;

namespace Player
{

    public class PlayerRoll : MonoBehaviour
    {

        [SerializeField]
        private float rollCooldownTime = 1;
        [SerializeField]
        private float rollDistance = 5;
        [SerializeField]
        private float velocityThreshold = .1f;
        [SerializeField]
        private float rollRecoveryTime = .3f;

        private GameObjectEventManager eventManager;
        private new Rigidbody2D rigidbody;

        private bool shouldRoll;
        private bool rolling;
        private bool rollOnCooldown;
        private float rollCooldownTimer;

        private bool rollRecovery;
        private float rollRecoveryTimer;

        private Vector2 rollStartPos;
        private Vector2 rollVelocity;
        private float movementSpeed;

        private UnityAction<ParamsObject> onSendMovementSpeedUnityAction;

        void Awake()
        {
            eventManager = GetComponent<GameObjectEventManager>();
            rigidbody = GetComponent<Rigidbody2D>();
            rollCooldownTimer = rollCooldownTime;
            rollRecoveryTimer = rollRecoveryTime;
            rollOnCooldown = false;
            rolling = false;
            rollRecovery = false;
            onSendMovementSpeedUnityAction = new UnityAction<ParamsObject>(OnSendMovementSpeed);
            eventManager.StartListening(PlayerEvents.OnSendMovementSpeed, onSendMovementSpeedUnityAction);
        }

        void Update()
        {
            if(shouldRoll)
            {
                if(rigidbody.velocity.magnitude > velocityThreshold)
                {
                    rollStartPos = rigidbody.position;
                    rollVelocity = rigidbody.velocity.normalized * movementSpeed * 3;
                    rigidbody.velocity = rollVelocity;
                    shouldRoll = false;
                    eventManager.TriggerEvent(PlayerEvents.OnRollStart);
                    rolling = true;
                }
                else
                {
                    shouldRoll = false;
                }
            }

            if(rollOnCooldown && !rolling && !rollRecovery)
            {
                rollCooldownTimer -= Time.deltaTime;
                if(rollCooldownTimer <= 0)
                {
                    rollCooldownTimer = rollCooldownTime;
                    eventManager.TriggerEvent(PlayerEvents.OnRollCooldownEnd);
                    rollOnCooldown = false;
                }
            }

            if(!rollOnCooldown && rolling && rollRecovery)
            {
                rollRecoveryTimer -= Time.deltaTime;
                if(rollRecoveryTimer <= 0)
                {
                    rollRecoveryTimer = rollRecoveryTime;
                    eventManager.TriggerEvent(PlayerEvents.OnRollCooldownStart);
                    rollRecovery = false;
                    rolling = false;
                    rollOnCooldown = true;
                    eventManager.TriggerEvent(PlayerEvents.OnRollEnd);
                }
            }

            if(!rollOnCooldown && !rollRecovery && rolling)
            {
                if(Vector2.Distance(rollStartPos, rigidbody.position) >= rollDistance)
                {
                    rigidbody.velocity = Vector2.zero;
                    rollRecovery = true;
                }
                if(!rollVelocity.Equals(rigidbody.velocity))
                {
                    rollRecovery = true;
                }
            }

            if(!rollOnCooldown && !rolling && !rollRecovery && Input.GetButtonDown("Roll"))
            {
                shouldRoll = true;
            }

        }

        private void OnSendMovementSpeed(ParamsObject paramsObj)
        {
            movementSpeed = paramsObj.Float;
            eventManager.StopListening(PlayerEvents.OnSendMovementSpeed, onSendMovementSpeedUnityAction);
        }

        void OnDrawGizmos()
        {
            if(rollRecovery && !rolling)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawSphere(rigidbody.position, .5f);
            }
            if(rolling && !rollRecovery)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(rigidbody.position, .5f);
            }
        }
    }
}
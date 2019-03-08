using UnityEngine;
using UnityEngine.Events;
using EventManagers;
using Panda;
using Events;

namespace Enemy
{
    public class RollTasks : MonoBehaviour
    {
        [SerializeField]
        private LayerMask movementBlockMask;
        [SerializeField]
        private float rollCooldownTime = 5;
        [SerializeField]
        private float rollDistance = 5;

        private float rollCooldownTimer;
        private float movementSpeed;

        private bool shouldRollAwayFromProjectile;
        private bool rolling;
        private bool rollOnCooldown;
        private bool roll;

        private Vector2 rollDirection;
        private Vector2 rollStartPos;
        private Vector2 rollVelocity;

        private GameObjectEventManager eventManager;
        private new Rigidbody2D rigidbody;

        private UnityAction<ParamsObject> onSendMovementSpeedUnityAction;

        void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();

            eventManager = GetComponent<GameObjectEventManager>();
            eventManager.StartListening(EnemyEvents.OnRoll, new UnityAction<ParamsObject>(OnRoll));
            onSendMovementSpeedUnityAction = new UnityAction<ParamsObject>(OnSendMovementSpeed);
            eventManager.StartListening(EnemyEvents.OnSendMovementSpeed, onSendMovementSpeedUnityAction);

            rollCooldownTimer = rollCooldownTime;
        }

        void Update()
        {
            if(roll)
            {
                RaycastHit2D posDir = Physics2D.Raycast(rigidbody.position, rollDirection.normalized, rollDistance, movementBlockMask);
                RaycastHit2D negDir = Physics2D.Raycast(rigidbody.position, -rollDirection.normalized, rollDistance, movementBlockMask);
                if(!posDir && !negDir)
                {
                    if(Random.value > .5f)
                    {
                        rollDirection = -rollDirection;
                    }
                }
                if(posDir && negDir)
                {
                    if(negDir.distance > posDir.distance)
                    {
                        rollDirection = -rollDirection;
                    }
                    if(negDir.distance == posDir.distance)
                    {
                        if(Random.value > .5f)
                        {
                            rollDirection = -rollDirection;
                        }
                    }
                }
                if(posDir && !negDir)
                {
                    rollDirection = -rollDirection;
                }

                rollStartPos = rigidbody.position;
                rigidbody.velocity = rollDirection.normalized * movementSpeed * 3;
                rollVelocity = rigidbody.velocity;
                shouldRollAwayFromProjectile = false;
                roll = false;
                eventManager.TriggerEvent(EnemyEvents.OnRollStart);
                rolling = true;
            }

            if(rollOnCooldown && !rolling)
            {
                rollCooldownTimer -= Time.deltaTime;
                if(rollCooldownTimer <= 0)
                {
                    rollCooldownTimer = rollCooldownTime;
                    eventManager.TriggerEvent(EnemyEvents.OnRollCooldownEnd);
                    rollOnCooldown = false;
                }
            }

            if(!rollOnCooldown && rolling)
            {
                if(Vector2.Distance(rollStartPos, rigidbody.position) >= rollDistance)
                {
                    rigidbody.velocity = Vector2.zero;
                    eventManager.TriggerEvent(EnemyEvents.OnRollEnd);
                    rolling = false;
                    eventManager.TriggerEvent(EnemyEvents.OnRollCooldownStart);
                    rollOnCooldown = true;
                }
                if(!rollVelocity.Equals(rigidbody.velocity))
                {
                    eventManager.TriggerEvent(EnemyEvents.OnRollEnd);
                    rolling = false;
                    eventManager.TriggerEvent(EnemyEvents.OnRollCooldownStart);
                    rollOnCooldown = true;
                }
            }
        }

        [Task]
        bool IsRolling()
        {
            return rolling;
        }

        [Task]
        bool ShouldRollAwayFromProjectile()
        {
            return shouldRollAwayFromProjectile;
        }

        [Task]
        bool IsRollOnCooldown()
        {
            return rollOnCooldown;
        }

        [Task]
        bool Roll()
        {
            roll = true;
            return true;
        }

        private void OnRoll(ParamsObject paramsObj)
        {
            shouldRollAwayFromProjectile = true;
            rollDirection = Vector3.Cross(paramsObj.Vector2, Vector3.forward);
        }

        private void OnSendMovementSpeed(ParamsObject paramsObj)
        {
            movementSpeed = paramsObj.Float;
            eventManager.StopListening(EnemyEvents.OnSendMovementSpeed, onSendMovementSpeedUnityAction);
        }

    }
}

using EventManagers;
using UnityEngine;
using UnityEngine.Events;

namespace Player {

    public class PlayerRoll : MonoBehaviour {

        [SerializeField]
        private float rollCooldownTime = 1;
        [SerializeField]
        private float rollDistance = 5;
        [SerializeField]
        private float velocityThreshold = .1f;

        private GameObjectEventManager eventManager;
        private new Rigidbody2D rigidbody;

        private bool shouldRoll;
        private bool rolling;
        private bool rollOnCooldown;
        private float rollCooldownTimer;
        private Vector2 rollStartPos;
        private Vector2 rollVelocity;
        private float movementSpeed;

        void Awake() {
            eventManager = GetComponent<GameObjectEventManager>();
            rigidbody = GetComponent<Rigidbody2D>();
            rollCooldownTimer = rollCooldownTime;
            rollOnCooldown = false;
            rolling = false;
            eventManager.StartListening("SendMovementSpeed", new UnityAction<ParamsObject>(ReturnMovementSpeed));
        }

        void Update() {

            if(rollOnCooldown && !rolling) {
                rollCooldownTimer -= Time.deltaTime;
                if(rollCooldownTimer <= 0) {
                    rollCooldownTimer = rollCooldownTime;
                    eventManager.TriggerEvent("OnRollCooldownEnd");
                    rollOnCooldown = false;
                }
            }

            if(!rollOnCooldown && rolling) {
                if(Vector2.Distance(rollStartPos, rigidbody.position) >= rollDistance) {
                    rigidbody.velocity = Vector2.zero;
                    eventManager.TriggerEvent("OnRollEnd");
                    rolling = false;
                    eventManager.TriggerEvent("OnRollCooldownStart");
                    rollOnCooldown = true;
                }
                if(!rollVelocity.Equals(rigidbody.velocity)) {
                    eventManager.TriggerEvent("OnRollEnd");
                    rolling = false;
                    eventManager.TriggerEvent("OnRollCooldownStart");
                    rollOnCooldown = true;
                }
            }

            if(!rollOnCooldown && !rolling && Input.GetButtonDown("Roll")) {
                shouldRoll = true;
            }

        }

        void FixedUpdate() {
            if (shouldRoll) {
                if(rigidbody.velocity.magnitude > velocityThreshold) {
                    rollStartPos = rigidbody.position;
                    rollVelocity = rigidbody.velocity.normalized * movementSpeed * 3;
                    rigidbody.velocity = rollVelocity;
                    shouldRoll = false;
                    eventManager.TriggerEvent("OnRollStart");
                    rolling = true;
                } else {
                    shouldRoll = false;
                }
            }
        }

        private void ReturnMovementSpeed(ParamsObject paramsObj) {
            movementSpeed = paramsObj.Float;
        }
    }
}
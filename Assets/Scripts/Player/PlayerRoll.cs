using EventManagers;
using UnityEngine;

namespace Player {

    public class PlayerRoll : MonoBehaviour {

        [SerializeField]
        private float rollCooldownTime = 1;
        [SerializeField]
        private float rollForce = 60;
        [SerializeField]
        private float rollTime = .1f;

        private GameObjectEventManager eventManager;
        private new Rigidbody2D rigidbody;
        public bool Rolling { get; set; }
        public bool RollOnCooldown { get; set; }
        private bool roll;
        private float rollCooldownTimer;
        private float rollTimer;

        void Start()  {
            eventManager = GetComponent<GameObjectEventManager>();
            rigidbody = GetComponent<Rigidbody2D>();
            rollCooldownTimer = rollCooldownTime;
            rollTimer = rollTime;
        }

        void Update() {

            if(!RollOnCooldown && Input.GetButtonDown("Roll")) {
                roll = true;
            }

            if (!RollOnCooldown) {
                rollTimer -= Time.deltaTime;
                if(rollTimer <= 0) {
                    rollTimer = rollTime;
                    Rolling = false;
                    RollOnCooldown = true;
                }
            }

            if(RollOnCooldown) {
                rollCooldownTimer -= Time.deltaTime;
                if(rollCooldownTimer <= 0) {
                    rollCooldownTimer = rollCooldownTime;
                    RollOnCooldown = false;
                    roll = false;
                }
            }

        }

        void FixedUpdate() {
            if (roll) {
                rigidbody.AddForce(rigidbody.velocity.normalized * 30, ForceMode2D.Impulse);
                roll = false;
                Rolling = true;
            }
        }
    }
}
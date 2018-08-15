using EventManagers;
using UnityEngine;

namespace Player {

    public class PlayerRoll : MonoBehaviour {

        [SerializeField]
        private float rollCooldownTime;
        [SerializeField]
        private float rollForce;

        private GameObjectEventManager eventManager;
        private new Rigidbody2D rigidbody;
        private bool rolling;
        private float rollCooldownTimer;

        void Start()  {
            eventManager = GetComponent<GameObjectEventManager>();
            rigidbody = GetComponent<Rigidbody2D>();
            rollCooldownTimer = rollCooldownTime;
        }

        void Update() {
            if (rolling) {
                rollCooldownTimer -= Time.deltaTime;
                if (rollCooldownTimer <= 0) {
                    rollCooldownTimer = rollCooldownTime;
                    rolling = false;
                }
            }
        }

        void FixedUpdate() {
            if (!rolling && Input.GetButtonDown("Roll")) {
                //rigidbody.velocity = Vector2.zero;
                rigidbody.AddForce(rigidbody.velocity.normalized * rollForce, ForceMode2D.Impulse);
                rolling = true;
            }
        }
    }
}
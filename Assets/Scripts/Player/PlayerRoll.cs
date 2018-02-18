using EventManagers;
using UnityEngine;

namespace Player {

    public class PlayerRoll : MonoBehaviour {

        [SerializeField]
        private float rollTime;
        [SerializeField]
        private float rollForce;

        private GameObjectEventManager eventManager;
        private Rigidbody2D rigidBody;
        private bool rolling;
        private float rollTimer;

        void Start()  {
            eventManager = GetComponent<GameObjectEventManager>();
            rigidBody = GetComponent<Rigidbody2D>();
            rollTimer = rollTime;
        }

        void Update() {
            if (rolling) {
                rollTimer -= Time.deltaTime;
                if (rollTimer <= 0) {
                    rollTimer = rollTime;
                    rolling = false;
                }
            }
        }

        void FixedUpdate() {
            if (!rolling && Input.GetButtonDown("Roll")) {
                rigidBody.AddForce(rigidBody.velocity.normalized * rollForce, ForceMode2D.Impulse);
                rolling = true;
            }
        }
    }
}
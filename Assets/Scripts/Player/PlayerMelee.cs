using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player {

    public class PlayerMelee : MonoBehaviour {

        [SerializeField]
        private Collider2D meleeBox;
        [SerializeField]
        private float attackTime;
        [SerializeField]
        private float meleeCooldownTime;

        private GameObjectEventManager eventManager;
        private bool attacking = false;
        private float lastStartAttackTime = 0;
        private float deltaStartAttackTime = 0;
        private bool cooldown = false;
        private float lastEndAttackTime = 0;
        private float deltaEndAttackTime = 0;

        void Start() {
            eventManager = GetComponent<GameObjectEventManager>();
            meleeBox.enabled = false;
        }

        void Update() {
            if(attacking) {
                deltaStartAttackTime = Time.time - lastStartAttackTime;
                if(deltaStartAttackTime >= attackTime) {
                    attacking = false;
                    cooldown = true;
                    lastEndAttackTime = Time.time;
                }
            }

            if(cooldown) {
                deltaEndAttackTime = Time.time - lastEndAttackTime;
                if(deltaEndAttackTime >= meleeCooldownTime) {
                    cooldown = false;
                }
            }

            if (Input.GetButtonDown("Melee") && !attacking && !cooldown) {
                attacking = true;
                lastStartAttackTime = Time.time;
            }
        }

    }
}

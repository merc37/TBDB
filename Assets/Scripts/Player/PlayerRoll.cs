using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player {
    public class PlayerRoll : MonoBehaviour {

        private Rigidbody2D rigidBody;

        void Start() {
            rigidBody = GetComponent<Rigidbody2D>();
        }

        void FixedUpdate() {
            Debug.Log(rigidBody.velocity);
            if(Input.GetButtonDown("Roll")) {
                rigidBody.AddForce(rigidBody.velocity * 3, ForceMode2D.Impulse);
            }
        }
    }
}
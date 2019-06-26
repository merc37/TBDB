using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerPickup : MonoBehaviour
    {
        private Pickup availablePickup;

        void Update() {
            if (PlayerInput.GetButtonDown("Interact")) {
                if(availablePickup != null) {
                    if(availablePickup.OnPickup(transform)) {
                        Destroy(availablePickup.gameObject);
                    }
                    availablePickup = null;
                }
            }
        }

        void OnTriggerEnter2D(Collider2D collider) {
            availablePickup = collider.GetComponent<Pickup>();
            if(availablePickup.AutoPickup) {
                if (availablePickup.OnPickup(transform)) {
                    Destroy(availablePickup.gameObject);
                }
                availablePickup = null;
            }
        }

        void OnTriggerExit2D(Collider2D collider) {
            availablePickup = null;
        }
    } 
}

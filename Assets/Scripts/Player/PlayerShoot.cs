using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Player {
	public class PlayerShoot : MonoBehaviour {
        
		private GameObjectEventManager eventManager;

        void Start() {
            eventManager = GetComponent<GameObjectEventManager>();
        }

        // Update is called once per frame
        void Update () {
            if(Input.GetButtonDown("Fire")) {
                if(eventManager != null) {
                    eventManager.TriggerEvent("OnShoot");
                }
            }
		}
	}
}
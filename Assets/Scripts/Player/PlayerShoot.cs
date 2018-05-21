using EventManagers;
using UnityEngine;

namespace Player {
	public class PlayerShoot : MonoBehaviour {
        
		private GameObjectEventManager eventManager;

        void Start() {
            eventManager = GetComponent<GameObjectEventManager>();
        }
        
        void Update () {
            if(Input.GetButtonDown("Fire")) {
                if(eventManager != null) {
                    eventManager.TriggerEvent("OnShoot");
                }
            }

            if (Input.GetButton("Fire")) {
                if (eventManager != null) {
                    eventManager.TriggerEvent("OnAutoShoot");
                }
            }

            if(Input.GetButtonDown("Reload")) {
                if(eventManager != null) {
                    eventManager.TriggerEvent("OnReload");
                }
            }
        }
	}
}
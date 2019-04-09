using EventManagers;
using UnityEngine;
using Events;
using UnityEngine.EventSystems;

namespace Player
{
    public class PlayerShoot : MonoBehaviour
    {

        private GameObjectEventManager eventManager;

        void Start()
        {
            eventManager = GetComponent<GameObjectEventManager>();
        }

        void Update()
        {
            if(PlayerInput.GetButton("Fire"))
            {
                if(eventManager != null)
                {
                    eventManager.TriggerEvent(GunEvents.OnShoot);
                }
            }

            if(PlayerInput.GetButtonUp("Fire"))
            {
                if(eventManager != null)
                {
                    eventManager.TriggerEvent(PlayerEvents.OnUnlockShoot);
                }
            }

            if(PlayerInput.GetButtonDown("Reload"))
            {
                if(eventManager != null)
                {
                    eventManager.TriggerEvent(GunEvents.OnReload);
                }
            }
        }
    }
}
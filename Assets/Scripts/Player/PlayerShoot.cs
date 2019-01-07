using EventManagers;
using UnityEngine;
using Events;

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
            if(Input.GetButton("Fire"))
            {
                if(eventManager != null)
                {
                    eventManager.TriggerEvent(GunEvents.OnShoot);
                }
            }

            if(Input.GetButtonUp("Fire"))
            {
                if(eventManager != null)
                {
                    eventManager.TriggerEvent(PlayerEvents.OnUnlockShoot);
                }
            }

            if(Input.GetButtonDown("Reload"))
            {
                if(eventManager != null)
                {
                    eventManager.TriggerEvent(GunEvents.OnReload);
                }
            }
        }
    }
}
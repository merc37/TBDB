using EventManagers;
using UnityEngine;
using Events;

namespace Player
{
    public class PlayerToggleInventory : MonoBehaviour
    {
        private GameObjectEventManager eventManager;

        void Awake()
        {
            eventManager = GetComponentInParent<GameObjectEventManager>();
        }

        void Update()
        {
            if(Input.GetButtonDown("Inventory"))
            {
                eventManager.TriggerEvent(PlayerEvents.OnInventoryToggle);
            }
        }
    }
}

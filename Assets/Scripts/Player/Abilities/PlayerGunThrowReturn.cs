using EventManagers;
using UnityEngine;
using Collisions;
using Events;
using UnityEngine.Events;

namespace Player
{
    public class PlayerGunThrowReturn : MonoBehaviour
    {
        private GameObjectEventManager eventManager;

        void Awake()
        {
            eventManager = GetComponentInParent<GameObjectEventManager>();
        }

        void OnCollisionEnter2D(Collision2D coll)
        {
            eventManager.TriggerEvent(PlayerEvents.OnReturnGun);
        }

        void OnTriggerEnter2D(Collider2D coll)
        {
            eventManager.TriggerEvent(PlayerEvents.OnReturnGun);
        }
    }
}

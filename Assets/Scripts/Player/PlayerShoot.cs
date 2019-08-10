using EventManagers;
using UnityEngine;
using Events;

namespace Player
{
    public class PlayerShoot : MonoBehaviour
    {
        [SerializeField]
        private float mouseMinDistance = 2;

        private GameObjectEventManager eventManager;
        private new Rigidbody2D rigidbody;

        void Awake()
        {
            eventManager = GetComponent<GameObjectEventManager>();
            rigidbody = GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            if (PlayerInput.GetButton("Fire"))
            {
                if (eventManager != null)
                {
                    Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition).ToVector2();
                    if (Vector2.Distance(rigidbody.position, target) < mouseMinDistance)
                    {
                        target = (rigidbody.rotation.ToVector2() * mouseMinDistance) + rigidbody.position;
                    }
                    eventManager.TriggerEvent(GunEvents.OnShoot, new ParamsObject(target));
                }
            }

            if (PlayerInput.GetButtonUp("Fire"))
            {
                if (eventManager != null)
                {
                    eventManager.TriggerEvent(PlayerEvents.OnUnlockShoot);
                }
            }

            if (PlayerInput.GetButtonDown("Reload"))
            {
                if (eventManager != null)
                {
                    eventManager.TriggerEvent(GunEvents.OnReload);
                }
            }
        }
    }
}
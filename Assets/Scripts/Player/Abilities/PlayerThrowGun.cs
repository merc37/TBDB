using EventManagers;
using UnityEngine;
using UnityEngine.Events;
using Events;
using Collisions;

namespace Player
{
    public class PlayerThrowGun : PlayerAbility
    {
        [SerializeField]
        private float returnTime = 1;
        [SerializeField]
        private float throwSpeed = 10;

        private Transform gunTransform;

        private Vector2 startLocalPosition;

        private Timer returnTimer;

        protected override void Awake()
        {
            base.Awake();
            eventManager.StartListening(GunEvents.OnUpdateGunTransform, new UnityAction<ParamsObject>(OnUpdateGunTransform));
            eventManager.StartListening(PlayerEvents.OnReturnGun, new UnityAction<ParamsObject>(OnReturnGun));
            returnTimer = new Timer(returnTime);
        }

        protected override bool StartAbility()
        {
            gunTransform.SetParent(null);
            Rigidbody2D rb = gunTransform.gameObject.AddComponent<Rigidbody2D>();
            rb.velocity = transform.up * throwSpeed;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            gunTransform.gameObject.AddComponent<StopOnCollision>();
            gunTransform.gameObject.AddComponent<BoxCollider2D>();
            gunTransform.gameObject.AddComponent<PlayerGunThrowReturn>();
            return true;
        }

        protected override void Update()
        {
            base.Update();

            if(IsAbilityActive())
            {
                if(returnTimer.Tick())
                {
                    gunTransform.parent = transform;
                    gunTransform.localPosition = startLocalPosition;
                    gunTransform.rotation = transform.rotation;
                    eventManager.TriggerEvent(GunEvents.OnReload);
                    Destroy(gunTransform.gameObject.GetComponent<StopOnCollision>());
                    Destroy(gunTransform.gameObject.GetComponent<BoxCollider2D>());
                    Destroy(gunTransform.gameObject.GetComponent<Rigidbody2D>());
                    Destroy(gunTransform.gameObject.GetComponent<PlayerGunThrowReturn>());
                    AbilityEnd();
                }
            }
        }

        private void OnUpdateGunTransform(ParamsObject paramsObj)
        {
            gunTransform = paramsObj.Transform;
            startLocalPosition = gunTransform.localPosition;
        }

        private void OnReturnGun(ParamsObject paramsObj)
        {
            gunTransform.parent = transform;
            gunTransform.localPosition = startLocalPosition;
            gunTransform.rotation = transform.rotation;
            eventManager.TriggerEvent(GunEvents.OnReload);
            Destroy(gunTransform.gameObject.GetComponent<StopOnCollision>());
            Destroy(gunTransform.gameObject.GetComponent<BoxCollider2D>());
            Destroy(gunTransform.gameObject.GetComponent<Rigidbody2D>());
            Destroy(gunTransform.gameObject.GetComponent<PlayerGunThrowReturn>());
        }
    }
}

using EventManagers;
using UnityEngine;
using UnityEngine.Events;
using Events;

namespace Player
{
    public class PlayerGun : Gun
    {

        private bool shootLocked = false;

        protected Rigidbody2D playerRigidbody;

        private Rigidbody2D _lastFired;
        public Rigidbody2D LastFired
        {
            get
            {
                return _lastFired;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            playerRigidbody = GetComponentInParent<Rigidbody2D>();
            eventManager.StartListening(PlayerEvents.OnUnlockShoot, new UnityAction<ParamsObject>(OnUnlockShoot));
        }

        protected override void OnShoot(ParamsObject paramsObj)
        {
            if(!shootLocked)
            {
                base.OnShoot(paramsObj);
                //GetComponentInParent<Animator>().SetTrigger("Shoot");
                if(!automatic)
                {
                    shootLocked = true;
                }
            }
        }

        protected override Rigidbody2D FireProjectile()
        {
            _lastFired = base.FireProjectile();
            ParamsObject paramsObj = new ParamsObject(7);
            paramsObj.Vector2 = playerRigidbody.position;
            GameObjectEventManager.TriggerRadiusEvent(PlayerRadiusEvents.OnPlayerMakeNoise, transform.position, 10, LayerMask.GetMask("Enemies"), paramsObj);
            return LastFired;
        }

        private void OnUnlockShoot(ParamsObject paramsObj)
        {
            shootLocked = false;
        }
    }
}


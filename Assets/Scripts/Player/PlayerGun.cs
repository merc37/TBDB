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

        public Rigidbody2D LastFired
        {
            get; private set;
        }

        protected override void Awake()
        {
            base.Awake();
            playerRigidbody = GetComponentInParent<Rigidbody2D>();
            eventManager.StartListening(PlayerEvents.OnUnlockShoot, new UnityAction<ParamsObject>(OnUnlockShoot));
        }

        protected override void OnShoot(ParamsObject paramsObj)
        {
            bool shootOverride = paramsObj != null ? paramsObj.Bool : false;
            if (!shootLocked || shootOverride)
            {
                base.OnShoot(paramsObj);
                //GetComponentInParent<Animator>().SetTrigger("Shoot");
                if (!automatic || shootOverride)
                {
                    shootLocked = true;
                }
            }
        }

        protected override Rigidbody2D FireProjectile(Vector2 target)
        {
            LastFired = base.FireProjectile(target);
            ParamsObject paramsObj = new ParamsObject(7);//<-Sound level
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


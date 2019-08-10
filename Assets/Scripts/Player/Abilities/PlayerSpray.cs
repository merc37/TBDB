using Events;
using EventManagers;
using UnityEngine;

namespace Player
{
    public class PlayerSpray : PlayerAbility
    {
        [SerializeField]
        private float sprayTime = 3;
        [SerializeField]
        private float sprayIntervalTime = .02f;

        private new Rigidbody2D rigidbody;

        private Timer sprayTimer;
        private Timer sprayIntervalTimer;

        protected override void Awake()
        {
            base.Awake();
            sprayTimer = new Timer(sprayTime);
            sprayIntervalTimer = new Timer(sprayIntervalTime);
            rigidbody = GetComponentInParent<Rigidbody2D>();
        }

        protected override bool StartAbility()
        {
            sprayIntervalTimer.Reset();
            return true;
        }

        protected override void Update()
        {
            base.Update();
            if (IsAbilityActive())
            {
                if (sprayTimer.Tick())
                {
                    AbilityEnd();
                    return;
                }

                if (sprayIntervalTimer.Tick())
                {
                    ParamsObject paramsObj = new ParamsObject(true);
                    paramsObj.Vector2 = (rigidbody.rotation.ToVector2() * 2) + rigidbody.position;
                    eventManager.TriggerEvent(GunEvents.OnShoot, paramsObj);
                }
            }
        }
    }
}

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

        private Timer sprayTimer;
        private Timer sprayIntervalTimer;

        protected override void Awake()
        {
            base.Awake();
            sprayTimer = new Timer(sprayTime);
            sprayIntervalTimer = new Timer(sprayIntervalTime);
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
                    eventManager.TriggerEvent(GunEvents.OnShoot, new ParamsObject(true));
                }
            }
        }
    }
}

using UnityEngine;

namespace Player
{
    public class PlayerSlowMotion : PlayerAbility
    {
        [SerializeField]
        private float slowMotionPercentage = 50;
        [SerializeField]
        private float slowMotionTime = 5;

        private Timer slowMotionTimer;

        private float originalTimeScale;

        protected override void Awake()
        {
            base.Awake();
            slowMotionTimer = new Timer(slowMotionTime);
        }

        void Start()
        {
            originalTimeScale = Time.timeScale;
        }

        protected override bool StartAbility()
        {
            Time.timeScale = (slowMotionPercentage / 100);
            return true;
        }

        protected override void Update()
        {
            base.Update();
            if (IsAbilityActive())
            {
                if (slowMotionTimer.Tick())
                {
                    Time.timeScale = originalTimeScale;
                    AbilityEnd();
                }
            }
        }
    }
}

using EventManagers;
using Events;
using UnityEngine;

namespace Player
{
    public class PlayerSlowMotion : MonoBehaviour
    {
        private GameObjectEventManager eventManager;
        [SerializeField]
        private float slowMotionPercentage;
        [SerializeField]
        private float maxSlowMotionTime;
        public float MaxSlowMotionTime { get; set; }

        private float _slowMotionTime;
        public float CurrentSlowMotionTime
        {
            get { return _slowMotionTime; }
            set
            {
                if(value < 0)
                {
                    _slowMotionTime = 0;
                }
                if(value > maxSlowMotionTime)
                {
                    _slowMotionTime = maxSlowMotionTime;
                }

                _slowMotionTime = value;
                eventManager.TriggerEvent(PlayerEvents.OnUpdateCurrentSlowMotionTime, new ParamsObject(CurrentSlowMotionTime));
            }
        }

        public bool SlowMotionActivated { get; set; }

        private float originalTimeScale;

        void Awake()
        {
            eventManager = GetComponent<GameObjectEventManager>();
            CurrentSlowMotionTime = MaxSlowMotionTime;
        }

        void Start()
        {
            eventManager.TriggerEvent(PlayerEvents.OnUpdateMaxSlowMotionTime, new ParamsObject(MaxSlowMotionTime));
            eventManager.TriggerEvent(PlayerEvents.OnUpdateCurrentSlowMotionTime, new ParamsObject(CurrentSlowMotionTime));
            originalTimeScale = Time.timeScale;
        }

        void Update()
        {
            if(Input.GetButtonDown("SlowMotion"))
            {
                Time.timeScale = Time.timeScale == originalTimeScale ? (slowMotionPercentage / 100) : originalTimeScale;
                SlowMotionActivated = !SlowMotionActivated;
            }

            if(SlowMotionActivated)
            {
                CurrentSlowMotionTime -= Time.deltaTime;
                if(CurrentSlowMotionTime == 0)
                {
                    Time.timeScale = originalTimeScale;
                    SlowMotionActivated = !SlowMotionActivated;
                }
            }
        }
    }
}

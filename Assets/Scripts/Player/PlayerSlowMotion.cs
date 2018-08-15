using EventManagers;
using UnityEngine;

namespace Player {
	public class PlayerSlowMotion : MonoBehaviour {

        private GameObjectEventManager eventManager;
        [SerializeField]
		private float slowMotionPercentage;
		[SerializeField]
		private float maxSlowMotionTime;
		public float MaxSlowMotionTime { get; set; }

		private float _slowMotionTime;
		public float CurrentSlowMotionTime {
			get{return _slowMotionTime;}
			set{
				if(value < 0) {
					_slowMotionTime = 0;
				}
				if(value > maxSlowMotionTime) {
					_slowMotionTime = maxSlowMotionTime;
				}

				_slowMotionTime = value;
                eventManager.TriggerEvent("UpdatePlayerSlowMotionTimeMeter", new ParamsObject(CurrentSlowMotionTime / MaxSlowMotionTime));
            }
		}

		public bool SlowMotionActivated {get; set;}

		// Use this for initialization
		void Start () {
            eventManager = GetComponent<GameObjectEventManager>();
            CurrentSlowMotionTime = MaxSlowMotionTime;
		}
		
		// Update is called once per frame
		void Update () {
			// Slow Motion
			if(Input.GetButtonDown("SlowMotion")) {
				Time.timeScale = Time.timeScale == 1f ? (slowMotionPercentage/100) : 1f;
				SlowMotionActivated = !SlowMotionActivated;
			}

			if(SlowMotionActivated) {
                CurrentSlowMotionTime -= Time.deltaTime;
                if(CurrentSlowMotionTime == 0) {
					Time.timeScale = 1f;
					SlowMotionActivated = !SlowMotionActivated;
				}
			}
		}
	}
}

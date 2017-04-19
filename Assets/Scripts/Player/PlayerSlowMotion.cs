using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player {
	public class PlayerSlowMotion : MonoBehaviour {

		[SerializeField]
		private float slowMotionPercentage;
		[SerializeField]
		private float maxSlowMotionTime;
		public float MaxSlowMotionTime {get{return maxSlowMotionTime;} set{maxSlowMotionTime = value;}}

		private float slowMotionTime;
		public float CurrentSlowMotionTime {
			get{return slowMotionTime;}
			set{
				if(value < 0) {
					slowMotionTime = 0;
				}
				if(value > maxSlowMotionTime) {
					slowMotionTime = maxSlowMotionTime;
				}

				slowMotionTime = value;
			}
		}

		public bool SlowMotionActivated {get; set;}

		// Use this for initialization
		void Start () {
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
				slowMotionTime -= Time.deltaTime;
				if(slowMotionTime == 0) {
					Time.timeScale = 1f;
					SlowMotionActivated = !SlowMotionActivated;
				}
			}
		}
	}
}

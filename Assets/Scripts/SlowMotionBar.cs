using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Player;

namespace UI {
	public class SlowMotionBar : MonoBehaviour {

		[SerializeField]
		private PlayerSlowMotion slowMotion;
		[SerializeField]
		private Image barFill;

		void Update () {
			barFill.fillAmount = (float)slowMotion.CurrentSlowMotionTime / (float)slowMotion.MaxSlowMotionTime;
		}
	}
}
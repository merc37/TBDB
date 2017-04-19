using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
	public class HealthBar : MonoBehaviour {

		[SerializeField]
		private Health health;
		[SerializeField]
		private Image barFill;

		void Update() {
			barFill.fillAmount = (float)health.CurrentAmount / (float)health.MaxAmount;
		}
	}
}
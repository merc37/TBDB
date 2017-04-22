using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
	public class AmmoCount : MonoBehaviour {

		[SerializeField]
		private Ammo ammo;
		[SerializeField]
		private Text ammoCountText;

		void Update() {
			ammoCountText.text = ammo.CurrentAmount + "/" + ammo.MaxAmount;
		}
	}
}
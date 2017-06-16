using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
	public class AmmoCount : MonoBehaviour {

		[SerializeField]
		private Gun gun;
		[SerializeField]
		private Text ammoCountText;

		void Update() {
			ammoCountText.text = gun.CurrentAmmo + "/" + gun.MaxAmmo;
		}
	}
}
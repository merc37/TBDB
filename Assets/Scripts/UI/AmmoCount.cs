using EventManagers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI {
	public class AmmoCount : MonoBehaviour {

		[SerializeField]
		private Gun gun;
		[SerializeField]
		private Text ammoCountText;

        void Awake() {
            GlobalEventManager.StartListening("UpdatePlayerAmmoCount", new UnityAction<ParamsObject>(UpdatePlayerAmmoCount));
        }

        private void UpdatePlayerAmmoCount(ParamsObject paramsObj) {
            ammoCountText.text = paramsObj.Int + "/" + paramsObj.Float;
        }
    }
}
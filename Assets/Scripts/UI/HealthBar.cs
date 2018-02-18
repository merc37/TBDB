using EventManagers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI {
	public class HealthBar : MonoBehaviour {
        
		[SerializeField]
		private Image barFill;

        void Awake() {
            GlobalEventManager.StartListening("UpdatePlayerHealth", new UnityAction<ParamsObject>(UpdatePlayerHealth));
        }

        private void UpdatePlayerHealth(ParamsObject paramsObj) {
            barFill.fillAmount = (float)paramsObj.Int / paramsObj.Float;
        }
	}
}
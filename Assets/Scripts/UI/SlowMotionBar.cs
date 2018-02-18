using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using EventManagers;

namespace UI {
	public class SlowMotionBar : MonoBehaviour {
        
		[SerializeField]
		private Image barFill;

        void Awake() {
            GlobalEventManager.StartListening("UpdatePlayerSlowMotionTimeMeter", new UnityAction<ParamsObject>(UpdatePlayerSlowMotionTimeMeter));
        }

        private void UpdatePlayerSlowMotionTimeMeter(ParamsObject paramsObj) {
            barFill.fillAmount = paramsObj.Float;
        }

    }
}
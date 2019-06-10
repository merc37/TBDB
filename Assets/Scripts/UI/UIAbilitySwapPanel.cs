using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIAbilitySwapPanel : MonoBehaviour
    {
        private Text text;

        void Awake() {
            text = GetComponent<Text>();
            text.enabled = false;
        }

        public void OnSwapModeActivate() {
            text.enabled = true;
        }

        public void OnSwapModeDeactivate() {
            text.enabled = false;
        }
    }
}

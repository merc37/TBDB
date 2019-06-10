using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIAbilityHighlightPanel : MonoBehaviour
    {
        private Image image;

        void Awake() {
            image = GetComponent<Image>();
            image.enabled = false;
        }

        public void OnHighlightActivate() {
            image.enabled = true;
        }

        public void OnHighlightDeactivate() {
            image.enabled = false;
        }
    }
}

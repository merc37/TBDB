using UnityEngine;
using Player;

namespace UI
{
    public class UIAbilityStatusPanels : MonoBehaviour
    {
        private UIAbilityStatusPanel[] uiAbilityStatusPanels;
        private UIAbilityHighlightPanel[] uiAbilityHighlightPanels;
        private UIAbilitySwapPanel uiAbilitySwapPanel;


        void Awake()
        {
            uiAbilityStatusPanels = GetComponentsInChildren<UIAbilityStatusPanel>();
            uiAbilityHighlightPanels = GetComponentsInChildren<UIAbilityHighlightPanel>();
            uiAbilitySwapPanel = GetComponentInChildren<UIAbilitySwapPanel>();
        }

        public void SetPlayerAbility(int index, PlayerAbility value) {
            uiAbilityStatusPanels[index].Ability = value;
        }

        public void OnSwapModeActivate() {
            uiAbilitySwapPanel.OnSwapModeActivate();
        }

        public void OnSwapModeDeactivate() {
            uiAbilitySwapPanel.OnSwapModeDeactivate();
        }

        public void OnHighlightActivate(int index) {
            uiAbilityHighlightPanels[index].OnHighlightActivate();
        }

        public void OnHighlightDeactivate(int index) {
            uiAbilityHighlightPanels[index].OnHighlightDeactivate();
        }
    }
}

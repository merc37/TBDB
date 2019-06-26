using UnityEngine;
using Player;

namespace UI
{
    public class UIAbilityStatusPanels : MonoBehaviour
    {
        private UIAbilityStatusPanel[] uiAbilityStatusPanels;

        void Awake()
        {
            uiAbilityStatusPanels = GetComponentsInChildren<UIAbilityStatusPanel>();
        }

        public void SetPlayerAbility(int index, PlayerAbility value) {
            uiAbilityStatusPanels[index].Ability = value;
        }
    }
}

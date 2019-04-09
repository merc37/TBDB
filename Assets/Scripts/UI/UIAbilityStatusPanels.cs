using UnityEngine;
using EventManagers;
using Events;
using UnityEngine.Events;

namespace UI
{
    public class UIAbilityStatusPanels : MonoBehaviour
    {
        private UIAbilityStatusPanel[] uiAbilityStatusPanels;

        void Awake()
        {
            uiAbilityStatusPanels = GetComponentsInChildren<UIAbilityStatusPanel>();
            GlobalEventManager.StartListening(PlayerGlobalEvents.OnPlayerUpdateAbility1, new UnityAction<ParamsObject>(OnPlayerUpdateAbility1));
            GlobalEventManager.StartListening(PlayerGlobalEvents.OnPlayerUpdateAbility2, new UnityAction<ParamsObject>(OnPlayerUpdateAbility2));
            GlobalEventManager.StartListening(PlayerGlobalEvents.OnPlayerUpdateAbility3, new UnityAction<ParamsObject>(OnPlayerUpdateAbility3));
        }

        private void OnPlayerUpdateAbility1(ParamsObject paramsObj)
        {
            uiAbilityStatusPanels[0].Ability = paramsObj.PlayerAbility;
        }

        private void OnPlayerUpdateAbility2(ParamsObject paramsObj)
        {
            uiAbilityStatusPanels[1].Ability = paramsObj.PlayerAbility;
        }

        private void OnPlayerUpdateAbility3(ParamsObject paramsObj)
        {
            uiAbilityStatusPanels[2].Ability = paramsObj.PlayerAbility;
        }
    }
}

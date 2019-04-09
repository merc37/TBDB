using UnityEngine;
using EventManagers;
using Events;
using UnityEngine.Events;

public class UIAbilityInventory : MonoBehaviour
{
    private UnityAction<ParamsObject> onPlayerUpdateAbility1UnityAction;
    private UnityAction<ParamsObject> onPlayerUpdateAbility2UnityAction;
    private UnityAction<ParamsObject> onPlayerUpdateAbility3UnityAction;

    private UIAbilityReceptacle[] uiAbilityReceptacles;

    void Awake()
    {
        GlobalEventManager.StartListening(PlayerGlobalEvents.OnPlayerInventoryToggle, new UnityAction<ParamsObject>(OnPlayerInventoryToggle));
        onPlayerUpdateAbility1UnityAction = new UnityAction<ParamsObject>(OnPlayerUpdateAbility1);
        onPlayerUpdateAbility2UnityAction = new UnityAction<ParamsObject>(OnPlayerUpdateAbility2);
        onPlayerUpdateAbility3UnityAction = new UnityAction<ParamsObject>(OnPlayerUpdateAbility3);
        uiAbilityReceptacles = GetComponentsInChildren<UIAbilityReceptacle>();
        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        GlobalEventManager.StartListening(PlayerGlobalEvents.OnPlayerUpdateAbility1, onPlayerUpdateAbility1UnityAction);
        GlobalEventManager.StartListening(PlayerGlobalEvents.OnPlayerUpdateAbility2, onPlayerUpdateAbility2UnityAction);
        GlobalEventManager.StartListening(PlayerGlobalEvents.OnPlayerUpdateAbility3, onPlayerUpdateAbility3UnityAction);
    }

    void OnDisable()
    {
        GlobalEventManager.StopListening(PlayerGlobalEvents.OnPlayerUpdateAbility1, onPlayerUpdateAbility1UnityAction);
        GlobalEventManager.StopListening(PlayerGlobalEvents.OnPlayerUpdateAbility2, onPlayerUpdateAbility2UnityAction);
        GlobalEventManager.StopListening(PlayerGlobalEvents.OnPlayerUpdateAbility3, onPlayerUpdateAbility3UnityAction);
    }

    private void OnPlayerUpdateAbility1(ParamsObject paramsObj)
    {
        uiAbilityReceptacles[0].Ability = paramsObj.PlayerAbility;
    }

    private void OnPlayerUpdateAbility2(ParamsObject paramsObj)
    {
        uiAbilityReceptacles[1].Ability = paramsObj.PlayerAbility;
    }

    private void OnPlayerUpdateAbility3(ParamsObject paramsObj)
    {
        uiAbilityReceptacles[2].Ability = paramsObj.PlayerAbility;
    }

    private void OnPlayerInventoryToggle(ParamsObject paramsObj)
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}

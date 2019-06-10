using UnityEngine.Events;
using UnityEngine;
using Events;

namespace EventManagers
{
    public class PlayerGlobalEventManager : MonoBehaviour
    {
        private GameObjectEventManager eventManager;

        void Awake()
        {
            eventManager = GetComponent<GameObjectEventManager>();
            eventManager.StartListening(HealthEvents.OnUpdateMaxHealth, new UnityAction<ParamsObject>(OnUpdateMaxHealth));
            eventManager.StartListening(HealthEvents.OnUpdateCurrentHealth, new UnityAction<ParamsObject>(OnUpdateCurrentHealth));
            eventManager.StartListening(GunEvents.OnUpdateCurrentAmmo, new UnityAction<ParamsObject>(OnUpdateCurrentAmmo));
            eventManager.StartListening(GunEvents.OnUpdateMaxAmmo, new UnityAction<ParamsObject>(OnUpdateMaxAmmo));
        }

        void Start()
        {
            GlobalEventManager.TriggerEvent(PlayerGlobalEvents.OnPlayerSendTransform, new ParamsObject(transform));
            GlobalEventManager.TriggerEvent(PlayerGlobalEvents.OnPlayerSendRigidbody, new ParamsObject(GetComponent<Rigidbody2D>()));
        }

        private void OnUpdateMaxHealth(ParamsObject paramsObj) {
            GlobalEventManager.TriggerEvent(PlayerGlobalEvents.OnPlayerUpdateMaxHealth, paramsObj);
        }

        private void OnUpdateCurrentHealth(ParamsObject paramsObj) {
            GlobalEventManager.TriggerEvent(PlayerGlobalEvents.OnPlayerUpdateCurrentHealth, paramsObj);
        }

        private void OnUpdateCurrentAmmo(ParamsObject paramsObj) {
            GlobalEventManager.TriggerEvent(PlayerGlobalEvents.OnPlayerUpdateCurrentAmmo, paramsObj);
        }

        private void OnUpdateMaxAmmo(ParamsObject paramsObj) {
            GlobalEventManager.TriggerEvent(PlayerGlobalEvents.OnPlayerUpdateMaxAmmo, paramsObj);
        }
    }
}

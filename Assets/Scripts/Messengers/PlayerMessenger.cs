using UnityEngine.Events;
using UnityEngine;
using EventManagers;

namespace Messengers {

    public class PlayerMessenger : MonoBehaviour {

        private GameObjectEventManager eventManager;

        void Awake() {
            eventManager = GetComponent<GameObjectEventManager>();
            eventManager.StartListening("UpdatePlayerSlowMotionTimeMeter", new UnityAction<ParamsObject>(UpdatePlayerSlowMotionTimeMeter));
            eventManager.StartListening("UpdateHealth", new UnityAction<ParamsObject>(UpdatePlayerHealth));
            eventManager.StartListening("UpdateAmmoCount", new UnityAction<ParamsObject>(UpdatePlayerAmmoCount));
            GlobalEventManager.StartListening("RequestPlayerTransform", new UnityAction<ParamsObject>(SendPlayerTransform));
        }

        private void SendPlayerTransform(ParamsObject paramsObj) {
            ParamsObject newParamsObj = new ParamsObject(transform);
            GlobalEventManager.TriggerEvent("ReturnPlayerTransform", newParamsObj);
        }

        private void UpdatePlayerSlowMotionTimeMeter(ParamsObject paramsObj) {
            GlobalEventManager.TriggerEvent("UpdatePlayerSlowMotionTimeMeter", paramsObj);
        }

        private void UpdatePlayerHealth(ParamsObject paramsObj) {
            GlobalEventManager.TriggerEvent("UpdatePlayerHealth", paramsObj);
        }

        private void UpdatePlayerAmmoCount(ParamsObject paramsObj) {
            GlobalEventManager.TriggerEvent("UpdatePlayerAmmoCount", paramsObj);
        }
    }
}

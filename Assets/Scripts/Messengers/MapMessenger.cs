using UnityEngine.Events;
using UnityEngine;
using EventManagers;

namespace Messengers {
    public class MapMessenger : MonoBehaviour {

        void Awake() {
            GlobalEventManager.StartListening("RequestMapTransform", new UnityAction<ParamsObject>(SendMapTransform));
        }

        private void SendMapTransform(ParamsObject paramsObj) {
            ParamsObject newParamsObj = new ParamsObject(transform);
            GlobalEventManager.TriggerEvent("ReturnMapTransform", newParamsObj);
        }
    }
}

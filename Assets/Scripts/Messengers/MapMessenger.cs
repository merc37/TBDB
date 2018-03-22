using UnityEngine.Events;
using UnityEngine;
using EventManagers;

namespace Messengers {
    public class MapMessenger : MonoBehaviour {

        void Awake() {
            GlobalEventManager.StartListening("RequestMapTransform", new UnityAction<ParamsObject>(SendUpMapTransform));
        }

        private void SendUpMapTransform(ParamsObject paramsObj) {
            ParamsObject newParamsObj = new ParamsObject(transform);
            GlobalEventManager.TriggerEvent("ReturnMapTransform", newParamsObj);
        }
    }
}

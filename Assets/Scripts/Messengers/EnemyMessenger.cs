using UnityEngine.Events;
using EventManagers;
using UnityEngine;

public class EnemyMessenger : MonoBehaviour {

    private GameObjectEventManager eventManager;

    void Awake() {
        eventManager = GetComponent<GameObjectEventManager>();
        GlobalEventManager.StartListening("ReturnPlayerTransform", new UnityAction<ParamsObject>(SendDownPlayerTransform));
        GlobalEventManager.StartListening("ReturnMapTransform", new UnityAction<ParamsObject>(SendDownMapTransform));
    }

    void Start() {
        GlobalEventManager.TriggerEvent("RequestPlayerTransform");
        GlobalEventManager.TriggerEvent("RequestMapTransform");
    }

    private void SendDownPlayerTransform(ParamsObject paramsObj) {
        eventManager.TriggerEvent("ReturnPlayerTransform", paramsObj);
        GlobalEventManager.StopListening("ReturnPlayerTransform", new UnityAction<ParamsObject>(SendDownPlayerTransform));
    }

    private void SendDownMapTransform(ParamsObject paramsObj) {
        eventManager.TriggerEvent("ReturnMapTransform", paramsObj);
        GlobalEventManager.StopListening("ReturnMapTransform", new UnityAction<ParamsObject>(SendDownMapTransform));
    }
}

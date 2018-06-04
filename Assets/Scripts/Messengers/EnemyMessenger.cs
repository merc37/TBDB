using UnityEngine.Events;
using EventManagers;
using UnityEngine;

public class EnemyMessenger : MonoBehaviour {

    private GameObjectEventManager eventManager;

    void Awake() {
        eventManager = GetComponent<GameObjectEventManager>();
        GlobalEventManager.StartListening("ReturnPlayerRigidbody", new UnityAction<ParamsObject>(SendDownPlayerRigidbody));
        GlobalEventManager.StartListening("ReturnMapTransform", new UnityAction<ParamsObject>(SendDownMapTransform));
    }

    void Start() {
        GlobalEventManager.TriggerEvent("RequestPlayerRigidbody");
        GlobalEventManager.TriggerEvent("RequestMapTransform");
    }

    private void SendDownPlayerRigidbody(ParamsObject paramsObj) {
        eventManager.TriggerEvent("ReturnPlayerRigidbody", paramsObj);
        GlobalEventManager.StopListening("ReturnPlayerRigidbody", new UnityAction<ParamsObject>(SendDownPlayerRigidbody));
    }

    private void SendDownMapTransform(ParamsObject paramsObj) {
        eventManager.TriggerEvent("ReturnMapTransform", paramsObj);
        GlobalEventManager.StopListening("ReturnMapTransform", new UnityAction<ParamsObject>(SendDownMapTransform));
    }
}

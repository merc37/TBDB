using UnityEngine.Events;
using UnityEngine;
using Events;

namespace EventManagers
{
    public class EnemyGlobalEventManager : MonoBehaviour
    {
        private GameObjectEventManager eventManager;

        private UnityAction<ParamsObject> onPlayerSendRigidbodyUnityAction;
        private UnityAction<ParamsObject> onMapSendTransformUnityAction;

        void Awake()
        {
            eventManager = GetComponent<GameObjectEventManager>();
            onMapSendTransformUnityAction = new UnityAction<ParamsObject>(OnMapSendTransform);
            GlobalEventManager.StartListening(MapGlobalEvents.OnMapSendTransform, onMapSendTransformUnityAction);
            onPlayerSendRigidbodyUnityAction = new UnityAction<ParamsObject>(OnPlayerSendRigidbody);
            GlobalEventManager.StartListening(PlayerGlobalEvents.OnPlayerSendRigidbody, onPlayerSendRigidbodyUnityAction);
        }

        private void OnPlayerSendRigidbody(ParamsObject paramsObj)
        {
            eventManager.TriggerEvent(EnemyEvents.OnPlayerSendRigidbody, paramsObj);
            GlobalEventManager.StopListening(PlayerGlobalEvents.OnPlayerSendRigidbody, onPlayerSendRigidbodyUnityAction);
        }

        private void OnMapSendTransform(ParamsObject paramsObj)
        {
            eventManager.TriggerEvent(EnemyEvents.OnMapSendTransform, paramsObj);
            GlobalEventManager.StopListening(MapGlobalEvents.OnMapSendTransform, onMapSendTransformUnityAction);
        }
    }
}

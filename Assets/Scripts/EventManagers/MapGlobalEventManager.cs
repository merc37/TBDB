using UnityEngine;
using Events;

namespace EventManagers
{
    public class MapGlobalEventManager : MonoBehaviour
    {
        void Start()
        {
            GlobalEventManager.TriggerEvent(MapGlobalEvents.OnMapSendTransform, new ParamsObject(transform));
        }
    }
}

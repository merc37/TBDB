using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EventManagers {

    public class GameObjectEventManager : MonoBehaviour {

        private Dictionary<string, ParamsEvent> eventDictionary;

        void Awake() {
            if(eventDictionary == null) {
                eventDictionary = new Dictionary<string, ParamsEvent>();
            }
        }

        public void StartListening(string eventName, UnityAction<ParamsObject> listener) {
            ParamsEvent thisEvent = null;

            if(eventDictionary.TryGetValue(eventName, out thisEvent)) {
                thisEvent.AddListener(listener);
            } else {
                thisEvent = new ParamsEvent();
                thisEvent.AddListener(listener);
                eventDictionary.Add(eventName, thisEvent);
            }
        }

        public void StopListening(string eventName, UnityAction<ParamsObject> listener) {
            ParamsEvent thisEvent = null;
            if(eventDictionary.TryGetValue(eventName, out thisEvent)) {
                thisEvent.RemoveListener(listener);
            }
        }

        public void TriggerEvent(string eventName, ParamsObject paramsObj = null) {
            ParamsEvent thisEvent = null;
            if(eventDictionary.TryGetValue(eventName, out thisEvent)) {
                thisEvent.Invoke(paramsObj);
            }
        }
    }
}

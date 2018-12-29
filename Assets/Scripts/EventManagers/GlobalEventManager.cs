using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

/*
 * All events should start with On
 * All message triggers should start with Send
 * All message listens should start with Return and should stop listening after receviced
 * Triggers should be put in Start
 * Listens in Awake
 * And for Global events: make sure to hand off heavy operation to that entity, because events happen sequentially
 * when triggered (I think)
 * Essentially if the player invokes a global event, it runs every single method attached in the player's update
 * So try to save it for enemy update
 */

namespace EventManagers {
    public class GlobalEventManager : MonoBehaviour {

        private Dictionary<string, ParamsEvent> eventDictionary;

        private static GlobalEventManager eventManager;

        public static GlobalEventManager instance
        {
            get {
                if(!eventManager) {
                    eventManager = FindObjectOfType(typeof(GlobalEventManager)) as GlobalEventManager;

                    if(!eventManager) {
                        Debug.LogError("There needs to be one active EventManger script on a GameObject in your scene.");
                    } else {
                        eventManager.Init();
                    }
                }

                return eventManager;
            }
        }

        void Init() {
            if(eventDictionary == null) {
                eventDictionary = new Dictionary<string, ParamsEvent>();
            }
        }

        public static void StartListening(string eventName, UnityAction<ParamsObject> listener) {
            ParamsEvent thisEvent = null;
            if(instance.eventDictionary.TryGetValue(eventName, out thisEvent)) {
                thisEvent.AddListener(listener);
            } else {
                thisEvent = new ParamsEvent();
                thisEvent.AddListener(listener);
                instance.eventDictionary.Add(eventName, thisEvent);
            }
        }

        public static void StopListening(string eventName, UnityAction<ParamsObject> listener) {
            if(eventManager == null) return;
            ParamsEvent thisEvent = null;
            if(instance.eventDictionary.TryGetValue(eventName, out thisEvent)) {
                thisEvent.RemoveListener(listener);
            }
        }

        public static void TriggerEvent(string eventName, ParamsObject paramsObj = null) {
            ParamsEvent thisEvent = null;
            if(instance.eventDictionary.TryGetValue(eventName, out thisEvent)) {
                thisEvent.Invoke(paramsObj);
            }
        }
    }
}
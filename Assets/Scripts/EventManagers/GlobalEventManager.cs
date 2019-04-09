using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace EventManagers
{
    public class GlobalEventManager : MonoBehaviour
    {

        private Dictionary<string, ParamsEvent> eventDictionary;
        private Dictionary<string, ParamsObject> lastEventValues;

        private static GlobalEventManager eventManager;

        public static GlobalEventManager instance
        {
            get
            {
                if(!eventManager)
                {
                    eventManager = FindObjectOfType(typeof(GlobalEventManager)) as GlobalEventManager;

                    if(!eventManager)
                    {
                        Debug.LogError("There needs to be one active EventManger script on a GameObject in your scene.");
                    }
                    else
                    {
                        eventManager.Init();
                    }
                }

                return eventManager;
            }
        }

        void Init()
        {
            if(eventDictionary == null)
            {
                eventDictionary = new Dictionary<string, ParamsEvent>();
                lastEventValues = new Dictionary<string, ParamsObject>();
            }
        }

        public static void StartListening(string eventName, UnityAction<ParamsObject> listener)
        {
            ParamsEvent thisEvent = null;
            if(instance.eventDictionary.TryGetValue(eventName, out thisEvent))
            {
                thisEvent.AddListener(listener);
            }
            else
            {
                thisEvent = new ParamsEvent();
                thisEvent.AddListener(listener);
                instance.eventDictionary.Add(eventName, thisEvent);
            }

            ParamsObject paramsObj = null;
            if(instance.lastEventValues != null && instance.lastEventValues.TryGetValue(eventName, out paramsObj))
            {
                if(paramsObj != null)
                {
                    listener.Invoke(paramsObj);
                }
            }
        }

        public static void StopListening(string eventName, UnityAction<ParamsObject> listener)
        {
            if(eventManager == null) return;
            ParamsEvent thisEvent = null;
            if(instance.eventDictionary.TryGetValue(eventName, out thisEvent))
            {
                thisEvent.RemoveListener(listener);
            }
        }

        public static void TriggerEvent(string eventName, ParamsObject paramsObj = null)
        {
            ParamsEvent thisEvent = null;
            if(instance.eventDictionary.TryGetValue(eventName, out thisEvent))
            {
                thisEvent.Invoke(paramsObj);
            }

            if(instance.lastEventValues != null)
            {
                instance.lastEventValues[eventName] = paramsObj;
            }
        }
    }
}
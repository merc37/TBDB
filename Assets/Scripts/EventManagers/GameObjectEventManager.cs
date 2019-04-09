using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
//Three kinds; update where new values are passed with expectation for more, send where one value is implied but not guranteed
//and just events who are their own descriptor
//Basically messages are certain local events upgraded to global so that other objects can hook into them
//So messengers upgrade to global; not done directly, to allow local hooking as "priority"
//Yeah globalevents is better cuz thats basically what i made them
//Exceptions are Rigidbodies, colliders and transforms which are assumed to be easy local access
//Radius events bypass this and call other objects event managers directly

namespace EventManagers
{
    public class GameObjectEventManager : MonoBehaviour
    {
        private Dictionary<string, ParamsEvent> eventDictionary;
        private Dictionary<string, ParamsObject> lastEventValues;

        void Awake()
        {
            if(eventDictionary == null)
            {
                eventDictionary = new Dictionary<string, ParamsEvent>();
                lastEventValues = new Dictionary<string, ParamsObject>();
            }
        }

        public void StartListening(string eventName, UnityAction<ParamsObject> listener)
        {
            if(eventDictionary == null)
            {
                eventDictionary = new Dictionary<string, ParamsEvent>();
                lastEventValues = new Dictionary<string, ParamsObject>();
            }

            ParamsEvent thisEvent = null;

            if(eventDictionary.TryGetValue(eventName, out thisEvent))
            {
                thisEvent.AddListener(listener);
            }
            else
            {
                thisEvent = new ParamsEvent();
                thisEvent.AddListener(listener);
                eventDictionary.Add(eventName, thisEvent);
            }

            ParamsObject paramsObj = null;
            if(lastEventValues != null && lastEventValues.TryGetValue(eventName, out paramsObj))
            {
                if(paramsObj != null)
                {
                    listener.Invoke(paramsObj);
                }
            }
        }

        public void StopListening(string eventName, UnityAction<ParamsObject> listener)
        {
            ParamsEvent thisEvent = null;
            if(eventDictionary.TryGetValue(eventName, out thisEvent))
            {
                thisEvent.RemoveListener(listener);
            }
        }

        public void TriggerEvent(string eventName, ParamsObject paramsObj = null)
        {
            if(eventDictionary == null)
            {
                eventDictionary = new Dictionary<string, ParamsEvent>();
                lastEventValues = new Dictionary<string, ParamsObject>();
            }

            ParamsEvent thisEvent = null;
            if(eventDictionary.TryGetValue(eventName, out thisEvent))
            {
                thisEvent.Invoke(paramsObj);
                lastEventValues[eventName] = paramsObj;
            }
        }

        public static void TriggerRadiusEvent(string eventName, Vector2 origin, float radius, LayerMask layerMask, ParamsObject paramsObj = null, Collider2D ignoreCollider = null)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(origin, radius, layerMask);
            GameObjectEventManager eventManager;
            foreach(Collider2D hit in hits)
            {
                if(ignoreCollider != null && hit.Equals(ignoreCollider))
                {
                    continue;
                }
                eventManager = hit.GetComponent<GameObjectEventManager>();
                if(eventManager != null)
                {
                    eventManager.TriggerEvent(eventName, paramsObj);
                }
            }
        }
    }
}

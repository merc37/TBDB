using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameObjectEventManager : MonoBehaviour {
	
	private Dictionary <string, UnityEvent> eventDictionary;

	void Awake ()
	{
		if (eventDictionary == null)
		{
			eventDictionary = new Dictionary<string, UnityEvent>();
		}
	}

	public void StartListening (string eventName, UnityAction listener)
	{
		UnityEvent thisEvent = null;
		if (eventDictionary.TryGetValue (eventName, out thisEvent))
		{
			thisEvent.AddListener (listener);
		} 
		else
		{
			thisEvent = new UnityEvent ();
			thisEvent.AddListener (listener);
			eventDictionary.Add (eventName, thisEvent);
		}
	}

	public void StopListening (string eventName, UnityAction listener)
	{
		UnityEvent thisEvent = null;
		if (eventDictionary.TryGetValue (eventName, out thisEvent))
		{
			thisEvent.RemoveListener (listener);
		}
	}

	public void TriggerEvent (string eventName)
	{
		UnityEvent thisEvent = null;
		if (eventDictionary.TryGetValue (eventName, out thisEvent))
		{
			thisEvent.Invoke();
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Ammo : MonoBehaviour {

	[SerializeField]
	private GameObjectEventManager eventManager;

	[SerializeField]
	private int maxAmmo;
	public int MaxAmount {get{return maxAmmo;} set{maxAmmo = value;}}

	private int ammo;
	public int CurrentAmount {
		get{return ammo;}
		set{
			if(value < 0) {
				ammo = 0;
			}
			if(value > maxAmmo) {
				ammo = maxAmmo;
			}

			ammo = value;
		}
	}

	void Start () {
		eventManager.StartListening("CheckAmmoAndShoot", new UnityAction(CheckAmmoAndShoot));
		CurrentAmount = MaxAmount;
	}

	private void CheckAmmoAndShoot() {
		if(CurrentAmount > 0) {
			CurrentAmount--;
			eventManager.TriggerEvent("OnShoot");
		}
	}
}

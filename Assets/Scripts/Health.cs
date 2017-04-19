using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

	[SerializeField]
	private int maxHealth;
	public int MaxAmount {get{return maxHealth;} set{maxHealth = value;}}

	private int health;
	public int CurrentAmount {
		get{return health;}
		set{
			if(value < 0) {
				health = 0;
			}
			if(value > maxHealth) {
				health = maxHealth;
			}

			health = value;
		}
	}

	// Use this for initialization
	void Start () {
		CurrentAmount = MaxAmount;
	}
}

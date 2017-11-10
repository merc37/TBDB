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

    public void Decrease(int amount) {
        CurrentAmount -= amount;
    }

    public void Increase(int amount) {
        CurrentAmount += amount;
    }

    public void SetMax() {
        CurrentAmount = MaxAmount;
    }

	// Use this for initialization
	void Start () {
		CurrentAmount = MaxAmount;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ProjectileOwner : MonoBehaviour {
	
	private GameObject owner;
	public GameObject Owner {
		get{return owner;}
		set{owner = value;}
	}
}

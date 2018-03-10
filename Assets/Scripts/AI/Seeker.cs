using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seeker : MonoBehaviour {
	
	[SerializeField]
	private Transform target;

	private EnemyMovement movement;

	void Awake() {
		movement = GetComponent<EnemyMovement>();
	}

	void Start() {
		
	}

	// Update is called once per frame
	void Update() {
		if (Input.GetAxis("DebugInteract") > 0) movement.SetTarget(target.position);
	}
}

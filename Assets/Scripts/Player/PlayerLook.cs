using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate() {
		// Look
		Vector3 distToMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
		float angle = Mathf.Atan2(distToMousePosition.x, distToMousePosition.y) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis(-angle, Vector3.forward);
	}
}

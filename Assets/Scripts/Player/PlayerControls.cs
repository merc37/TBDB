using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour {

    public float walkSpeed;
    Rigidbody2D RB;

	// Use this for initialization
	void Start () {
        RB = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // Use for physics updates
    void FixedUpdate() {
        // Movement
		RB.AddForce(Input.GetAxis("VerticalMovement") * walkSpeed * transform.up);
		RB.AddForce(Input.GetAxis("HorizontalMovement") * walkSpeed * transform.right);
    }
}

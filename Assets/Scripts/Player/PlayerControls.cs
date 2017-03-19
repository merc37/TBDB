using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour {

    public float speed;
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
        RB.AddForce((Input.GetKey(KeyCode.W) ? 1 : 0) * speed * transform.up);
        RB.AddForce((Input.GetKey(KeyCode.A) ? -1 : 0) * speed * transform.right);
        RB.AddForce((Input.GetKey(KeyCode.S) ? -1 : 0) * speed * transform.up);
        RB.AddForce((Input.GetKey(KeyCode.D) ? 1 : 0) * speed * transform.right);
    }
}

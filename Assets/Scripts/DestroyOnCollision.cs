using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnCollision : MonoBehaviour {

	void OnCollisionEnter2D(Collision2D coll) {
		if(coll.gameObject.tag == "Block"){
			Destroy(this.gameObject);
		}
	}

	void OnTriggerEnter2D(Collider2D coll) {
		if(coll.gameObject.tag == "Block"){
			Destroy(this.gameObject);
		}
	}
}

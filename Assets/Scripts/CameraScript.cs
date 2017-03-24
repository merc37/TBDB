using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

	public Transform player;
	public Transform map;

	private Vector3 offset;

	// Use this for initialization
	void Start () {
		offset = transform.position - player.position;
	}
	
	// LateUpdate is called once per frame, but after all other updates
	void LateUpdate () {
		transform.position = player.position + offset;
		if(transform.position.x < map.position.x) {
			transform.position = new Vector3(map.position.x, transform.position.y, transform.position.z);
		}
		if(transform.position.y < map.position.y) {
			transform.position = new Vector3(transform.position.x, map.position.y, transform.position.z);
		}
	}
}

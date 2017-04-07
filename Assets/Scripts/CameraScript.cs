using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

	public Transform player;
	public Transform map;

	private Vector3 newCameraPosition;

	// Use this for initialization
	void Start () {
		newCameraPosition = new Vector3(0, 0, 0);
	}
	
	// LateUpdate is called once per frame, but after all other updates
	void LateUpdate () {
		newCameraPosition.Set(player.position.x, player.position.y, -10);
		transform.position = newCameraPosition;
	}
}

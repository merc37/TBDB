using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour {

	public float slowMotionPercentage;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		// Slow Motion
		if(Input.GetButtonDown("SlowMotion")) {
			Time.timeScale = Time.timeScale == 1f ? (slowMotionPercentage/100) : 1f;
		}
	}
}

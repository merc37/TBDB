using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour {

	public Rigidbody2D laserShot;
	public float laserShotSpeed;
	public float slowMotionPercentage;

	private Rigidbody2D lastFired;

	// Use this for initialization
	void Start () {
		lastFired = null;
	}
	
	// Update is called once per frame
	void Update () {
		// Shoot
		if(Input.GetButtonDown("Fire")) {
			Rigidbody2D newLaserShot = (Rigidbody2D) Instantiate(laserShot, transform.position, transform.rotation);
			newLaserShot.velocity = (GetComponentInChildren<PlayerLook>().transform.rotation * transform.up) * laserShotSpeed;
			lastFired = newLaserShot;
		}

		// Sword

		// Slow Motion
		if(Input.GetButtonDown("SlowMotion")) {
			Time.timeScale = Time.timeScale == 1f ? (slowMotionPercentage/100) : 1f;
		}
	}

	void FixedUpdate() {
		//Curve Projectiles
		if(lastFired != null) {
			float degree = Input.GetAxis("CurveProjectile");
			if(degree != 0) {
				float cos = Mathf.Cos(-degree * Mathf.Deg2Rad);
				float sin = Mathf.Sin(-degree * Mathf.Deg2Rad);
				float vx = lastFired.velocity.x;
				float vy = lastFired.velocity.y;
				lastFired.velocity = new Vector2(vx*cos - vy*sin, vx*sin + vy*cos);
			}
		}
	}
}

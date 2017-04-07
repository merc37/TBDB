using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour {

	public Rigidbody2D laserShot;
	public float laserShotSpeed;
	public int maxAmmo;

	private Rigidbody2D lastFired;
	private int currAmmo;

	// Use this for initialization
	void Start () {
		lastFired = null;
		currAmmo = maxAmmo;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Fire")) {
			if(currAmmo > 0) {
				Rigidbody2D newLaserShot = (Rigidbody2D) Instantiate(laserShot, transform.position, transform.rotation);
				newLaserShot.velocity = (GetComponentInChildren<PlayerLook>().transform.rotation * transform.up) * laserShotSpeed;
				lastFired = newLaserShot;
				currAmmo--;
			}
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour {

	public Rigidbody2D laserShot;
	public float laserShotSpeed;
	public int maxAmmo;
    public float maxBulletAngle;
    public float curveDeadzone;

    private Rigidbody2D lastFired;
	private int currAmmo;
    private Transform playerTransform;

	// Use this for initialization
	void Start () {
        lastFired = null;
        curveDeadzone *= Mathf.Deg2Rad;
        currAmmo = maxAmmo;
        playerTransform = GetComponentInChildren<PlayerLook>().transform;
    }
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Fire")) {
			if(currAmmo > 0) {
                Rigidbody2D newLaserShot = (Rigidbody2D) Instantiate(laserShot, playerTransform.rotation * ( new Vector3(0.25f, 0.25f, -1) ) + (new Vector3(transform.position.x, transform.position.y, 0)), playerTransform.rotation);
				newLaserShot.velocity = (playerTransform.rotation * transform.up) * laserShotSpeed;
				lastFired = newLaserShot;
				currAmmo--;
            }
		}
	}

	void FixedUpdate() {
        //Curve Projectiles
        if (lastFired != null && Time.timeScale != 1) {
            //float degree = Input.GetAxis("CurveProjectile");
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 lineToMouse = new Vector2(mousePosition.x - lastFired.position.x, mousePosition.y - lastFired.position.y);
            float SgtAngle = Mathf.Acos(Vector2.Dot(lastFired.velocity, lineToMouse) / (lastFired.velocity.magnitude * lineToMouse.magnitude));
            float angleSign = Mathf.Sign(lastFired.velocity.x * (mousePosition.y - lastFired.position.y) - ((mousePosition.x - lastFired.position.x) * lastFired.velocity.y));
            print(lastFired.velocity);
            if (SgtAngle > curveDeadzone) {
                SgtAngle = Mathf.Min(Mathf.Abs(SgtAngle), Mathf.Abs(maxBulletAngle)) * angleSign;
                float cos = Mathf.Cos(SgtAngle);
                float sin = Mathf.Sin(SgtAngle);
                float vx = lastFired.velocity.x;
                float vy = lastFired.velocity.y;
                lastFired.velocity = new Vector2(vx * cos - vy * sin, vx * sin + vy * cos);
            }
        }
    }
}

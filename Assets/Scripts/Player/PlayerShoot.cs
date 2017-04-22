using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Player {
	public class PlayerShoot : MonoBehaviour {

		[SerializeField]
		private GameObjectEventManager eventManager;

<<<<<<< Updated upstream
		// Update is called once per frame
		void Update () {
			if(Input.GetButtonDown("Fire")) {
				if(eventManager != null) {
					eventManager.TriggerEvent("CheckAmmoAndShoot");
				}
			}
=======
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
                Rigidbody2D newLaserShot = (Rigidbody2D) Instantiate(laserShot, playerTransform.rotation * ( new Vector3(0.3125f, 0.5f, -1) ) + (new Vector3(transform.position.x, transform.position.y, 0)), playerTransform.rotation);
				newLaserShot.velocity = (playerTransform.rotation * transform.up) * laserShotSpeed;
				lastFired = newLaserShot;
				currAmmo--;
            }
>>>>>>> Stashed changes
		}
	}
}
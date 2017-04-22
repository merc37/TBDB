using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShootProjectile : MonoBehaviour {

	[SerializeField]
	private GameObjectEventManager eventManager;
	[SerializeField]
	private Transform spriteTransform;
	[SerializeField]
	private Rigidbody2D projectileToBeFired;
	[SerializeField]
	private float projectileSpeed;

    private Rigidbody2D lastFired;
	public Rigidbody2D LastFired {
		get {
			return lastFired;
		}
	}

	void Start () {
        lastFired = null;
		eventManager.StartListening("OnShoot", new UnityAction(OnShoot));
    }

	private void OnShoot() {
		Rigidbody2D newProjectile = (Rigidbody2D) Instantiate(projectileToBeFired, spriteTransform.rotation * ( new Vector3(0.25f, 0.25f, -1) ) + (new Vector3(transform.position.x, transform.position.y, 0)), spriteTransform.rotation);
		newProjectile.velocity = (spriteTransform.rotation * Vector3.up) * projectileSpeed;
		newProjectile.GetComponent<ProjectileOwner>().Owner = this.gameObject;
		lastFired = newProjectile;
	}
}
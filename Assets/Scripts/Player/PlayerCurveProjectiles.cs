using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player {
	public class PlayerCurveProjectiles : MonoBehaviour {

		[SerializeField]
		private ShootProjectile shootProjectile;
		[SerializeField]
		private float maxBulletAngle;
		[SerializeField]
		private float curveDeadzone;

		private Rigidbody2D lastFired;

		void Start() {
			curveDeadzone *= Mathf.Deg2Rad;
		}

		void FixedUpdate() {
			//Curve Projectiles
			if (Time.timeScale != 1f) {
				lastFired = shootProjectile.LastFired;
				if(lastFired != null) {
					Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					Vector2 lineToMouse = new Vector2(mousePosition.x - lastFired.position.x, mousePosition.y - lastFired.position.y);
					float SgtAngle = Mathf.Acos(Vector2.Dot(lastFired.velocity, lineToMouse) / (lastFired.velocity.magnitude * lineToMouse.magnitude));
					float angleSign = Mathf.Sign(lastFired.velocity.x * (mousePosition.y - lastFired.position.y) - ((mousePosition.x - lastFired.position.x) * lastFired.velocity.y));
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
	}
}
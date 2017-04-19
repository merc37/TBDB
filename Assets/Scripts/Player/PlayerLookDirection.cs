using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player {
	public class PlayerLookDirection : MonoBehaviour {

		private Vector2 distToMousePosition;

		void FixedUpdate() {
			distToMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
			float angle = Mathf.Atan2(distToMousePosition.x, distToMousePosition.y) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.AngleAxis(-angle, Vector3.forward);
		}
	}
}
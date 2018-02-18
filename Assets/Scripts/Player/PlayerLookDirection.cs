using UnityEngine;

namespace Player {
	public class PlayerLookDirection : MonoBehaviour {

		private Vector2 distToMousePosition;
        private new Collider2D collider;

        void Start() {
			collider = GetComponent<Collider2D>();
        }
        
		void FixedUpdate() {
			distToMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - collider.bounds.center;
			float angle = Mathf.Atan2(distToMousePosition.x, distToMousePosition.y) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.AngleAxis(-angle, Vector3.forward);
		}
	}
}
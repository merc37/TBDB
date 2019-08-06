using UnityEngine;

namespace Player
{
    public class PlayerLookDirection : MonoBehaviour
    {

        private Vector2 distToMousePosition;
        private new Collider2D collider;
        private new Rigidbody2D rigidbody;

        void Start()
        {
            collider = GetComponent<Collider2D>();
            rigidbody = GetComponent<Rigidbody2D>();
        }

        void FixedUpdate()
        {
            distToMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - collider.bounds.center;
            rigidbody.rotation = distToMousePosition.ToAngle();
        }

        void OnDrawGizmos()
        {
            if (rigidbody != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(rigidbody.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
        }
    }
}
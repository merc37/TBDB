using UnityEngine;
using System.Linq;

namespace Collisions
{
    public class ReflectOnCollision : MonoBehaviour
    {

        [SerializeField]
        private string[] collisionTags = new string[1];

        [SerializeField]
        private LayerMask mask;

        private Rigidbody2D rb;
        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private Vector3 oldVelocity;
        private void FixedUpdate()
        {
            oldVelocity = rb.velocity;
        }

        void OnCollisionEnter2D(Collision2D coll)
        {
            if (collisionTags.Contains(coll.gameObject.tag))
            {
                Vector3 reflectedVelocity = Vector3.Reflect(oldVelocity, coll.contacts[0].normal);
                rb.velocity = reflectedVelocity;

                Quaternion rotation = Quaternion.FromToRotation(oldVelocity, reflectedVelocity);
                transform.rotation *= rotation;
            }
        }

        void OnTriggerEnter2D(Collider2D coll)
        {
            if (collisionTags.Contains(coll.gameObject.tag))
            {
                RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, oldVelocity, 1, mask);
                Vector2 normal = raycastHit.normal;
                normal.x = Mathf.Round(normal.x);
                normal.y = Mathf.Round(normal.y);
                Vector3 reflectedVelocity = Vector3.Reflect(oldVelocity, normal);
                rb.velocity = reflectedVelocity;

                Quaternion rotation = Quaternion.FromToRotation(oldVelocity, reflectedVelocity);
                transform.rotation *= rotation;
            }
        }
        //triggered = false;
    }
}

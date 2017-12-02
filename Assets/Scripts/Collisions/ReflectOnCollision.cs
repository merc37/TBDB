using UnityEngine;
using System.Linq;

namespace Collisions {
    public class ReflectOnCollision : MonoBehaviour {

        [SerializeField]
        [TagSelector]
        private string[] collisionTags = new string[1];

        private Rigidbody2D rb;
        private void Start() {
            rb = GetComponent<Rigidbody2D>();
        }

        private Vector3 oldVelocity;
        private void FixedUpdate() {
            oldVelocity = rb.velocity;
        }

        void OnCollisionEnter2D(Collision2D coll) {
            if(collisionTags.Contains(coll.gameObject.tag)) {
                Vector3 reflectedVelocity = Vector3.Reflect(oldVelocity, coll.contacts[0].normal);
                rb.velocity = reflectedVelocity;

                Quaternion rotation = Quaternion.FromToRotation(oldVelocity, reflectedVelocity);
                transform.rotation *= rotation;
            }
        }
    }
}

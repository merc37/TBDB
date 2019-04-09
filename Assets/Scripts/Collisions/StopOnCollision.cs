using UnityEngine;
using System.Linq;

namespace Collisions
{
    public class StopOnCollision : MonoBehaviour
    {
        [SerializeField]
        [TagSelector]
        private string[] collisionTags = new string[1];

        private new Rigidbody2D rigidbody;

        void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();
        }

        void OnCollisionEnter2D(Collision2D coll)
        {
            if(rigidbody != null)
            {
                if(collisionTags.Length == 0 || collisionTags.Contains(coll.gameObject.tag))
                {
                    rigidbody.velocity = Vector2.zero;
                }
            }
        }

        void OnTriggerEnter2D(Collider2D coll)
        {
            if(rigidbody != null)
            {
                if(collisionTags.Length == 0 || collisionTags.Contains(coll.gameObject.tag))
                {
                    rigidbody.velocity = Vector2.zero;
                }
            }
        }
    }
}

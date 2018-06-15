using UnityEngine;
using System.Linq;

namespace Collisions {
    public class DestroyOnCollision : MonoBehaviour {

        [SerializeField]
        [TagSelector]
        private string[] collisionTags = new string[1];
        
        void OnCollisionEnter2D(Collision2D coll) {
            if (collisionTags.Contains(coll.gameObject.tag)) {
                Destroy(this.gameObject);
            }
        }
        
        void OnTriggerEnter2D(Collider2D coll) {
            if (collisionTags.Contains(coll.gameObject.tag)) {
                Destroy(this.gameObject);
            }
        }
    }
}


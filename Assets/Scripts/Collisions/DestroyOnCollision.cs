using UnityEngine;
using System.Linq;

namespace Collisions {
    public class DestroyOnCollision : MonoBehaviour {

        [SerializeField]
        [TagSelector]
        private string[] collisionTags = new string[1];

        //void OnCollisionStay2D(Collision2D coll) {
        //    if(collisionTags.Contains(coll.gameObject.tag)) {
        //        Debug.Log("Destroyed position:" + transform.position);
        //        Destroy(this.gameObject);
        //    }
        //}
        void OnCollisionEnter2D(Collision2D coll) {
            if (collisionTags.Contains(coll.gameObject.tag)) {
                Debug.Log("Destroyed position:" + transform.position);
                Destroy(this.gameObject);
            }
        }

        //void OnTriggerStay2D(Collider2D coll) {
        //    if (collisionTags.Contains(coll.gameObject.tag)) {
        //        Destroy(this.gameObject);
        //    }
        //}
        void OnTriggerEnter2D(Collider2D coll) {
            if (collisionTags.Contains(coll.gameObject.tag)) {
                Debug.Log("Destroyed position:" + transform.position);
                //this.gameObject.SendMessage("OnImpact");
                Destroy(this.gameObject);
            }
        }
    }
}


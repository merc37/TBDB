using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectOnCollision : MonoBehaviour {

    [TagSelector]
    public string CollisionTag = "";

    private Rigidbody2D rb;
    private void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    private Vector3 oldVelocity;
    private void FixedUpdate() {
        oldVelocity = rb.velocity;
    }

    //void OnCollisionStay2D(Collision2D coll) {
    //    if (coll.gameObject.tag == CollisionTag) {

    //        //Vector3 reflectedVelocity = Vector3.Reflect(oldVelocity, coll.contacts[0].normal);
    //        //rb.velocity = reflectedVelocity;
    //        //rb.velocity *= -1;

    //        //Quaternion rotation = Quaternion.FromToRotation(oldVelocity, reflectedVelocity);
    //        //transform.rotation *= rotation;
    //    }
    //}

    private void OnCollisionEnter2D(Collision2D coll) {
        if (coll.gameObject.tag == CollisionTag) {
            Vector3 reflectedVelocity = Vector3.Reflect(oldVelocity, coll.contacts[0].normal);
            rb.velocity = reflectedVelocity;

            Quaternion rotation = Quaternion.FromToRotation(oldVelocity, reflectedVelocity);
            transform.rotation *= rotation;
        }
    }

    void OnTriggerEnter2D(Collider2D coll) {
        if (coll.gameObject.tag == CollisionTag) {
            //Vector3 reflectedVelocity = Vector3.Reflect(oldVelocity, coll.gameObject);
            //rb.velocity = reflectedVelocity;

            //Quaternion rotation = Quaternion.FromToRotation(oldVelocity, reflectedVelocity);
            //transform.rotation *= rotation;
        }
    }

    //void OnTriggerStay2D(Collider2D coll) {
    //    //if (coll.gameObject.tag == CollisionTag) {
    //    //    Vector3 reflectedVelocity = Vector3.Reflect(oldVelocity, coll.contacts[0].normal);
    //    //    rb.velocity = reflectedVelocity;

    //    //    Quaternion rotation = Quaternion.FromToRotation(oldVelocity, reflectedVelocity);
    //    //    transform.rotation *= rotation;

    //        //Collider2D[] contacts = new Collider2D[] { };
    //        //coll.GetContacts(contacts);
    //        //Vector3 reflectedVelocity = Vector3.Reflect(oldVelocity, contacts[0].`);
    //}
}

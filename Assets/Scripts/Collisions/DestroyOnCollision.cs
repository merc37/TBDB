using UnityEngine;

public class DestroyOnCollision : MonoBehaviour {

    [TagSelector]
    public string CollisionTag = "";

	void OnCollisionStay2D(Collision2D coll) {
		if(coll.gameObject.tag == CollisionTag){
			Destroy(this.gameObject);
		}
	}

	void OnTriggerStay2D(Collider2D coll) {
		if(coll.gameObject.tag == CollisionTag){
			Destroy(this.gameObject);
		}
	}
}

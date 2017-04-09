using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileRotate : MonoBehaviour {

    Rigidbody2D RB;

    // Use this for initialization
    void Start () {
        RB = GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void FixedUpdate() {
        // Look
        // transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.forward);
        // transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z);
        // RB.angularVelocity = 0;

        Vector3 projectileVelocity = RB.velocity;
        float angle = Mathf.Atan2(projectileVelocity.x, projectileVelocity.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(-angle, Vector3.forward);
    }
}

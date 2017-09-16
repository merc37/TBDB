using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeToLive : MonoBehaviour {

    [SerializeField]
    private float TTL;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (TTL <= 0) Destroy(this.gameObject);
        else TTL -= Time.deltaTime;
	}
}

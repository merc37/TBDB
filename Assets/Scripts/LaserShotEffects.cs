using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserShotEffects : MonoBehaviour {

    TrailRenderer trailRenderer;
    SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
        trailRenderer.sortingOrder = spriteRenderer.sortingOrder;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

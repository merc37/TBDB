using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserShotEffects : MonoBehaviour {

    private TrailRenderer trailRenderer;
    private SpriteRenderer spriteRenderer;

	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
        trailRenderer.sortingOrder = spriteRenderer.sortingOrder;
    }
}

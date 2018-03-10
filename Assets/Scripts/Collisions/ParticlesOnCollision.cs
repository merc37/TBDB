using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesOnCollision : MonoBehaviour {

	private SpriteRenderer spriteRenderer;
	private ParticleSystem particleSystem;

	void Awake() {
		spriteRenderer = GetComponent<SpriteRenderer>();
		particleSystem = gameObject.AddComponent<ParticleSystem>() as ParticleSystem;
		particleSystem.Stop();

		var main = particleSystem.main;
		main.duration = 1;
		main.loop = false;
		main.startDelay = 0;
		main.startLifetime = 0.3f;
		// main.startSpeed = ?;
		// main.startSize = ?;

		var emission = particleSystem.emission;
	}

	void OnCollisionEnter2D(Collision2D coll) {

		particleSystem.Play();
	} 

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


}

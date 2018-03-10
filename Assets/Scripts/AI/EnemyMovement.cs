using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyMovement : MonoBehaviour {

	[SerializeField]
	private float walkSpeed;
	
	private BasicThetaStarPathfinding pathfinding;
	private Rigidbody2D rigidbody;
	private Collider2D collider;
	private List<Node> path;
	
	void Awake() {
		pathfinding = GameObject.Find("testMap2").GetComponent<BasicThetaStarPathfinding>();
		rigidbody = GetComponent<Rigidbody2D>();
		collider = GetComponent<Collider2D>();
	}
    
	void Update () {
		if (path != null && path.Count > 0) {
			if (reachedNode(path[0])) path.RemoveAt(0);
			else {
				Vector3 direction = (path[0].worldPosition - transform.position).normalized;
				rigidbody.AddForce(direction * walkSpeed);
			}
		}
	}

	public void SetTarget(Vector3 target) {
		path = pathfinding.FindPath(transform.position, target, collider);
		GameObject.Find("testMap2").GetComponent<Grid>().path = new List<Node>(path);
	}

	public void StopMovement() {
		path = null;
	}

	private float pointAccuracy = 0.5f;
	private bool isAtNode(Node node) {
		float distanceFromNode = Vector2.Distance(transform.position, node.worldPosition);
		if (distanceFromNode <= pointAccuracy) return true;
		else return false;
	}
	
	private bool reachedNode(Node node) {
		if (isAtNode(node)) return true;
		return false;
		if (rigidbody.velocity.magnitude > 0) {
			if (Vector3.Dot(rigidbody.velocity, transform.position - node.worldPosition) < 0) {
				return true;
			}
		}
		return false;
	}
}

using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Panda;
using EventManagers;
using UnityEngine.Events;
using System.Collections;

public class EnemyMovementTasks : MonoBehaviour {

	[SerializeField]
	private float walkSpeed = 10;
    [SerializeField]
    private int recalculatePathDistance = 5;
    [SerializeField]
    private int recalculatePathAngle = 20;

    private GameObjectEventManager eventManager;

    private BasicThetaStarPathfinding pathfinding;
	private new Rigidbody2D rigidbody;
	private new Collider2D collider;
	private List<Node> path;
    private Vector3 targetVector;
	
	void Awake() {
        eventManager = GetComponent<GameObjectEventManager>();
        eventManager.StartListening("ReturnMapTransform", new UnityAction<ParamsObject>(SetPathfinding));
        eventManager.StartListening("SetMovementTarget", new UnityAction<ParamsObject>(SetMovementTarget));
        rigidbody = GetComponent<Rigidbody2D>();
		collider = GetComponent<Collider2D>();
        targetVector = Vector3.negativeInfinity;
        path = new List<Node>();
	}

    [Task]
    //Succeeds on continuing to move toward next node in path, Fails if reached next node or if path not set
	bool MoveTowardNextNode () {
        //Fails if path not set
        if(path.Count < 1) {
            return false;
        }
        //Fails if next node reached
        if(ReachedNode(path[0])) {
            return false;
        }
        //Succeeds if continuing to move toward next node in path
		Vector3 direction = (path[0].worldPosition - transform.position).normalized;
		rigidbody.AddForce(direction * walkSpeed);
        return true;
	}

    [Task]
    //Succeeds if next node set, Fails if no next node to set or target is too far from end of path
    bool SetNextNode() {

        if(path.Count >= 1) {
            path.RemoveAt(0);
            //if(path.Count >= 1) {
            //    Vector3 direction = transform.position - path[0].worldPosition;
            //    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            //    Quaternion q = Quaternion.AngleAxis(angle + 90, Vector3.forward);
            //    transform.rotation = q;
            //}

            return false;
        }

        return true;
    }

    [Task]
    //Succeeds if path gets set, Fails if there is no target to path to, if end of path reached
	bool RecalculatePathToTarget() {
        path = pathfinding.FindPath(transform.position, targetVector, collider);
        Vector3 direction = transform.position - targetVector;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle + 90, Vector3.forward);
        transform.rotation = q;
        GameObject.Find("testMap2").GetComponent<Grid>().path = new List<Node>(path);//Just for gizmos
        return true;
	}

    [Task]
    bool FinalNodeDistanceToTargetOkay() {
        if(path == null || path.Count == 0) {
            return false;
        }
        if(Vector2.Distance(path[path.Count - 1].worldPosition, targetVector) > recalculatePathDistance) {
            return false;
        }
        return true;
    }

    Vector2 p1, p2;

    bool ReachedNode(Node node) {
        if (isAtNode(node)) return true;
		if (rigidbody.velocity.magnitude > 0) {
            p1 = node.worldPosition;
            p2 = node.worldPosition + Vector3.Cross(rigidbody.velocity.normalized, -Vector3.forward).normalized;
            float d = (transform.position.x - p1.x)*(p2.y - p1.y) - (transform.position.y - p1.y)*(p2.x - p1.x);
            return d > 0;
		}
        return false;
	}

    private float pointAccuracy = 0.5f;
    private bool isAtNode(Node node) {
        float distanceFromNode = Vector2.Distance(transform.position, node.worldPosition);
        if(distanceFromNode <= pointAccuracy) return true;
        else return false;
    }

    private void SetMovementTarget(ParamsObject paramsObj) {
        targetVector = paramsObj.Vector3;
    }

    private void SetPathfinding(ParamsObject paramsObj) {
        pathfinding = paramsObj.Transform.GetComponent<BasicThetaStarPathfinding>();
    }

    void OnDrawGizmos() {
        if(p1 != null && p2 != null) {
            Gizmos.DrawLine(p1, p2);
        }
    }
}

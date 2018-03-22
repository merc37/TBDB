using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Panda;
using EventManagers;
using UnityEngine.Events;

public class EnemyMovementTasks : MonoBehaviour {

	[SerializeField]
	private float walkSpeed;
    [SerializeField]
    private int recalculatePathDistance = 5;

    private GameObjectEventManager eventManager;

    private BasicThetaStarPathfinding pathfinding;
	private Rigidbody2D rigidbody;
	private Collider2D collider;
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
        if(path.Count > 0 && Vector3.Distance(path[path.Count - 1].worldPosition, targetVector) > recalculatePathDistance) {
            return false;
        }
        if(path.Count >= 1) {
            path.RemoveAt(0);
            return true;
        }
        return false;
    }

    [Task]
    //Succeeds if path gets set, Fails if there is no target to path to, if end of path reached
	bool RecalculatePathToTarget() {
        path = pathfinding.FindPath(transform.position, targetVector, collider);
        //GameObject.Find("testMap2").GetComponent<Grid>().path = new List<Node>(path);//Just for gizmos
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

    bool ReachedNode(Node node) {
        if (isAtNode(node)) return true;
        return false;
		if (rigidbody.velocity.magnitude > 0) {
			if (Vector3.Dot(rigidbody.velocity, transform.position - node.worldPosition) < 0) {
                return true;
			}
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
}

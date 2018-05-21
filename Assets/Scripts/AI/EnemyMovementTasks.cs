using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Panda;
using EventManagers;
using UnityEngine.Events;

public class EnemyMovementTasks : MonoBehaviour {

    [SerializeField]
    private float maxWalkSpeed = 8;
    private float speed;

    [SerializeField]
    private float recalculatePathDistance = 2;

    private GameObjectEventManager eventManager;

    private BasicThetaStarPathfinding pathfinding;
	private new Rigidbody2D rigidbody;
	private new Collider2D collider;
	private List<Node> path;
    private int currentPathIndex;
    private Vector2 targetVector;
	
	void Awake() {
        eventManager = GetComponent<GameObjectEventManager>();
        eventManager.StartListening("ReturnMapTransform", new UnityAction<ParamsObject>(SetPathfinding));
        eventManager.StartListening("SetMovementTarget", new UnityAction<ParamsObject>(SetMovementTarget));
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        
        speed = maxWalkSpeed * rigidbody.drag;
		
        targetVector = Vector3.negativeInfinity;

        currentPathIndex = -1;
	}

    [Task]
	bool MoveTowardNextNode () {
        Vector2 nodeWorldPos = path[currentPathIndex].worldPosition;
        Vector2 direction = (nodeWorldPos - rigidbody.position).normalized;
        rigidbody.AddForce(direction * speed);
        return true;
	}

    [Task]
    bool ReachedFinalNode() {
        return currentPathIndex >= path.Count - 1;
    }

    Vector2 tooFarEndNode, targetPos;
    [Task]
    bool TargetTooFarFromEndOfPath() {
        Vector2 finalNodeWorldPos = path[path.Count - 1].worldPosition;
        tooFarEndNode = finalNodeWorldPos;
        targetPos = targetVector;
        float distanceToTargetFromEndOfPath = Vector2.Distance(finalNodeWorldPos, targetVector);
        return distanceToTargetFromEndOfPath > recalculatePathDistance;
    }

    [Task]
    //Succeeds when next node set
    bool SetNextNode() {
        currentPathIndex++;
        if(currentPathIndex < path.Count - 1) {
            Vector3 direction = transform.position - path[currentPathIndex].worldPosition;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion q = Quaternion.AngleAxis(angle + 90, Vector3.forward);
            transform.rotation = q;
        }

        return true;
    }

    [Task]
	bool RecalculatePathToTarget() {
        path = pathfinding.FindPath(transform.position, targetVector, collider);
        currentPathIndex = 0;
        GameObject.Find("testMap2").GetComponent<Grid>().path = new List<Node>(path);//Just for gizmos
        return true;
	}

    [Task]
    bool ReachedNode() {
        Node node = path[currentPathIndex];
        if (isAtNode(node)) return true;
		if (rigidbody.velocity.magnitude > 0) {
            Vector2 p1 = node.worldPosition;
            Vector2 p2 = node.worldPosition + Vector3.Cross(rigidbody.velocity.normalized, -Vector3.forward).normalized;
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
        targetVector = paramsObj.Vector2;
    }

    private void SetPathfinding(ParamsObject paramsObj) {
        pathfinding = paramsObj.Transform.GetComponent<BasicThetaStarPathfinding>();
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(tooFarEndNode, targetPos);
    }
}

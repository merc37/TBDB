using EventManagers;
using UnityEngine;
using UnityEngine.Events;
using Panda;
using System.Collections.Generic;
using Pathfinding;

public class BasicEnemyTasks : MonoBehaviour {

    [SerializeField]
    private float sightAngle = 35;
    [SerializeField]
    private float sightDistance = 10;
    [SerializeField]
    private LayerMask sightBlockMask;
    [SerializeField]
    private float attackRange = 7;
    [SerializeField]
    private int lowAmmoThreshold = 0;
    [SerializeField]
    private float maxWalkSpeed = 10;
    private float speed;
    [SerializeField]
    private float recalculatePathDistance = 2;

    private GameObjectEventManager eventManager;
    private Rigidbody2D playerRigidbody;
    private new Rigidbody2D rigidbody;
    private new Collider2D collider;
    private BasicThetaStarPathfinding pathfinding;
    private List<Node> path;
    private Vector2 movementTarget;
    private bool lowOnAmmo = false;
    private bool hasZeroHealth = false;

    private Vector2 addForce = Vector2.zero;

    void Awake() {

        eventManager = GetComponent<GameObjectEventManager>();
        eventManager.StartListening("ReturnPlayerRigidbody", new UnityAction<ParamsObject>(SetPlayerRigidbody));
        eventManager.StartListening("ReturnMapTransform", new UnityAction<ParamsObject>(SetPathfinding));
        eventManager.StartListening("AmmoCount", new UnityAction<ParamsObject>(CheckLowAmmo));
        eventManager.StartListening("HealthPoints", new UnityAction<ParamsObject>(CheckZeroHealth));

        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();

        speed = maxWalkSpeed * rigidbody.drag;
    }

    void FixedUpdate() {
        rigidbody.AddForce(addForce);
    }

    [Task]
    bool SecurePinging() {
        addForce = Vector2.zero;
        return true;
    }

    [Task]
    bool Test() {
        Vector2 direction = (movementTarget - rigidbody.position).normalized;
        rigidbody.AddForce(direction * speed);
        return true;
    }

    [Task]
    bool Reload() {
        eventManager.TriggerEvent("OnReload");
        return true;
    }

    [Task]
    bool HasZeroHealth() {
        return hasZeroHealth;
    }

    [Task]
    bool SetMovementTargetToCover() {
        return false;
    }

    [Task]
    bool LowOnAmmo() {
        return lowOnAmmo;
    }

    [Task]
    bool DestroySelf() {
        Destroy(this);
        return true;
    }

    [Task]
    bool PlayerTooFarFromEndOfPath() {
        Vector2 finalNodeWorldPos = path[path.Count - 1].worldPosition;
        float distanceToTargetFromEndOfPath = Vector2.Distance(finalNodeWorldPos, playerRigidbody.position);
        return distanceToTargetFromEndOfPath > recalculatePathDistance;
    }

    [Task]
    bool MoveAlongPath() {
        if(ReachedNode(path[0])) {
            path.RemoveAt(0);
        }

        if(path.Count <= 0) {
            return false;
        }

        Vector2 nodeWorldPos = path[0].worldPosition;
        Vector2 direction = (nodeWorldPos - rigidbody.position).normalized;
        //rigidbody.AddForce(direction * speed);
        addForce = direction * speed;

        return true;
    }

    [Task]
    bool SetMovementTargetToPlayer() {
        movementTarget = playerRigidbody.position;
        return true;
    }

    [Task]
    bool RecalculatePathToMovementTarget() {
        path = pathfinding.FindPath(transform.position, movementTarget, collider);

        if(path == null) {
            return false;
        }

        if(path.Count <= 0) {
            return false;
        }

        Vector2 direction = rigidbody.position - movementTarget;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle + 90, Vector3.forward);
        transform.rotation = q;

        GameObject.Find("testMap2").GetComponent<Grid>().path = new List<Node>(path);//Just for gizmos
        return true;
    }

    [Task]
    bool Shoot() {
        eventManager.TriggerEvent("OnShoot");
        return true;
    }

    [Task]
    bool PlayerInAttackRange() {
        float distanceToTarget = Vector2.Distance(rigidbody.position, playerRigidbody.position);
        return distanceToTarget <= attackRange;
    }

    [Task]
    bool PlayerInSight() {
        float angle = Vector3.Angle(playerRigidbody.position - rigidbody.position, transform.up);
        if(angle <= sightAngle) {
            float distance = Vector3.Distance(rigidbody.position, playerRigidbody.position);
            if(distance <= sightDistance) {
                RaycastHit2D raycastHit = Physics2D.Linecast(rigidbody.position, playerRigidbody.position, sightBlockMask);
                if(!raycastHit) {
                    return true;
                }
            }
        }
        return false;
    }

    bool ReachedNode(Node node) {
        if(isAtNode(node)) return true;
        if(rigidbody.velocity.magnitude > 0) {
            Vector2 p1 = node.worldPosition;
            Vector2 p2 = node.worldPosition + Vector3.Cross(rigidbody.velocity.normalized, -Vector3.forward).normalized;
            float d = (transform.position.x - p1.x) * (p2.y - p1.y) - (transform.position.y - p1.y) * (p2.x - p1.x);
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

    private void CheckLowAmmo(ParamsObject paramsObj) {
        lowOnAmmo = paramsObj.Int <= lowAmmoThreshold;
    }

    private void CheckZeroHealth(ParamsObject paramsObj) {
        hasZeroHealth = paramsObj.Float <= 0;
    }

    private void SetPlayerRigidbody(ParamsObject paramsObj) {
        playerRigidbody = paramsObj.Rigidbody;
        Debug.Log(playerRigidbody);
        //eventManager.StopListening("ReturnPlayerRigidbody", new UnityAction<ParamsObject>(SetPlayerRigidbody));
    }

    private void SetPathfinding(ParamsObject paramsObj) {
        pathfinding = paramsObj.Transform.GetComponent<BasicThetaStarPathfinding>();
        //eventManager.StopListening("ReturnMapTransform", new UnityAction<ParamsObject>(SetPathfinding));
    }

    void OnDrawGizmos() {
        Quaternion q1 = Quaternion.AngleAxis(-sightAngle, Vector3.forward);
        Quaternion q2 = Quaternion.AngleAxis(sightAngle, Vector3.forward);
        Gizmos.DrawRay(transform.position, q1 * transform.up * sightDistance);
        Gizmos.DrawRay(transform.position, q2 * transform.up * sightDistance);
    }
}

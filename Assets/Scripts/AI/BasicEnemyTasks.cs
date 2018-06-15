using EventManagers;
using UnityEngine;
using UnityEngine.Events;
using Panda;
using System.Collections.Generic;
using Pathfinding;

public class BasicEnemyTasks : MonoBehaviour {

    private static readonly Vector2 NullVector = new Vector2(float.MinValue, float.MinValue);

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
    private int lowHealthThreshold = 3;
    [SerializeField]
    private float maxWalkSpeed = 10;
    private float speed;
    [SerializeField]
    private float recalculatePathDistance = 2;
    [SerializeField]
    private float rollCooldownTime = 5;
    [SerializeField]
    private float rollForce = 10;

    private GameObjectEventManager eventManager;
    private Rigidbody2D playerRigidbody;
    private new Rigidbody2D rigidbody;
    private new Collider2D collider;
    private BasicThetaStarPathfinding pathfinding;
    private List<Node> path;
    private Vector2 movementTarget;
    private bool lowOnAmmo;
    private bool lowOnHealth;
    private bool hasZeroHealth;
    private bool shouldRoll;
    private float rollCooldownTimer;
    private Vector2 rollDirection;
    private bool rolling;
    private bool roll;

    private Vector2 addForce = Vector2.zero;
    private Vector2 playerLastKnownPosition = NullVector;

    void Awake() {

        eventManager = GetComponent<GameObjectEventManager>();
        eventManager.StartListening("ReturnPlayerRigidbody", new UnityAction<ParamsObject>(SetPlayerRigidbody));
        eventManager.StartListening("ReturnMapTransform", new UnityAction<ParamsObject>(SetPathfinding));
        eventManager.StartListening("AmmoCount", new UnityAction<ParamsObject>(OnAmmoCountUpdate));
        eventManager.StartListening("HealthPoints", new UnityAction<ParamsObject>(OnHealthPointUpdate));
        eventManager.StartListening("Roll", new UnityAction<ParamsObject>(OnRoll));

        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();

        path = new List<Node>();
        speed = maxWalkSpeed * rigidbody.drag;
        lowOnAmmo = false;
        hasZeroHealth = false;
        rollCooldownTimer = rollCooldownTime;
    }

    void FixedUpdate() {
        rigidbody.AddForce(addForce);
        if(roll) {
            rigidbody.AddForce(rollDirection.normalized * rollForce, ForceMode2D.Impulse);
            roll = false;
        }
    }

    void Update() {
        if(rolling) {
            rollCooldownTimer -= Time.time;
            if(rollCooldownTimer <= 0) {
                rolling = false;
                rollCooldownTimer = rollCooldownTime;
            }
        }
    }

    [Task]
    bool Roll() {
        rolling = true;
        shouldRoll = false;
        roll = true;
        return true;
    }

    [Task]
    bool ShouldRoll() {
        return shouldRoll;
    }

    [Task]
    bool CanRoll() {
        return !rolling;
    }

    [Task]
    bool AimAtPlayer() {
        return true;
    }

    [Task]
    bool LowOnHealth() {
        return lowOnHealth;
    }

    [Task]
    bool SetLookTargetToPlayer() {
        Vector2 direction = rigidbody.position - playerRigidbody.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rigidbody.rotation = angle + 90;
        //Quaternion q = Quaternion.AngleAxis(angle + 90, Vector3.forward);
        //transform.rotation = q;
        return true;
    }

    [Task]
    bool SetLookTargetToMovementDirection() {
        if(path.Count > 0) {
            Vector2 nodeWorldPos = path[0].worldPosition;
            Vector2 direction = rigidbody.position - nodeWorldPos;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            rigidbody.rotation = angle + 90;
            //Quaternion q = Quaternion.AngleAxis(angle + 90, Vector3.forward);
            //transform.rotation = q;
        }
        return true;
    }

    [Task]
    bool UnSetPlayerLastKnownPosition() {
        playerLastKnownPosition = NullVector;
        return true;
    }

    [Task]
    bool SetMovementTargetToPlayerLastKnownPosition() {
        movementTarget = playerLastKnownPosition;
        return true;
    }

    [Task]
    bool PlayerLastPositionKnown() {
        return playerLastKnownPosition != NullVector;
    }

    [Task]
    bool MoveTowardsMovementTarget() {

        //If path is empty or the target is too far from the end of it, set it
        if(path.Count == 0 || Vector2.Distance(path[path.Count - 1].worldPosition, movementTarget) > recalculatePathDistance) {
            path = pathfinding.FindPath(transform.position, movementTarget, collider);
        }

        //If path still empty there is no route to the target and this should return TODO: make sure pathfinding does not return null
        if(path.Count == 0) {
            return false;
        }

        GameObject.Find("testMap2").GetComponent<Grid>().path = new List<Node>(path);//Just for gizmos


        if(ReachedNode(path[0])) {
            path.RemoveAt(0);
        }

        //reached end of path
        if(path.Count == 0) {
            return false;
        }

        Vector2 nodeWorldPos = path[0].worldPosition;
        Vector2 direction = (nodeWorldPos - rigidbody.position).normalized;
        addForce = direction * speed;

        return true;
    }

    [Task]
    bool StopMovement() {
        addForce = Vector2.zero;
        path = new List<Node>();
        return true;
    }

    [Task]
    bool Stop() {
        addForce = Vector2.zero;
        rigidbody.velocity = Vector2.zero;
        path = new List<Node>();
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
        Destroy(gameObject);
        return true;
    }

    [Task]
    bool SetMovementTargetToPlayerPosition() {
        movementTarget = playerRigidbody.position;
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
        return IsInSight(rigidbody.position, playerRigidbody.position);
    }

    bool IsInSight(Vector2 from, Vector2 to) {
        float angle = Vector2.Angle(to - from, transform.up);
        if(angle <= sightAngle) {
            float distance = Vector2.Distance(from, to);
            if(distance <= sightDistance) {
                RaycastHit2D raycastHit = Physics2D.Linecast(from, to, sightBlockMask);
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

    private void OnAmmoCountUpdate(ParamsObject paramsObj) {
        lowOnAmmo = paramsObj.Int <= lowAmmoThreshold;
    }

    private void OnHealthPointUpdate(ParamsObject paramsObj) {
        lowOnHealth = paramsObj.Int <= lowHealthThreshold;
        hasZeroHealth = paramsObj.Int <= 0;
    }

    private void OnRoll(ParamsObject paramsObj) {
        shouldRoll = true;
        rollDirection = Vector3.Cross(paramsObj.Vector2, Vector3.forward);
    }

    private void SetPlayerRigidbody(ParamsObject paramsObj) {
        playerRigidbody = paramsObj.Rigidbody;
        eventManager.StopListening("ReturnPlayerRigidbody", new UnityAction<ParamsObject>(SetPlayerRigidbody));
    }

    private void SetPathfinding(ParamsObject paramsObj) {
        pathfinding = paramsObj.Transform.GetComponent<BasicThetaStarPathfinding>();
        eventManager.StopListening("ReturnMapTransform", new UnityAction<ParamsObject>(SetPathfinding));
    }

    void OnDrawGizmos() {
        Quaternion q1 = Quaternion.AngleAxis(-sightAngle, Vector3.forward);
        Quaternion q2 = Quaternion.AngleAxis(sightAngle, Vector3.forward);
        Gizmos.DrawRay(transform.position, q1 * transform.up * sightDistance);
        Gizmos.DrawRay(transform.position, q2 * transform.up * sightDistance);
    }
}

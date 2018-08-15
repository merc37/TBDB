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
	[SerializeField]
	private float maxPathSearchDistance = 20;
    [SerializeField]
    private float rotateSpeed = 5;

    private GameObjectEventManager eventManager;
    private Rigidbody2D playerRigidbody;
    private new Rigidbody2D rigidbody;
    private new Collider2D collider;
    private BasicThetaStarPathfinding pathfinding;
    private Grid grid;
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
    private bool coverFound;

    private Vector2 addForce = Vector2.zero;
    private float lookTarget = -90;
    private Vector2 playerLastKnownPosition = NullVector;
    private Vector2 playerLastKnownHeading = NullVector;

    private float gunProjectileSpeed;
    private Transform gunTransform;

    void Awake() {

        eventManager = GetComponent<GameObjectEventManager>();
        eventManager.StartListening("ReturnPlayerRigidbody", new UnityAction<ParamsObject>(SetPlayerRigidbody));
        eventManager.StartListening("ReturnMapTransform", new UnityAction<ParamsObject>(SetPathfinding));
        eventManager.StartListening("AmmoCount", new UnityAction<ParamsObject>(OnAmmoCountUpdate));
        eventManager.StartListening("HealthPoints", new UnityAction<ParamsObject>(OnHealthPointUpdate));
        eventManager.StartListening("Roll", new UnityAction<ParamsObject>(OnRoll));
        eventManager.StartListening("UpdateGunInfo", new UnityAction<ParamsObject>(OnGunInfoUpdate));

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
        if(lookTarget != float.NegativeInfinity) {
            rigidbody.rotation = Mathf.LerpAngle(rigidbody.rotation, lookTarget, rotateSpeed * Time.fixedDeltaTime);
            if(Mathf.Abs(Mathf.DeltaAngle(rigidbody.rotation, lookTarget)) <= 1) {
                lookTarget = float.NegativeInfinity;
            }
        }

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
    bool SetCoverFound() {
        coverFound = true;
        return true;
    }

    [Task]
    bool FoundCover() {
        return coverFound;
    }

    [Task]
    bool PlayerRunningAway() {
        bool isAcute = Vector2.Dot(rigidbody.velocity, playerRigidbody.velocity) > 0;
        bool playerRunning = playerRigidbody.velocity.magnitude >= rigidbody.velocity.magnitude;
        return isAcute && playerRunning;
    }

    [Task]
    bool SetLookTargetRandom() {
        lookTarget = Random.Range(-360, 360);
        return true;
    }

    [Task]
    bool SetLookTargetToPlayerLastKnownHeading() {
        if(playerLastKnownHeading != NullVector) {
            float angle = Mathf.Atan2(playerLastKnownHeading.y, playerLastKnownHeading.x) * Mathf.Rad2Deg;
            lookTarget = angle + 90;
            return true;
        }
        return false;
    }

    [Task]
    bool UnSetPlayerLastKnownHeading() {
        playerLastKnownHeading = NullVector;
        return true;
    }
    
    [Task]
    bool SetPlayerLastKnownHeading() {
        Vector2 direction = playerRigidbody.position.normalized - playerRigidbody.velocity.normalized;
        playerLastKnownHeading = direction;
        return true;
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
        float timeguess1 = Vector2.Distance(rigidbody.position, playerRigidbody.position) / gunProjectileSpeed;
        Vector2 Positionguess =(playerRigidbody.position + (playerRigidbody.velocity * (timeguess1)));
        float timeguess = Vector2.Distance(rigidbody.position, Positionguess) / gunProjectileSpeed;
        Vector2 playerguesstimation = playerRigidbody.position + (playerRigidbody.velocity * (timeguess));
        Vector2 direction = rigidbody.position - playerguesstimation;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        rigidbody.rotation = angle + 90;

        return true;
    }

    [Task]
    bool AimAtPlayer() {
        float timeguess1 = Vector2.Distance(rigidbody.position, playerRigidbody.position) / gunProjectileSpeed;
        Vector2 Positionguess =(playerRigidbody.position + (playerRigidbody.velocity * (timeguess1)));
        float timeguess = Vector2.Distance(rigidbody.position, Positionguess) / gunProjectileSpeed;
        Vector2 playerguesstimation = playerRigidbody.position + (playerRigidbody.velocity * (timeguess));
        aimDireaction = rigidbody.position - playerguesstimation;
        float angle = Mathf.Atan2(aimDireaction.y, aimDireaction.x) * Mathf.Rad2Deg;

        rigidbody.rotation = angle + 90;

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
        lookTarget = angle + 90;
        return true;
    }

    [Task]
    bool SetLookTargetToMovementDirection() {
        if(path != null && path.Count > 0) {
            Vector2 nodeWorldPos = path[0].worldPosition;
            Vector2 direction = rigidbody.position - nodeWorldPos;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            lookTarget = angle + 90;
        }
        return true;
    }
    
    [Task]
    bool SetPlayerLastKnownPosition() {
        playerLastKnownPosition = playerRigidbody.position;
        return true;
    }

    [Task]
    bool UnSetPlayerLastKnownPosition() {
        playerLastKnownPosition = NullVector;
        return true;
    }

    [Task]
    bool SetMovementTargetToPlayerLastKnownPosition() {
        if(playerLastKnownPosition != NullVector) {
            movementTarget = playerLastKnownPosition;
            return true;
        }
        return false;
    }

    [Task]
    bool MoveTowardsMovementTarget() {
        Vector2 dir = (movementTarget - rigidbody.position).normalized;
        addForce = dir * speed;
        return true;
    }

    [Task]
    bool PathToMovementTarget() {

        //If path is empty or the target is too far from the end of it, set it
        if(path == null || path.Count == 0) {
            path = pathfinding.FindPath(transform.position, movementTarget, collider, maxPathSearchDistance);
        } else if(Vector2.Distance(path[path.Count - 1].worldPosition, movementTarget) > recalculatePathDistance/* && ReachedNode(path[0])*/) {
            path = pathfinding.FindPath(transform.position, movementTarget, collider, maxPathSearchDistance);
        }

        //If path still empty there is no route to the target and this should return TODO: make sure pathfinding does not return null
        if(path == null) {
            return false;
        }

        GameObject.Find("testMap2").GetComponent<Grid>().path = new List<Node>(path);//Just for gizmos

        if(path.Count != 0 && ReachedNode(path[0])) {
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
        if(path != null) {
            path.Clear();
        }
        return true;
    }

    [Task]
    bool Stop() {
        addForce = Vector2.zero;
        rigidbody.velocity = Vector2.zero;
        if(path != null) {
            path.Clear();
        }
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
    bool InCover() {
        return !Physics2D.Linecast(playerRigidbody.position, rigidbody.position);
    }

    List<Node> potentialCoverNodes = new List<Node>();
    Vector2 bestCoverPoint;
    bool returnFalse;
    [Task]
    bool SetMovementTargetToCover() {

        potentialCoverNodes = new List<Node>();
        HashSet<Vector2> inProcessQueue = new HashSet<Vector2>();
        Queue<Node> nodesToProcess = new Queue<Node>();
        grid.resetGrid();
        Node node = grid.NodeFromWorldPoint(rigidbody.position);
        node.parent = null;
        nodesToProcess.Enqueue(node);
        inProcessQueue.Add(node.worldPosition);

        while(Vector2.Distance(rigidbody.position, node.worldPosition) < 20) {
            node = nodesToProcess.Dequeue();
            Quaternion left = Quaternion.AngleAxis(-7, Vector3.forward);
            Quaternion right = Quaternion.AngleAxis(7, Vector3.forward);
            RaycastHit2D leftRaycastHit = Physics2D.Linecast(playerRigidbody.position, left * node.worldPosition, sightBlockMask);
            RaycastHit2D rightRaycastHit = Physics2D.Linecast(playerRigidbody.position, right * node.worldPosition, sightBlockMask);
            if(leftRaycastHit && rightRaycastHit) {
                potentialCoverNodes.Add(node);
            }
            foreach(Node neighbor in grid.GetNeighbors(node)) {
                if(!inProcessQueue.Contains(neighbor.worldPosition)) {
                    if(neighbor.isWalkable) {
                        neighbor.parent = node;
                        nodesToProcess.Enqueue(neighbor);
                        inProcessQueue.Add(neighbor.worldPosition);
                    }
                }
            }
        }

        int nodeCount = 0;
        int smallestNodeCount = int.MaxValue;
        Node currNode;
        bestCoverPoint = NullVector;
        foreach(Node coverPoint in potentialCoverNodes) {
            currNode = coverPoint;
            nodeCount = 0;
            while(currNode != null) {
                currNode = currNode.parent;
                nodeCount++;
            }

            if(nodeCount < smallestNodeCount) {
                smallestNodeCount = nodeCount;
                bestCoverPoint = coverPoint.worldPosition;
            }
        }

        if(bestCoverPoint != NullVector) {
            movementTarget = bestCoverPoint;
            return true;
        }
        
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
    bool PlayerInMedAttackRange() {
        float distanceToTarget = Vector2.Distance(rigidbody.position, playerRigidbody.position);
        return distanceToTarget <= attackRange/4;
    }

    [Task]
    bool PlayerInAttackRange() {
        float distanceToTarget = Vector2.Distance(rigidbody.position, playerRigidbody.position);
        return distanceToTarget <= attackRange;
    }

    [Task]
    bool PlayerInSight() {
        float angle = Vector2.Angle(playerRigidbody.position - rigidbody.position, transform.up);
        if(angle <= sightAngle) {
            float distance = Vector2.Distance(rigidbody.position, playerRigidbody.position);
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
            Vector2 p2 = Vector3.Cross(rigidbody.velocity.normalized, -Vector3.forward).normalized;
            Vector2 nodeWorldPosition = node.worldPosition;
            p2 = p2 + nodeWorldPosition;
            float d = (transform.position.x - p1.x) * (p2.y - p1.y) - (transform.position.y - p1.y) * (p2.x - p1.x);
            return d > 0;
        }
        return false;
    }

    private float pointAccuracy = 0.1f;
    private bool isAtNode(Node node) {
        float distanceFromNode = Vector2.Distance(rigidbody.position, node.worldPosition);
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
        if(Random.value > .5f) {
            rollDirection = -rollDirection;
        }
    }
    
    private void OnGunInfoUpdate(ParamsObject paramsObj) {
        gunProjectileSpeed = paramsObj.Float;
        gunTransform = paramsObj.Transform;
    }

    private void SetPlayerRigidbody(ParamsObject paramsObj) {
        playerRigidbody = paramsObj.Rigidbody;
        eventManager.StopListening("ReturnPlayerRigidbody", new UnityAction<ParamsObject>(SetPlayerRigidbody));
    }

    private void SetPathfinding(ParamsObject paramsObj) {
        pathfinding = paramsObj.Transform.GetComponent<BasicThetaStarPathfinding>();
        grid = paramsObj.Transform.GetComponent<Grid>();
        eventManager.StopListening("ReturnMapTransform", new UnityAction<ParamsObject>(SetPathfinding));
    }

    void OnDrawGizmos() {
        Quaternion q1 = Quaternion.AngleAxis(-sightAngle, Vector3.forward);
        Quaternion q2 = Quaternion.AngleAxis(sightAngle, Vector3.forward);
        Gizmos.DrawRay(transform.position, q1 * transform.up * sightDistance);
        Gizmos.DrawRay(transform.position, q2 * transform.up * sightDistance);
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(playerLastKnownPosition, playerLastKnownHeading.normalized * -20);
        Gizmos.color = Color.red;
        Gizmos.DrawCube(playerLastKnownPosition, new Vector3(.3f, .3f, 1));
    }
}

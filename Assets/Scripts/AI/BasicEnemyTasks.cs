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
    private float moveToAngle = -90;
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
        if(moveToAngle != float.NegativeInfinity) {
            rigidbody.rotation = Mathf.LerpAngle(rigidbody.rotation, moveToAngle, rotateSpeed * Time.fixedDeltaTime);
            if(Mathf.DeltaAngle(rigidbody.rotation, moveToAngle) <= 1) {
                moveToAngle = float.NegativeInfinity;
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
    bool PlayerRunningAway() {
        bool isAcute = Vector2.Dot(rigidbody.velocity, playerRigidbody.velocity) > 0;
        bool playerRunning = playerRigidbody.velocity.magnitude >= rigidbody.velocity.magnitude;
        return isAcute && playerRunning;
    }

    [Task]
    bool SetMoveToAngleRandom() {
        moveToAngle = Random.Range(-360, 360);
        return true;
    }

    [Task]
    bool SetMoveToAngleToPlayerLastKnownHeading() {
        float angle = Mathf.Atan2(playerLastKnownHeading.y, playerLastKnownHeading.x) * Mathf.Rad2Deg;
        moveToAngle = angle + 90;
        return true;
    }

    [Task]
    bool UnSetPlayerLastKnownHeading() {
        playerLastKnownHeading = NullVector;
        return true;
    }

    [Task]
    bool PlayerLastHeadingKnown() {
        return playerLastKnownHeading != NullVector;
    }

    Vector2 seePlayerLastKnownHeading;
    [Task]
    bool SetPlayerLastKnownHeading() {
        Vector2 direction = playerRigidbody.position.normalized - playerRigidbody.velocity.normalized;
        playerLastKnownHeading = direction;
        seePlayerLastKnownHeading = playerLastKnownHeading;
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

    Vector2 pointToAimAt;
    [Task]
    bool AimAtPlayer() {
        float playerSpeed = playerRigidbody.velocity.magnitude;

        float projectileSpeed = gunProjectileSpeed;//Needs to be adjusted to reflect enemy motion
        Vector2 projectileSpawn = gunTransform.GetChild(0).position;

        float distanceBetweenProjectileAndPlayer = Vector2.Distance(projectileSpawn, playerRigidbody.position);
        float angleBetweenPlayerVelocityAndProjectile = Vector2.Angle(playerRigidbody.position - playerRigidbody.velocity, playerRigidbody.position - projectileSpawn);

        float x = 1 - (2 * distanceBetweenProjectileAndPlayer * Mathf.Cos(angleBetweenPlayerVelocityAndProjectile));
        float y = (projectileSpeed * projectileSpeed) - ((playerSpeed * playerSpeed) * x);
        float tSqrd = (distanceBetweenProjectileAndPlayer * distanceBetweenProjectileAndPlayer) / y;
        if(tSqrd < 0) {
            return true;//Return false?
        }

        float t = Mathf.Sqrt(tSqrd);
        pointToAimAt = playerRigidbody.position + (t * playerRigidbody.velocity);
        Vector2 direction = rigidbody.position - pointToAimAt;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
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
        moveToAngle = angle + 90;
        return true;
    }

    [Task]
    bool SetLookTargetToMovementDirection() {
        if(path.Count > 0) {
            Vector2 nodeWorldPos = path[0].worldPosition;
            Vector2 direction = rigidbody.position - nodeWorldPos;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            moveToAngle = angle + 90;
        }
        return true;
    }

    Vector2 seePlayerLastKnownPosition;
    [Task]
    bool SetPlayerLastKnownPosition() {
        playerLastKnownPosition = playerRigidbody.position;
        seePlayerLastKnownPosition = playerLastKnownPosition;
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
        path.Clear();
        return true;
    }

    [Task]
    bool Stop() {
        addForce = Vector2.zero;
        rigidbody.velocity = Vector2.zero;
        path.Clear();
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
    
    List<Vector2> potentialCoverPoints = new List<Vector2>();
    Vector2 bestCoverPoint;
    [Task]
    bool SetMovementTargetToCover() {
        return false;

        potentialCoverPoints = new List<Vector2>();
        float searchRadius = 3;
        int samples = 10;
        Vector2 point;
        List<Node> coverPath;
        int x = 0;
        while(potentialCoverPoints.Count < (samples * 2)) {
            for(int i = 0; i < samples; i++) {
                point = rigidbody.position + (Random.insideUnitCircle * searchRadius);
                RaycastHit2D pointNotInSightOfPlayer = Physics2D.Linecast(playerRigidbody.position, point, sightBlockMask);
                if(pointNotInSightOfPlayer) {
                    //RaycastHit2D pointBlocked = Physics2D.Linecast(point, point, sightBlockMask);
                    potentialCoverPoints.Add(point);
                }
            }
            searchRadius += 2;
        }

        float biggestDifference = float.MinValue;
        float diff;
        bestCoverPoint = NullVector;
        foreach(Vector2 coverPoint in potentialCoverPoints) {
            coverPath = pathfinding.FindPath(rigidbody.position, coverPoint, collider, 5);
            if(coverPath != null) {
                diff = Vector2.Distance(coverPoint, playerRigidbody.position) - Vector2.Distance(coverPoint, rigidbody.position);
                if(diff > biggestDifference) {
                    bestCoverPoint = coverPoint;
                }
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
        return distanceToTarget <= attackRange/2;
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
            Vector2 p2 = node.worldPosition + Vector3.Cross(rigidbody.velocity.normalized, -Vector3.forward).normalized;
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
        eventManager.StopListening("ReturnMapTransform", new UnityAction<ParamsObject>(SetPathfinding));
    }

    void OnDrawGizmos() {
        Quaternion q1 = Quaternion.AngleAxis(-sightAngle, Vector3.forward);
        Quaternion q2 = Quaternion.AngleAxis(sightAngle, Vector3.forward);
        Gizmos.DrawRay(transform.position, q1 * transform.up * sightDistance);
        Gizmos.DrawRay(transform.position, q2 * transform.up * sightDistance);
        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(seePlayerLastKnownPosition, new Vector3(.3f, .3f, 1));
        Gizmos.color = Color.red;
        Gizmos.DrawRay(seePlayerLastKnownPosition, seePlayerLastKnownHeading * 5);
        //for(int i = 0; i < potentialCoverPoints.Count; i++) {
        //    Gizmos.DrawCube(potentialCoverPoints[i], new Vector3(.1f, .1f, 1));
        //}
        //Gizmos.color = Color.red;
        //Gizmos.DrawCube(bestCoverPoint, new Vector3(.5f, .5f, 1));
    }
}

using Panda;
using UnityEngine;
using UnityEngine.Events;
using EventManagers;

public class EnemySightTasks : MonoBehaviour {

    [SerializeField]
    private float sightAngle = 35;
    [SerializeField]
    private float sightDistance = 10;
    [SerializeField]
    private LayerMask mask;

    private GameObjectEventManager eventManager;
    private Rigidbody2D playerRigidbody;
    private new Rigidbody2D rigidbody;

    void Awake() {
        eventManager = GetComponent<GameObjectEventManager>();
        eventManager.StartListening("ReturnPlayerTransform", new UnityAction<ParamsObject>(SetPlayerTransform));
        rigidbody = GetComponent<Rigidbody2D>();
    }

    [Task]
    bool PlayerInSight() {
        float angle = Vector3.Angle(playerRigidbody.position - rigidbody.position, transform.up);
        if(angle <= sightAngle) {
            float distance = Vector3.Distance(rigidbody.position, playerRigidbody.position);
            if(distance <= sightDistance) {
                RaycastHit2D raycastHit = Physics2D.Linecast(rigidbody.position, playerRigidbody.position, mask);
                if(!raycastHit) {
                    return true;
                }
            }
        }
        return false;
    }

    [Task]
    bool SetMovementTargetToPlayer() {
        eventManager.TriggerEvent("SetMovementTarget", new ParamsObject(playerRigidbody.position));
        return true;
    }

    [Task]
    bool SetAttackTargetToPlayer() {
        eventManager.TriggerEvent("SetAttackTarget", new ParamsObject(playerRigidbody.position));
        return true;
    }

    [Task]
    bool SetLookTargetToPlayer() {
        Vector3 direction = rigidbody.position - playerRigidbody.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle + 90, Vector3.forward);
        transform.rotation = q;
        return false;
    }

    private void SetPlayerTransform(ParamsObject paramsObj) {
        playerRigidbody = paramsObj.Transform.GetComponent<Rigidbody2D>();
    }

    void OnDrawGizmos() {
        Quaternion q1 = Quaternion.AngleAxis(-sightAngle, Vector3.forward);
        Quaternion q2 = Quaternion.AngleAxis(sightAngle, Vector3.forward);
        Gizmos.DrawRay(transform.position, q1 * transform.up * sightDistance);
        Gizmos.DrawRay(transform.position, q2 * transform.up * sightDistance);
    }
}

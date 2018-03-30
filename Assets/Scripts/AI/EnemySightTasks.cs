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
    private Transform playerTransform;

    void Awake() {
        eventManager = GetComponent<GameObjectEventManager>();
        eventManager.StartListening("ReturnPlayerTransform", new UnityAction<ParamsObject>(SetPlayerTransform));
    }

    [Task]
    bool PlayerInSight() {
        Vector3 lastKnownPlayerLocation = playerTransform.position;
        float angle = Vector3.Angle(playerTransform.position - transform.position, transform.up);
        if(angle <= sightAngle) {
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            if(distance <= sightDistance) {
                RaycastHit2D raycastHit = Physics2D.Linecast(transform.position, playerTransform.position, mask);
                if(!raycastHit) {
                    return true;
                }
            }
        }
        return false;
    }

    [Task]
    bool SetMovementTargetToPlayer() {
        eventManager.TriggerEvent("SetMovementTarget", new ParamsObject(playerTransform.position));
        return true;
    }

    [Task]
    bool SetLookTargetToPlayer() {
        Vector3 direction = transform.position - playerTransform.position;
        transform.rotation = Quaternion.AngleAxis(Vector3.Angle(direction, transform.up), Vector3.forward);
        return false;
    }

    private void SetPlayerTransform(ParamsObject paramsObj) {
        playerTransform = paramsObj.Transform;
    }

    void OnDrawGizmos() {
        Quaternion q1 = Quaternion.AngleAxis(-sightAngle, Vector3.forward);
        Quaternion q2 = Quaternion.AngleAxis(sightAngle, Vector3.forward);
        Gizmos.DrawRay(transform.position, q1 * transform.up * sightDistance);
        Gizmos.DrawRay(transform.position, q2 * transform.up * sightDistance);
    }
}

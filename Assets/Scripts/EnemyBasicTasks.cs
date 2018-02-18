using UnityEngine;
using Panda;
using EventManagers;

public class Enemy : MonoBehaviour {

    [SerializeField]
    private Transform player;
    [SerializeField]
    private float maxSightDistance = 10;
    [SerializeField]
    private float maxSightAngle = 70;
    [SerializeField]
    private float attackRange = 10;

    private GameObjectEventManager eventManager;
    private Rigidbody2D rigidBody;

    void Start() {
        eventManager = GetComponent<GameObjectEventManager>();
        rigidBody = GetComponent<Rigidbody2D>();
    }

    void OnDrawGizmos() {
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, 0, maxSightAngle) * transform.up * maxSightDistance);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, 0, -maxSightAngle) * transform.up * maxSightDistance);
        Gizmos.DrawRay(transform.position, transform.up * maxSightDistance / 2);
    }

    [Task]
    public void IsPlayerInSight() {

        Vector3 positionDifferenece = transform.position - player.position;
        float distToPlayer = positionDifferenece.magnitude;

        if(distToPlayer <= maxSightDistance) {
            float angleToPlayer = Mathf.Abs(Vector3.Angle(transform.up, positionDifferenece) - 180);

            if(angleToPlayer <= maxSightAngle) {

                RaycastHit2D checkBlocked = Physics2D.Linecast(transform.position, player.position, LayerMask.NameToLayer("Blocks"));

                if(!checkBlocked) {
                    Task.current.Succeed();
                }
            }
        }

        Task.current.Fail();
    }

    private bool isPlayerInRange() {

        Vector3 positionDifferenece = transform.position - player.position;
        float distToPlayer = positionDifferenece.magnitude;

        if(distToPlayer <= attackRange) {
            return true;
        }

        return false;
    }
}

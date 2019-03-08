using EventManagers;
using Panda;
using UnityEngine;
using UnityEngine.Events;
using Events;

namespace Enemy
{
    public class SightTasks : MonoBehaviour
    {
        private static readonly Vector2 NullVector = Constants.NullVector;

        [SerializeField]
        private float sightAngle = 35;
        [SerializeField]
        private float sightDistance = 10;
        [SerializeField]
        private LayerMask sightBlockMask;

        private bool playerInSight;

        private int previousFrameCount;

        private GameObjectEventManager eventManager;
        private Rigidbody2D playerRigidbody;
        private new Rigidbody2D rigidbody;

        private UnityAction<ParamsObject> onPlayerSendRigidbodyUnityAction;

        void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();

            eventManager = GetComponent<GameObjectEventManager>();
            onPlayerSendRigidbodyUnityAction = new UnityAction<ParamsObject>(OnPlayerSendRigidbody);
            eventManager.StartListening(EnemyEvents.OnPlayerSendRigidbody, onPlayerSendRigidbodyUnityAction);
        }

        void Start()
        {
            eventManager.TriggerEvent(EnemyEvents.OnSendSightAngle, new ParamsObject(sightAngle));
            eventManager.TriggerEvent(EnemyEvents.OnSendSightDistance, new ParamsObject(sightDistance));
            eventManager.TriggerEvent(EnemyEvents.OnSendSightBlockMask, new ParamsObject(sightBlockMask));
        }

        [Task]
        bool PlayerInSight()
        {
            if(Time.frameCount == previousFrameCount)
            {
                return playerInSight;
            }

            previousFrameCount = Time.frameCount;

            float angle = Vector2.Angle(playerRigidbody.position - rigidbody.position, transform.up);
            if(angle <= sightAngle)
            {
                float distance = Vector2.Distance(rigidbody.position, playerRigidbody.position);
                if(distance <= sightDistance)
                {
                    RaycastHit2D raycastHit = Physics2D.Linecast(rigidbody.position, playerRigidbody.position, sightBlockMask);
                    if(!raycastHit)
                    {
                        playerInSight = true;
                        return playerInSight;
                    }
                }
            }
            playerInSight = false;
            return playerInSight;
        }

        [Task]
        bool SetRotationRandom()
        {
            rigidbody.rotation = Random.Range(0, 360);
            return true;
        }

        private void OnPlayerSendRigidbody(ParamsObject paramsObj)
        {
            playerRigidbody = paramsObj.Rigidbody;
            eventManager.StopListening(EnemyEvents.OnPlayerSendRigidbody, onPlayerSendRigidbodyUnityAction);
        }

        void OnDrawGizmos()
        {
            Quaternion q1 = Quaternion.AngleAxis(-sightAngle, Vector3.forward);
            Quaternion q2 = Quaternion.AngleAxis(sightAngle, Vector3.forward);
            Gizmos.DrawRay(transform.position, q1 * transform.up * sightDistance);
            Gizmos.DrawRay(transform.position, q2 * transform.up * sightDistance);
        }
    }
}

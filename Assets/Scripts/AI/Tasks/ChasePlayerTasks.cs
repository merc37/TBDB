using EventManagers;
using Panda;
using UnityEngine;
using UnityEngine.Events;
using Events;

namespace Enemy
{
    public class ChasePlayerTasks : MonoBehaviour
    {
        private static readonly Vector2 NullVector = Constants.NullVector;

        private Vector2 playerLastKnownPosition = NullVector;
        private Vector2 playerLastKnownHeading = NullVector;

        private bool beenToPlayerLastKnownPosition;
        private bool playerLastKnownPositionInSight;

        private int previousFrameCount;

        private float sightAngle;
        private float sightDistance;

        private LayerMask sightBlockMask;

        private GameObjectEventManager eventManager;
        private Rigidbody2D playerRigidbody;
        private new Rigidbody2D rigidbody;

        private UnityAction<ParamsObject> onPlayerSendRigidbodyUnityAction;
        private UnityAction<ParamsObject> onSendSightAngleUnityAction;
        private UnityAction<ParamsObject> onSendSightDistanceUnityAction;
        private UnityAction<ParamsObject> onSendSightBlockMaskUnityAction;

        void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();

            eventManager = GetComponent<GameObjectEventManager>();
            onPlayerSendRigidbodyUnityAction = new UnityAction<ParamsObject>(OnPlayerSendRigidbody);
            eventManager.StartListening(EnemyEvents.OnPlayerSendRigidbody, onPlayerSendRigidbodyUnityAction);
            eventManager.StartListening(EnemyEvents.OnSetPlayerLastKnownLocation, new UnityAction<ParamsObject>(OnSetPlayerLastKnownLocation));
            eventManager.StartListening(EnemyEvents.OnSetPlayerLastKnownHeading, new UnityAction<ParamsObject>(OnSetPlayerLastKnownHeading));
            onSendSightAngleUnityAction = new UnityAction<ParamsObject>(OnSendSightAngle);
            eventManager.StartListening(EnemyEvents.OnSendSightAngle, onSendSightAngleUnityAction);
            onSendSightDistanceUnityAction = new UnityAction<ParamsObject>(OnSendSightDistance);
            eventManager.StartListening(EnemyEvents.OnSendSightDistance, onSendSightDistanceUnityAction);
            onSendSightBlockMaskUnityAction = new UnityAction<ParamsObject>(OnSendSightBlockMask);
            eventManager.StartListening(EnemyEvents.OnSendSightBlockMask, onSendSightBlockMaskUnityAction);
        }

        [Task]
        bool SetMovementTargetToPlayerPosition()
        {
            eventManager.TriggerEvent(EnemyEvents.OnSetMovementTarget, new ParamsObject(playerRigidbody.position));
            return true;
        }

        [Task]
        bool SetRotationToPlayer()
        {
            Vector2 direction = playerRigidbody.position - rigidbody.position;
            rigidbody.rotation = direction.AngleFromZero();
            return true;
        }

        [Task]
        bool SetRotationToPlayerLastKnownPosition()
        {
            Vector2 direction = playerLastKnownPosition - rigidbody.position;
            rigidbody.rotation = direction.AngleFromZero();
            return true;
        }

        [Task]
        bool SetMovementTargetToPlayerLastKnownPosition()
        {
            eventManager.TriggerEvent(EnemyEvents.OnSetMovementTarget, new ParamsObject(playerLastKnownPosition));
            return true;
        }

        [Task]
        bool SetPlayerLastKnownPosition()
        {
            eventManager.TriggerEvent(EnemyEvents.OnSetPlayerLastKnownLocation, new ParamsObject(playerRigidbody.position));
            return true;
        }

        [Task]
        bool IsPlayerLastPositionKnown()
        {
            return playerLastKnownPosition != NullVector;
        }

        [Task]
        bool UnSetPlayerLastKnownPosition()
        {
            playerLastKnownPosition = NullVector;
            return true;
        }

        [Task]
        bool PlayerLastKnownPositionLineOfSight()
        {
            if(Time.frameCount == previousFrameCount)
            {
                return playerLastKnownPositionInSight;
            }

            previousFrameCount = Time.frameCount;

            float angle = Vector2.Angle(playerLastKnownPosition - rigidbody.position, transform.up);
            if(angle <= sightAngle)
            {
                float distance = Vector2.Distance(rigidbody.position, playerLastKnownPosition);
                if(distance <= sightDistance)
                {
                    RaycastHit2D raycastHit = Physics2D.Linecast(rigidbody.position, playerLastKnownPosition, sightBlockMask);
                    if(!raycastHit)
                    {
                        playerLastKnownPositionInSight = true;
                        return playerLastKnownPositionInSight;
                    }
                }
            }
            playerLastKnownPositionInSight = false;
            return playerLastKnownPositionInSight;
        }

        [Task]
        bool SetRotationToPlayerLastKnownHeading()
        {
            rigidbody.rotation = playerLastKnownHeading.AngleFromZero();
            return true;
        }

        [Task]
        bool SetPlayerLastKnownHeading()
        {
            eventManager.TriggerEvent(EnemyEvents.OnSetPlayerLastKnownHeading, new ParamsObject(playerRigidbody.velocity));
            return true;
        }

        [Task]
        bool IsPlayerLastHeadingKnown()
        {
            return playerLastKnownHeading != NullVector;
        }

        [Task]
        bool UnSetPlayerLastKnownHeading()
        {
            playerLastKnownHeading = NullVector;
            return true;
        }

        [Task]
        bool BeenToPlayerLastKnownPosition()
        {
            return beenToPlayerLastKnownPosition;
        }

        [Task]
        bool SetBeenToPlayerLastKnownPosition()
        {
            beenToPlayerLastKnownPosition = true;
            return true;
        }

        [Task]
        bool IsPlayerFacing()
        {
            float angle = Vector2.Angle(rigidbody.position - playerRigidbody.position, playerRigidbody.transform.up);
            if(angle <= sightAngle)
            {
                return true;
            }
            return false;
        }

        [Task]
        bool IsPlayerRunningAway()
        {
            float distance1 = Vector2.Distance(playerRigidbody.position, rigidbody.position);
            float distance2 = Vector2.Distance(playerRigidbody.position + playerRigidbody.velocity.normalized, rigidbody.position);

            if(distance2 > distance1)
            {
                return true;
            }
            return false;
        }

        private void OnPlayerSendRigidbody(ParamsObject paramsObj)
        {
            playerRigidbody = paramsObj.Rigidbody;
            eventManager.StopListening(EnemyEvents.OnPlayerSendRigidbody, onPlayerSendRigidbodyUnityAction);
        }

        private void OnSetPlayerLastKnownLocation(ParamsObject paramsObj)
        {
            playerLastKnownPosition = paramsObj.Vector2;
            beenToPlayerLastKnownPosition = false;
        }

        private void OnSetPlayerLastKnownHeading(ParamsObject paramsObj)
        {
            playerLastKnownHeading = paramsObj.Vector2;
        }

        private void OnSendSightAngle(ParamsObject paramsObj)
        {
            sightAngle = paramsObj.Float;
            eventManager.StopListening(EnemyEvents.OnSendSightAngle, onSendSightAngleUnityAction);
        }

        private void OnSendSightDistance(ParamsObject paramsObj)
        {
            sightDistance = paramsObj.Float;
            eventManager.StopListening(EnemyEvents.OnSendSightDistance, onSendSightDistanceUnityAction);
        }

        private void OnSendSightBlockMask(ParamsObject paramsObj)
        {
            sightBlockMask = paramsObj.LayerMask;
            eventManager.StopListening(EnemyEvents.OnSendSightBlockMask, onSendSightBlockMaskUnityAction);
        }

        void OnDrawGizmos()
        {
            if(IsPlayerLastPositionKnown())
            {
                Gizmos.color = Color.red;
                Gizmos.DrawCube(playerLastKnownPosition, new Vector3(.3f, .3f, .3f));
            }
        }
    }
}

using EventManagers;
using Panda;
using UnityEngine;
using UnityEngine.Events;

namespace Enemy
{
    public class ChasePlayerTasks : MonoBehaviour
    {
        private static readonly Vector2 NullVector = Constants.NullVector;

        private Vector2 playerLastKnownPosition = NullVector;
        private Vector2 playerLastKnownHeading = NullVector;

        private GameObjectEventManager eventManager;
        private Rigidbody2D playerRigidbody;
        private new Rigidbody2D rigidbody;

        void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();

            eventManager = GetComponent<GameObjectEventManager>();
            eventManager.StartListening("ReturnPlayerRigidbody", new UnityAction<ParamsObject>(SetPlayerRigidbody));
            eventManager.StartListening("OnSetPlayerLastKnownPosition", new UnityAction<ParamsObject>(SetPlayerLastKnownPosition));
            eventManager.StartListening("OnSetPlayerLastKnownHeading", new UnityAction<ParamsObject>(SetPlayerLastKnownHeading));
        }

        [Task]
        bool SetMovementTargetToPlayerPosition()
        {
            eventManager.TriggerEvent("OnSetMovementTarget", new ParamsObject(playerRigidbody.position));
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
        bool SetMovementTargetToPlayerLastKnownPosition()
        {
            eventManager.TriggerEvent("OnSetMovementTarget", new ParamsObject(playerLastKnownPosition));
            return true;
        }

        [Task]
        bool SetPlayerLastKnownPosition()
        {
            playerLastKnownPosition = playerRigidbody.position;
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
        bool SetRotationToPlayerLastKnownHeading()
        {
            rigidbody.rotation = playerLastKnownHeading.AngleFromZero();
            return true;
        }

        [Task]
        bool SetPlayerLastKnownHeading()
        {
            Vector2 direction = playerRigidbody.velocity.normalized - playerRigidbody.position.normalized;
            playerLastKnownHeading = direction;
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

        private void SetPlayerRigidbody(ParamsObject paramsObj)
        {
            playerRigidbody = paramsObj.Rigidbody;
            eventManager.StopListening("ReturnPlayerRigidbody", new UnityAction<ParamsObject>(SetPlayerRigidbody));
        }

        private void SetPlayerLastKnownPosition(ParamsObject paramsObj)
        {
            playerLastKnownPosition = paramsObj.Vector2;
        }

        private void SetPlayerLastKnownHeading(ParamsObject paramsObj)
        {
            playerLastKnownHeading = paramsObj.Vector2;
        }
    }
}

using EventManagers;
using Panda;
using UnityEngine;
using UnityEngine.Events;
using Events;

namespace Enemy
{
    public class StrafeTasks : MonoBehaviour
    {
        [SerializeField]
        private float strafePointDistance = 3;

        private Vector2 strafeCenter;

        private GameObjectEventManager eventManager;
        private new Rigidbody2D rigidbody;
        private Rigidbody2D playerRigidbody;

        private UnityAction<ParamsObject> onPlayerSendRigidbodyUnityAction;

        void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();

            eventManager = GetComponent<GameObjectEventManager>();
            onPlayerSendRigidbodyUnityAction = new UnityAction<ParamsObject>(OnPlayerSendRigidbody);
            eventManager.StartListening(EnemyEvents.OnPlayerSendRigidbody, onPlayerSendRigidbodyUnityAction);
        }

        [Task]
        bool SetStrafeCenterAtCurrentPosition()
        {
            strafeCenter = rigidbody.position;
            return true;
        }

        [Task]
        bool SetMovementTargetToStrafePoint()
        {
            float angle = Random.Range(0, Mathf.PI * 2);
            Vector2 strafePos = strafeCenter + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * strafePointDistance;
            eventManager.TriggerEvent(EnemyEvents.OnSetMovementTarget, new ParamsObject(strafePos));

            return true;
        }

        private void OnPlayerSendRigidbody(ParamsObject paramsObj)
        {
            playerRigidbody = paramsObj.Rigidbody;
            eventManager.StopListening(EnemyEvents.OnPlayerSendRigidbody, onPlayerSendRigidbodyUnityAction);
        }
    }
}

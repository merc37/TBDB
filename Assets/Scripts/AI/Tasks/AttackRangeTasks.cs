using EventManagers;
using Panda;
using UnityEngine;
using UnityEngine.Events;
using Events;

namespace Enemy
{
    public class AttackRangeTasks : MonoBehaviour
    {
        [SerializeField]
        private float attackRange = 7;

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

        [Task]
        bool PlayerInAttackRange()
        {
            float distanceToTarget = Vector2.Distance(rigidbody.position, playerRigidbody.position);
            return distanceToTarget <= attackRange;
        }

        [Task]
        bool PlayerInMediumAttackRange()
        {
            float distanceToTarget = Vector2.Distance(rigidbody.position, playerRigidbody.position);
            return distanceToTarget <= attackRange / 2;
        }

        private void OnPlayerSendRigidbody(ParamsObject paramsObj)
        {
            playerRigidbody = paramsObj.Rigidbody;
            eventManager.StopListening(EnemyEvents.OnPlayerSendRigidbody, onPlayerSendRigidbodyUnityAction);
        }
    }
}

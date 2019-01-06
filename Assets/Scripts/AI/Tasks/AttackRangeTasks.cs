using EventManagers;
using Panda;
using UnityEngine;
using UnityEngine.Events;

namespace Enemy
{
    public class AttackRangeTasks : MonoBehaviour
    {
        [SerializeField]
        private float attackRange = 7;

        private GameObjectEventManager eventManager;
        private Rigidbody2D playerRigidbody;
        private new Rigidbody2D rigidbody;

        void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();

            eventManager = GetComponent<GameObjectEventManager>();
            eventManager.StartListening("ReturnPlayerRigidbody", new UnityAction<ParamsObject>(SetPlayerRigidbody));
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

        private void SetPlayerRigidbody(ParamsObject paramsObj)
        {
            playerRigidbody = paramsObj.Rigidbody;
            eventManager.StopListening("ReturnPlayerRigidbody", new UnityAction<ParamsObject>(SetPlayerRigidbody));
        }
    }
}

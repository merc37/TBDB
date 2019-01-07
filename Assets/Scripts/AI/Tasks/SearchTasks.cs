using EventManagers;
using Panda;
using Pathfinding;
using UnityEngine;
using UnityEngine.Events;
using Events;

namespace Enemy
{
    public class SearchTasks : MonoBehaviour
    {
        [SerializeField]
        private float searchTime = 7;
        [SerializeField]
        private float searchRadius = 8.5f;

        private Pathfinding.Grid grid;

        private float searchTimer;

        private Vector2 searchDirection;
        private Vector2 searchCenter;
        private Vector2 currentSearchPoint;

        private GameObjectEventManager eventManager;
        private new Rigidbody2D rigidbody;

        private UnityAction<ParamsObject> onMapSendTransformUnityAction;

        void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();

            eventManager = GetComponent<GameObjectEventManager>();
            onMapSendTransformUnityAction = new UnityAction<ParamsObject>(OnMapSendTransform);
            eventManager.StartListening(EnemyEvents.OnMapSendTransform, onMapSendTransformUnityAction);

            searchTimer = searchTime;
        }

        [Task]
        bool SetMovementTargetToSearchPoint()
        {
            Node walkableNode = grid.NodeFromWorldPoint(searchCenter + (Random.insideUnitCircle * searchRadius));
            while(!walkableNode.isWalkable)
            {
                walkableNode = grid.NodeFromWorldPoint(searchCenter + (Random.insideUnitCircle * searchRadius));
            }
            eventManager.TriggerEvent(EnemyEvents.OnSetMovementTarget, new ParamsObject(walkableNode.worldPosition));
            return true;
        }

        [Task]
        bool SetSearchDirectionToLookDirection()
        {
            searchDirection = new Vector2(Mathf.Cos(rigidbody.rotation), Mathf.Sin(rigidbody.rotation));
            return true;
        }

        [Task]
        bool SetSearchCenterFromPosition()
        {
            searchCenter = rigidbody.position + (searchDirection.normalized * searchRadius);
            return true;
        }

        [Task]
        bool ResetSearchTimer()
        {
            searchTimer = searchTime;
            return true;
        }

        [Task]
        bool CheckSearchTimer()
        {
            searchTimer -= Time.deltaTime;
            if(searchTimer <= 0)
            {
                return false;
            }
            return true;
        }

        private void OnMapSendTransform(ParamsObject paramsObj)
        {
            grid = paramsObj.Transform.GetComponent<Pathfinding.Grid>();
            eventManager.StopListening(EnemyEvents.OnMapSendTransform, onMapSendTransformUnityAction);
        }
    }
}

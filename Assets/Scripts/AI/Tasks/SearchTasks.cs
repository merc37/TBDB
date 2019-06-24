using EventManagers;
using Panda;
using Pathfinding;
using UnityEngine;
using UnityEngine.Events;
using Events;
using System.Collections.Generic;

namespace Enemy
{
    public class SearchTasks : MonoBehaviour
    {
        [SerializeField]
        private float searchTime = 7;
        [SerializeField]
        private float searchRadius = 8.5f;
        [SerializeField]
        private float maxSearchCenterDistance = 10f;
        [SerializeField]
        private LayerMask unwalkableMask;

        private PathfindingGrid grid;

        private bool isSearching;

        private float searchTimer;

        private int lastFrameTimerWasChecked;

        private Vector2 playerLastKnownHeading;
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
            eventManager.StartListening(EnemyEvents.OnSetPlayerLastKnownHeading, new UnityAction<ParamsObject>(OnSetPlayerLastKnownHeading));

            searchTimer = searchTime;
        }

        void Update()
        {
            //if(isSearching)
            //{
            //    if((Time.frameCount - lastFrameTimerWasChecked) > 1)
            //    {
            //        searchTimer = searchTime;
            //        isSearching = false;
            //    }
            //}
        }

        [Task]
        bool SetMovementTargetToSearchPointAroundSearchCenter()
        {
            List<PathfindingNode> possibleSearchNodes = new List<PathfindingNode>();
            List<PathfindingNode> neighborsToCheck = new List<PathfindingNode>();
            HashSet<PathfindingNode> neighborsChecked = new HashSet<PathfindingNode>();
            PathfindingNode searchCenterNode = grid.NodeAtWorldPosition(searchCenter);
            float searchRadius = this.searchRadius;
            if(Vector2.Distance(searchCenter, searchCenterNode.WorldPosition) > grid.GridScale)
            {
                searchCenter = searchCenterNode.WorldPosition;
                searchRadius = Vector2.Distance(searchCenter, rigidbody.position);
            }
            neighborsToCheck.Add(searchCenterNode);
            PathfindingNode neighborToCheck;
            while(neighborsToCheck.Count > 0)
            {
                neighborToCheck = neighborsToCheck[0];
                neighborsToCheck.RemoveAt(0);
                foreach(PathfindingNode neighbor in grid.GetNeighbors(neighborToCheck))
                {
                    if(!neighborsChecked.Contains(neighbor))
                    {
                        if(Vector2.Distance(searchCenter, neighbor.WorldPosition) <= searchRadius)
                        {
                            neighborsToCheck.Add(neighbor);
                            if(neighbor.IsWalkable)
                            {
                                possibleSearchNodes.Add(neighbor);
                            }
                        }
                        neighborsChecked.Add(neighbor);
                    }
                }
            }

            if(possibleSearchNodes.Count <= 0)
            {
                return false;
            }

            PathfindingNode searchNode = possibleSearchNodes[Random.Range(0, possibleSearchNodes.Count - 1)];
            eventManager.TriggerEvent(EnemyEvents.OnSetMovementTarget, new ParamsObject(searchNode.WorldPosition));
            return true;
        }

        [Task]
        bool SetSearchCenterToCurrentPosition()
        {
            searchCenter = rigidbody.position;
            return true;
        }

        [Task]
        bool SetSearchCenterInMovementDirection()
        {
            searchCenter = rigidbody.position + (rigidbody.velocity.normalized * searchRadius);
            return true;
        }

        [Task]
        bool SetSearchCenterInLookDirection()
        {
            searchCenter = rigidbody.position + (new Vector2(Mathf.Cos(rigidbody.rotation), Mathf.Sin(rigidbody.rotation)).normalized * searchRadius);
            return true;
        }

        [Task]
        bool SetSearchCenterInPlayerLastKnownHeadingDirection()
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody.position, playerLastKnownHeading.normalized, maxSearchCenterDistance, unwalkableMask);
            if(hit.collider != null) {
                searchCenter = hit.point;
                return true;
            }
            searchCenter = rigidbody.position + (playerLastKnownHeading.normalized * maxSearchCenterDistance);
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
            isSearching = true;
            lastFrameTimerWasChecked = Time.frameCount;
            searchTimer -= Time.deltaTime;
            if(searchTimer <= 0)
            {
                searchTimer = searchTime;
                isSearching = false;
                return false;
            }
            return true;
        }

        [Task]
        bool SetSearching(bool searching) {
            isSearching = searching;
            return true;
        }

        [Task]
        bool IsSearching()
        {
            return isSearching;
        }

        private void OnMapSendTransform(ParamsObject paramsObj)
        {
            grid = paramsObj.Transform.GetComponent<PathfindingGrid>();
            eventManager.StopListening(EnemyEvents.OnMapSendTransform, onMapSendTransformUnityAction);
        }

        private void OnSetPlayerLastKnownHeading(ParamsObject paramsObj)
        {
            playerLastKnownHeading = paramsObj.Vector2;
        }
    }
}

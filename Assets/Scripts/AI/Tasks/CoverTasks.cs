using EventManagers;
using Panda;
using Pathfinding;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Events;

namespace Enemy
{
    public class CoverTasks : MonoBehaviour
    {
        private static readonly Vector2 NullVector = Constants.NullVector;

        [SerializeField]
        private float maxCoverSearchDistance = 20;
        [SerializeField]
        private LayerMask sightBlockMask;

        private Vector2 playerLastKnownPosition = NullVector;

        private PathfindingGrid grid;

        private GameObjectEventManager eventManager;
        private new Rigidbody2D rigidbody;

        private UnityAction<ParamsObject> onPlayerSendRigidbodyUnityAction;
        private UnityAction<ParamsObject> onMapSendTransformUnityAction;

        void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();

            eventManager = GetComponent<GameObjectEventManager>();
            onMapSendTransformUnityAction = new UnityAction<ParamsObject>(OnMapSendTransform);
            eventManager.StartListening(EnemyEvents.OnMapSendTransform, onMapSendTransformUnityAction);
            eventManager.StartListening(EnemyEvents.OnSetPlayerLastKnownLocation, new UnityAction<ParamsObject>(OnSetPlayerLastKnownLocation));
        }

        [Task]
        bool SetMovementTargetToCover()
        {
            Vector2 bestCoverPoint;
            List<PathfindingNode> potentialCoverNodes = new List<PathfindingNode>();
            HashSet<Vector2> inProcessQueue = new HashSet<Vector2>();
            Queue<PathfindingNode> nodesToProcess = new Queue<PathfindingNode>();
            grid.Reset();
            PathfindingNode node = grid.NodeAtWorldPosition(rigidbody.position);
            node.Parent = null;
            nodesToProcess.Enqueue(node);
            inProcessQueue.Add(node.WorldPosition);

            while(nodesToProcess.Count > 0 && Vector2.Distance(rigidbody.position, node.WorldPosition) < maxCoverSearchDistance)
            {
                node = nodesToProcess.Dequeue();
                RaycastHit2D raycastHit = Physics2D.Linecast(playerLastKnownPosition, node.WorldPosition, sightBlockMask);
                if(raycastHit)
                {
                    potentialCoverNodes.Add(node);
                }
                foreach(PathfindingNode neighbor in grid.GetNeighbors(node))
                {
                    if(!inProcessQueue.Contains(neighbor.WorldPosition))
                    {
                        if(neighbor.IsWalkable)
                        {
                            neighbor.Parent = node;
                            nodesToProcess.Enqueue(neighbor);
                            inProcessQueue.Add(neighbor.WorldPosition);
                        }
                    }
                }
            }

            int nodeCount = 0;
            int smallestNodeCount = int.MaxValue;
            PathfindingNode currNode;
            bestCoverPoint = NullVector;
            foreach(PathfindingNode coverPoint in potentialCoverNodes)
            {
                currNode = coverPoint;
                nodeCount = 0;
                while(currNode != null)
                {
                    currNode = currNode.Parent;
                    nodeCount++;
                }

                if(nodeCount < smallestNodeCount)
                {
                    smallestNodeCount = nodeCount;
                    bestCoverPoint = coverPoint.WorldPosition;
                }
            }

            if(bestCoverPoint != NullVector)
            {
                eventManager.TriggerEvent(EnemyEvents.OnSetMovementTarget, new ParamsObject(bestCoverPoint));
                return true;
            }

            return false;
        }

        private void OnSetPlayerLastKnownLocation(ParamsObject paramsObj)
        {
            playerLastKnownPosition = paramsObj.Vector2;
        }

        private void OnMapSendTransform(ParamsObject paramsObj)
        {
            grid = paramsObj.Transform.GetComponent<PathfindingGrid>();
            eventManager.StopListening(EnemyEvents.OnMapSendTransform, onMapSendTransformUnityAction);
        }

        void OnDrawGizmos()
        {
            if(playerLastKnownPosition != NullVector)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(playerLastKnownPosition, rigidbody.position);
            }
        }
    }
}

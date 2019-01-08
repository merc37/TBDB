using EventManagers;
using Panda;
using Pathfinding;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Events;

namespace Enemy
{
    public class FindCoverTasks : MonoBehaviour
    {
        private static readonly Vector2 NullVector = Constants.NullVector;

        [SerializeField]
        private float maxCoverSearchDistance = 20;
        [SerializeField]
        private LayerMask sightBlockMask;

        private bool coverFound;

        private PathfindingGrid grid;

        private GameObjectEventManager eventManager;
        private Rigidbody2D playerRigidbody;
        private new Rigidbody2D rigidbody;

        private UnityAction<ParamsObject> onPlayerSendRigidbodyUnityAction;
        private UnityAction<ParamsObject> onMapSendTransformUnityAction;

        void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();

            eventManager = GetComponent<GameObjectEventManager>();
            onMapSendTransformUnityAction = new UnityAction<ParamsObject>(OnMapSendTransform);
            eventManager.StartListening(EnemyEvents.OnMapSendTransform, onMapSendTransformUnityAction);
            onPlayerSendRigidbodyUnityAction = new UnityAction<ParamsObject>(OnPlayerSendRigidbody);
            eventManager.StartListening(EnemyEvents.OnPlayerSendRigidbody, onPlayerSendRigidbodyUnityAction);
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

            while(Vector2.Distance(rigidbody.position, node.WorldPosition) < maxCoverSearchDistance)
            {
                node = nodesToProcess.Dequeue();
                Quaternion left = Quaternion.AngleAxis(-7, Vector3.forward);
                Quaternion right = Quaternion.AngleAxis(7, Vector3.forward);
                RaycastHit2D leftRaycastHit = Physics2D.Linecast(playerRigidbody.position, left * node.WorldPosition, sightBlockMask);
                RaycastHit2D rightRaycastHit = Physics2D.Linecast(playerRigidbody.position, right * node.WorldPosition, sightBlockMask);
                if(leftRaycastHit && rightRaycastHit)
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


        [Task]
        bool SetCoverFound()
        {
            coverFound = true;
            return true;
        }

        [Task]
        bool FoundCover()
        {
            return coverFound;
        }

        [Task]
        bool InCover()
        {
            return !Physics2D.Linecast(playerRigidbody.position, rigidbody.position);
        }

        private void OnPlayerSendRigidbody(ParamsObject paramsObj)
        {
            playerRigidbody = paramsObj.Rigidbody;
            eventManager.StopListening(EnemyEvents.OnPlayerSendRigidbody, onPlayerSendRigidbodyUnityAction);
        }

        private void OnMapSendTransform(ParamsObject paramsObj)
        {
            grid = paramsObj.Transform.GetComponent<PathfindingGrid>();
            eventManager.StopListening(EnemyEvents.OnMapSendTransform, onMapSendTransformUnityAction);
        }
    }
}

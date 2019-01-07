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
        private LayerMask sightBlockMask;

        private bool coverFound;

        private Pathfinding.Grid grid;

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
            List<Node> potentialCoverNodes = new List<Node>();
            HashSet<Vector2> inProcessQueue = new HashSet<Vector2>();
            Queue<Node> nodesToProcess = new Queue<Node>();
            grid.resetGrid();
            Node node = grid.NodeFromWorldPoint(rigidbody.position);
            node.parent = null;
            nodesToProcess.Enqueue(node);
            inProcessQueue.Add(node.worldPosition);

            while(Vector2.Distance(rigidbody.position, node.worldPosition) < 20)
            {
                node = nodesToProcess.Dequeue();
                Quaternion left = Quaternion.AngleAxis(-7, Vector3.forward);
                Quaternion right = Quaternion.AngleAxis(7, Vector3.forward);
                RaycastHit2D leftRaycastHit = Physics2D.Linecast(playerRigidbody.position, left * node.worldPosition, sightBlockMask);
                RaycastHit2D rightRaycastHit = Physics2D.Linecast(playerRigidbody.position, right * node.worldPosition, sightBlockMask);
                if(leftRaycastHit && rightRaycastHit)
                {
                    potentialCoverNodes.Add(node);
                }
                foreach(Node neighbor in grid.GetNeighbors(node))
                {
                    if(!inProcessQueue.Contains(neighbor.worldPosition))
                    {
                        if(neighbor.isWalkable)
                        {
                            neighbor.parent = node;
                            nodesToProcess.Enqueue(neighbor);
                            inProcessQueue.Add(neighbor.worldPosition);
                        }
                    }
                }
            }

            int nodeCount = 0;
            int smallestNodeCount = int.MaxValue;
            Node currNode;
            bestCoverPoint = NullVector;
            foreach(Node coverPoint in potentialCoverNodes)
            {
                currNode = coverPoint;
                nodeCount = 0;
                while(currNode != null)
                {
                    currNode = currNode.parent;
                    nodeCount++;
                }

                if(nodeCount < smallestNodeCount)
                {
                    smallestNodeCount = nodeCount;
                    bestCoverPoint = coverPoint.worldPosition;
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
            grid = paramsObj.Transform.GetComponent<Pathfinding.Grid>();
            eventManager.StopListening(EnemyEvents.OnMapSendTransform, onMapSendTransformUnityAction);
        }
    }
}

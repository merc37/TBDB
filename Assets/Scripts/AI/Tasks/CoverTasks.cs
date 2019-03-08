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
        private float closeCoverDistance = 3;

        private Vector2 playerLastKnownPosition = NullVector;
        private Vector2 coverTarget;

        private float coverDistance;

        private float sightAngle;

        private LayerMask sightBlockMask;

        private PathfindingGrid grid;

        private GameObjectEventManager eventManager;
        private new Rigidbody2D rigidbody;

        private UnityAction<ParamsObject> onPlayerSendRigidbodyUnityAction;
        private UnityAction<ParamsObject> onMapSendTransformUnityAction;
        private UnityAction<ParamsObject> onSendSightAngleUnityAction;
        private UnityAction<ParamsObject> onSendSightBlockMaskUnityAction;

        void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();

            eventManager = GetComponent<GameObjectEventManager>();
            onMapSendTransformUnityAction = new UnityAction<ParamsObject>(OnMapSendTransform);
            eventManager.StartListening(EnemyEvents.OnMapSendTransform, onMapSendTransformUnityAction);
            eventManager.StartListening(EnemyEvents.OnSetPlayerLastKnownLocation, new UnityAction<ParamsObject>(OnSetPlayerLastKnownLocation));
            onSendSightAngleUnityAction = new UnityAction<ParamsObject>(OnSendSightAngle);
            eventManager.StartListening(EnemyEvents.OnSendSightAngle, onSendSightAngleUnityAction);
            onSendSightBlockMaskUnityAction = new UnityAction<ParamsObject>(OnSendSightBlockMask);
            eventManager.StartListening(EnemyEvents.OnSendSightBlockMask, onSendSightBlockMaskUnityAction);
        }

        Vector2 leftP, rightP, nodeP;
        [Task]
        bool SetCoverTargetFromPlayerLastKnownPosition()
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

                Quaternion left = Quaternion.AngleAxis(-7, Vector3.forward);
                Quaternion right = Quaternion.AngleAxis(7, Vector3.forward);
                RaycastHit2D leftRaycastHit = Physics2D.Linecast(playerLastKnownPosition, left * node.WorldPosition, sightBlockMask);
                RaycastHit2D rightRaycastHit = Physics2D.Linecast(playerLastKnownPosition, right * node.WorldPosition, sightBlockMask);
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

            float nodeDistance = 0;
            float smallestNodeDistance = float.MaxValue;
            PathfindingNode currNode;
            bestCoverPoint = NullVector;
            foreach(PathfindingNode coverNode in potentialCoverNodes)
            {
                currNode = coverNode;
                nodeDistance = 0;
                while(currNode != null)
                {
                    if(currNode.Parent != null)
                    {
                        nodeDistance += Vector2.Distance(currNode.WorldPosition, currNode.Parent.WorldPosition);
                    }
                    currNode = currNode.Parent;
                }
                if(nodeDistance < 5)
                {
                    //print("NodePos: " + coverNode.WorldPosition + ", NodeDist: " + nodeDistance);
                }
                if(nodeDistance < smallestNodeDistance)
                {
                    smallestNodeDistance = nodeDistance;
                    bestCoverPoint = coverNode.WorldPosition;
                }
            }

            if(bestCoverPoint != NullVector)
            {
                coverDistance = smallestNodeDistance;
                coverTarget = bestCoverPoint;
                //print("BEST NodePos: " + coverTarget + ", NodeDist: " + coverDistance);
                Quaternion left = Quaternion.AngleAxis(-sightAngle, Vector3.forward);
                Quaternion right = Quaternion.AngleAxis(sightAngle, Vector3.forward);
                leftP = left * coverTarget;
                rightP = right * coverTarget;
                nodeP = coverTarget;
                return true;
            }

            return false;
        }

        [Task]
        bool SetMovementTargetToCoverTarget()
        {
            eventManager.TriggerEvent(EnemyEvents.OnSetMovementTarget, new ParamsObject(coverTarget));
            return true;
        }

        [Task]
        bool IsCoverTargetClose()
        {
            return coverDistance <= closeCoverDistance;
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

        private void OnSendSightAngle(ParamsObject paramsObj)
        {
            sightAngle = paramsObj.Float;
            eventManager.StopListening(EnemyEvents.OnSendSightAngle, onSendSightAngleUnityAction);
        }

        private void OnSendSightBlockMask(ParamsObject paramsObj)
        {
            sightBlockMask = paramsObj.LayerMask;
            eventManager.StopListening(EnemyEvents.OnSendSightBlockMask, onSendSightBlockMaskUnityAction);
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawCube(leftP, new Vector3(.3f, .3f, .3f));
            Gizmos.color = Color.green;
            Gizmos.DrawCube(rightP, new Vector3(.3f, .3f, .3f));
            Gizmos.color = Color.black;
            Gizmos.DrawCube(nodeP, new Vector3(.3f, .3f, .3f));
        }
    }
}

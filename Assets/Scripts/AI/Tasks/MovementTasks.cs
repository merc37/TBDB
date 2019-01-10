using EventManagers;
using Panda;
using Pathfinding;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Events;

namespace Enemy
{
    public class MovementTasks : MonoBehaviour
    {
        private static readonly Vector2 NullVector = Constants.NullVector;

        [SerializeField]
        private float maxRunSpeed = 15;
        [SerializeField]
        private float recalculatePathDistance = 5;
        //[SerializeField]
        private float maxPathSearchDistance = Mathf.Infinity;
        [SerializeField]
        private float pointAccuracy = 0.1f;

        private List<PathfindingNode> path;

        private Vector2 movementDirection;
        private Vector2 movementTarget;

        private bool reachedEndOfPath;

        private GameObjectEventManager eventManager;
        private Pathfinder pathfinder;
        private new Rigidbody2D rigidbody;
        private new Collider2D collider;

        private UnityAction<ParamsObject> onMapSendTransformUnityAction;

        void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();
            collider = GetComponent<Collider2D>();

            eventManager = GetComponent<GameObjectEventManager>();
            onMapSendTransformUnityAction = new UnityAction<ParamsObject>(OnMapSendTransform);
            eventManager.StartListening(EnemyEvents.OnMapSendTransform, onMapSendTransformUnityAction);
            eventManager.StartListening(EnemyEvents.OnSetMovementTarget, new UnityAction<ParamsObject>(OnSetMovementTarget));
        }

        void FixedUpdate()
        {
            rigidbody.velocity = Vector2.ClampMagnitude(movementDirection.normalized * maxRunSpeed, maxRunSpeed);
        }

        [Task]
        bool MoveTowardsMovementTarget()
        {
            movementDirection = (movementTarget - rigidbody.position);
            return true;
        }

        [Task]
        bool ForceRecalculatePathToMovementTarget()
        {
            path = pathfinder.FindPath(rigidbody.position, movementTarget, maxPathSearchDistance);
            return path != null;
        }

        private List<PathfindingNode> gizmoPath;
        [Task]
        bool PathToMovementTarget()
        {
            //If path is empty or the target is too far from the end of it, set it
            if(path == null || path.Count <= 0)
            {
                path = pathfinder.FindPath(rigidbody.position, movementTarget, maxPathSearchDistance);
                gizmoPath = path.ConvertAll(node => new PathfindingNode(node.WorldPosition, node.GridPosition, node.IsWalkable));
                for(int i = 0; i < gizmoPath.Count; i++)
                {
                    gizmoPath[i].Parent = path[i].Parent;
                }
            }
            if(path != null && path.Count > 0)
            {
                PathfindingNode lastNode = path[path.Count - 1];
                if(Vector2.Distance(lastNode.WorldPosition, movementTarget) > recalculatePathDistance)
                {
                    path = pathfinder.FindPath(rigidbody.position, movementTarget, maxPathSearchDistance);
                    gizmoPath = path.ConvertAll(node => new PathfindingNode(node.WorldPosition, node.GridPosition, node.IsWalkable));
                    for(int i = 0; i < gizmoPath.Count; i++)
                    {
                        gizmoPath[i].Parent = path[i].Parent;
                    }
                }
            }

            //If path still empty there is no route to the target and this should return
            if(path == null)
            {
                return false;
            }

            //If path count is 0, the target node is the start node so just move to movement targetd
            if(path.Count == 0)
            {
                movementDirection = movementTarget - rigidbody.position;

                return true;
            }

            if(path.Count != 0 && ReachedNode(path[0]))
            {
                path.RemoveAt(0);
                //reached end of path
                if(path.Count == 0)
                {
                    reachedEndOfPath = true;
                    return true;
                }
            }

            Vector2 nodeWorldPos = path[0].WorldPosition;
            movementDirection = nodeWorldPos - rigidbody.position;

            return true;
        }

        [Task]
        bool IsMovementStopped()
        {
            return rigidbody.velocity.magnitude < pointAccuracy;
        }

        [Task]
        bool SetRotationToMovementTarget()
        {
            Vector2 direction = movementTarget - rigidbody.position;
            rigidbody.rotation = direction.AngleFromZero();
            return true;
        }

        [Task]
        bool SetRotationToMovementDirection()
        {
            rigidbody.rotation = movementDirection.AngleFromZero();
            return true;
        }

        [Task]
        bool SetRotationToOppositeMovementDirection()
        {
            rigidbody.rotation = (-movementDirection).AngleFromZero();
            return true;
        }

        [Task]
        bool StopMovement()
        {
            if(path != null)
            {
                path.Clear();
                gizmoPath.Clear();
            }
            movementDirection = Vector2.zero;

            return true;
        }

        [Task]
        bool ReachedMovementTarget()
        {
            bool reachedMovementTarget = reachedEndOfPath || (Vector2.Distance(rigidbody.position, movementTarget) < pointAccuracy);
            reachedEndOfPath = false;
            return reachedMovementTarget;
        }

        [Task]
        bool SetMovementTargetToNull()
        {
            movementTarget = NullVector;
            return true;
        }

        [Task]
        bool IsMovementTargetNull()
        {
            return movementTarget == NullVector;
        }

        private bool ReachedNode(PathfindingNode node)
        {
            if(isAtNode(node)) return true;
            if(rigidbody.velocity.magnitude > 0)
            {
                Vector2 p1 = node.WorldPosition;
                Vector2 p2 = Vector3.Cross(rigidbody.velocity.normalized, -Vector3.forward).normalized;
                Vector2 nodeWorldPosition = node.WorldPosition;
                p2 = p2 + nodeWorldPosition;
                float d = (rigidbody.position.x - p1.x) * (p2.y - p1.y) - (rigidbody.position.y - p1.y) * (p2.x - p1.x);
                return d > 0;
            }
            return false;
        }

        private bool isAtNode(PathfindingNode node)
        {
            float distanceFromNode = Vector2.Distance(rigidbody.position, node.WorldPosition);
            if(distanceFromNode <= pointAccuracy) return true;
            else return false;
        }

        private void OnMapSendTransform(ParamsObject paramsObj)
        {
            pathfinder = paramsObj.Transform.GetComponent<Pathfinder>();
            eventManager.StopListening(EnemyEvents.OnMapSendTransform, onMapSendTransformUnityAction);
        }

        private void OnSetMovementTarget(ParamsObject paramsObj)
        {
            movementTarget = paramsObj.Vector2;
        }

        void OnDrawGizmos()
        {
            if(gizmoPath != null)
            {
                foreach(var node in gizmoPath)
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawLine(node.WorldPosition, node.Parent.WorldPosition);
                    node.DrawGizmos(0.25f, true);
                }
            }
        }
    }
}
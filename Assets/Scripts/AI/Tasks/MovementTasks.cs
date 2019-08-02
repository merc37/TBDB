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
        [SerializeField]
        private float maxRunSpeed = 15;
        [SerializeField]
        private float recalculatePathDistance = 5;
        //[SerializeField]
        private float maxPathSearchDistance = Mathf.Infinity;
        [SerializeField]
        private float pointAccuracy = 0.1f;
        [SerializeField]
        private LayerMask movementBlockMask;

        private List<PathfindingNode> path;

        private Vector2 movementDirection;
        private Vector2 movementTarget = Constants.NullVector, lastMovementTargetPathedTo = -Constants.NullVector;

        private bool reachedEndOfPath;
        private bool rolling;

        private GameObjectEventManager eventManager;
        private Pathfinder pathfinder;
        private new Rigidbody2D rigidbody;
        private new CircleCollider2D collider;

        private UnityAction<ParamsObject> onMapSendTransformUnityAction;

        void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();
            collider = GetComponent<CircleCollider2D>();
            eventManager = GetComponent<GameObjectEventManager>();
            onMapSendTransformUnityAction = new UnityAction<ParamsObject>(OnMapSendTransform);
            eventManager.StartListening(EnemyEvents.OnMapSendTransform, onMapSendTransformUnityAction);
            eventManager.StartListening(EnemyEvents.OnSetMovementTarget, new UnityAction<ParamsObject>(OnSetMovementTarget));
            eventManager.StartListening(EnemyEvents.OnRollStart, new UnityAction<ParamsObject>(OnRollStart));
            eventManager.StartListening(EnemyEvents.OnRollEnd, new UnityAction<ParamsObject>(OnRollEnd));
        }

        void Start()
        {
            eventManager.TriggerEvent(EnemyEvents.OnSendMovementSpeed, new ParamsObject(maxRunSpeed));
        }

        void FixedUpdate()
        {
            if(!rolling)
            {
                rigidbody.velocity = Vector2.ClampMagnitude(movementDirection.normalized * maxRunSpeed, maxRunSpeed);
            }
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
            path = pathfinder.FindPath(rigidbody.position, movementTarget, collider, maxPathSearchDistance);
            return path != null;
        }

        private List<PathfindingNode> gizmoPath;
        [Task]
        bool PathToMovementTarget()
        {
            //If path is empty or the target is too far from the end of it, set it
            if(path == null || path.Count <= 0 || movementTarget == Constants.NullVector || lastMovementTargetPathedTo != movementTarget)
            {
                path = pathfinder.FindPath(rigidbody.position, movementTarget, collider, maxPathSearchDistance);
                lastMovementTargetPathedTo = movementTarget;
                gizmoPath = path.ConvertAll(node => new PathfindingNode(node.WorldPosition, node.GridPosition, node.IsWalkable));
                for(int i = 0; i < gizmoPath.Count; i++)
                {
                    gizmoPath[i].Parent = path[i].Parent;
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
            rigidbody.rotation = direction.ToAngle();
            return true;
        }

        [Task]
        bool SetRotationToMovementDirection()
        {
            rigidbody.rotation = movementDirection.ToAngle();
            return true;
        }

        [Task]
        bool SetRotationToOppositeMovementDirection()
        {
            rigidbody.rotation = (-movementDirection).ToAngle();
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
            movementTarget = Constants.NullVector;
            return true;
        }

        [Task]
        bool IsMovementTargetNull()
        {
            return movementTarget == Constants.NullVector;
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

        private void OnRollStart(ParamsObject paramsObj)
        {
            rolling = true;
        }

        private void OnRollEnd(ParamsObject paramsObj)
        {
            rolling = false;
        }

        void OnDrawGizmos()
        {
            if(gizmoPath != null)
            {
                Gizmos.color = Color.white;
                foreach(var node in gizmoPath)
                {
                    Gizmos.DrawLine(node.WorldPosition, node.Parent.WorldPosition);
                    node.DrawGizmos(0.25f, true);
                }
            }
            Gizmos.color = Color.cyan;
            Gizmos.DrawCube(movementTarget, new Vector3(.4f, .4f, .4f));
        }
    }
}
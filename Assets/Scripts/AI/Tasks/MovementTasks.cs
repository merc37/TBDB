using EventManagers;
using Panda;
using Pathfinding;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Enemy
{
    public class MovementTasks : MonoBehaviour
    {
        private static readonly Vector2 NullVector = Constants.NullVector;

        [SerializeField]
        private float maxRunSpeed = 15;
        [SerializeField]
        private float recalculatePathDistance = 5;
        [SerializeField]
        private float maxPathSearchDistance = 20;
        [SerializeField]
        private float pointAccuracy = 0.1f;

        private List<Node> path;

        private Vector2 movementDirection;
        private Vector2 movementTarget;

        private bool reachedEndOfPath;

        private GameObjectEventManager eventManager;
        private BasicThetaStarPathfinding pathfinding;
        private new Rigidbody2D rigidbody;
        private new Collider2D collider;

        void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();
            collider = GetComponent<Collider2D>();

            eventManager = GetComponent<GameObjectEventManager>();
            eventManager.StartListening("ReturnMapTransform", new UnityAction<ParamsObject>(SetPathfinding));
            eventManager.StartListening("OnSetMovementTarget", new UnityAction<ParamsObject>(SetMovementTarget));
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
        bool PathToMovementTarget()
        {
            //If path is empty or the target is too far from the end of it, set it
            bool recalculatePath = path == null || path.Count == 0;
            recalculatePath = recalculatePath || Vector2.Distance(path[path.Count - 1].worldPosition, movementTarget) > recalculatePathDistance;
            //if(path == null || path.Count == 0)
            //{
            //    path = pathfinding.FindPath(rigidbody.position, movementTarget, collider, maxPathSearchDistance);
            //}
            //else if(Vector2.Distance(path[path.Count - 1].worldPosition, movementTarget) > recalculatePathDistance)
            //{
            //    path = pathfinding.FindPath(rigidbody.position, movementTarget, collider, maxPathSearchDistance);
            //}

            if(recalculatePath)
            {
                path = pathfinding.FindPath(rigidbody.position, movementTarget, collider, maxPathSearchDistance);
            }

            //If path still empty there is no route to the target and this should return
            if(path == null || path.Count == 0)
            {
                return false;
            }

            GameObject.Find("testMap2").GetComponent<Pathfinding.Grid>().path = new List<Node>(path);//Just for gizmos

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

            Vector2 nodeWorldPos = path[0].worldPosition;
            movementDirection = nodeWorldPos - rigidbody.position;

            return true;
        }

        [Task]
        bool IsMovementStopped()
        {
            return rigidbody.velocity.magnitude < pointAccuracy;
        }

        [Task]
        bool SetRotationToMovementDirection()
        {
            rigidbody.rotation = movementDirection.AngleFromZero();
            return true;
        }

        [Task]
        bool StopMovement()
        {
            if(path != null)
            {
                path.Clear();
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

        private bool ReachedNode(Node node)
        {
            if(isAtNode(node)) return true;
            if(rigidbody.velocity.magnitude > 0)
            {
                Vector2 p1 = node.worldPosition;
                Vector2 p2 = Vector3.Cross(rigidbody.velocity.normalized, -Vector3.forward).normalized;
                Vector2 nodeWorldPosition = node.worldPosition;
                p2 = p2 + nodeWorldPosition;
                float d = (rigidbody.position.x - p1.x) * (p2.y - p1.y) - (rigidbody.position.y - p1.y) * (p2.x - p1.x);
                return d > 0;
            }
            return false;
        }

        private bool isAtNode(Node node)
        {
            float distanceFromNode = Vector2.Distance(rigidbody.position, node.worldPosition);
            if(distanceFromNode <= pointAccuracy) return true;
            else return false;
        }

        private void SetPathfinding(ParamsObject paramsObj)
        {
            pathfinding = paramsObj.Transform.GetComponent<BasicThetaStarPathfinding>();
            eventManager.StopListening("ReturnMapTransform", new UnityAction<ParamsObject>(SetPathfinding));
        }

        private void SetMovementTarget(ParamsObject paramsObj)
        {
            movementTarget = paramsObj.Vector2;
        }
    }
}
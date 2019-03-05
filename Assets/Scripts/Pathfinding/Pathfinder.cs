using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pathfinding
{
    [RequireComponent(typeof(PathfindingGrid))]
    public class Pathfinder : MonoBehaviour
    {
        private PathfindingGrid _pathfindingGrid;

        void Awake()
        {
            _pathfindingGrid = GetComponent<PathfindingGrid>();
        }

        public List<PathfindingNode> FindPath(Vector2 startPosition, Vector2 targetPosition, Collider2D collider, float maxDistance = Mathf.Infinity)
        {
            _pathfindingGrid.Reset();

            var startNode = _pathfindingGrid.NodeAtWorldPosition(startPosition);
            var targetNode = _pathfindingGrid.NodeAtWorldPosition(targetPosition);

            // Try to find valid node if target is not walkable
            if(!targetNode.IsWalkable)
            {
                var walkableNeighbors = _pathfindingGrid.GetNeighbors(targetNode).Where(n => n.IsWalkable);
                if(!walkableNeighbors.Any())
                {
                    return null;
                }

                float minDistToStart = Mathf.Infinity;
                foreach(var node in walkableNeighbors)
                {
                    var dist = Vector2.Distance(startPosition, node.WorldPosition);
                    if(dist < minDistToStart)
                    {
                        targetNode = node;
                        minDistToStart = dist;
                    }
                }
            }

            // Find path
            List<PathfindingNode> openSet = new List<PathfindingNode>();
            HashSet<PathfindingNode> closedSet = new HashSet<PathfindingNode>();

            startNode.GCost = 0;
            startNode.Parent = startNode;
            openSet.Add(startNode);

            while(openSet.Count > 0)
            {
                var currentNode = openSet[0];
                foreach(var node in openSet)
                {
                    if(node.FCost < currentNode.FCost
                        || (node.FCost == currentNode.FCost && node.HCost < currentNode.HCost))
                    {
                        currentNode = node;
                    }
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if(currentNode == targetNode)
                {
                    return RetracePath(startNode, targetNode);
                }

                var neighbors = _pathfindingGrid.GetNeighbors(currentNode);
                foreach(var neighbor in neighbors)
                {
                    if(!neighbor.IsWalkable || closedSet.Contains(neighbor)
                        || neighbor.FCost > maxDistance)
                    {
                        continue;
                    }

                    // Basic Theta Star update vertex
                    if(currentNode.Parent != null && _pathfindingGrid.LineOfSight(currentNode, currentNode.Parent, collider))
                    {
                        float newMoveCost = currentNode.Parent.GCost +
                                            Vector2.Distance(currentNode.Parent.WorldPosition, neighbor.WorldPosition);
                        if(newMoveCost < neighbor.GCost)
                        {
                            neighbor.GCost = newMoveCost;
                            neighbor.HCost = Vector2.Distance(neighbor.WorldPosition, targetNode.WorldPosition);
                            neighbor.Parent = currentNode.Parent;

                            if(!openSet.Contains(neighbor))
                                openSet.Add(neighbor);
                        }
                    }
                    else
                    {
                        float newMoveCost = currentNode.GCost + Vector2.Distance(currentNode.WorldPosition, neighbor.WorldPosition);
                        if(newMoveCost < neighbor.GCost)
                        {
                            neighbor.GCost = newMoveCost;
                            neighbor.HCost = Vector2.Distance(neighbor.WorldPosition, targetNode.WorldPosition);
                            neighbor.Parent = currentNode;

                            if(!openSet.Contains(neighbor))
                                openSet.Add(neighbor);
                        }
                    }
                }
            }

            return null;
        }

        private List<PathfindingNode> RetracePath(PathfindingNode start, PathfindingNode target)
        {
            List<PathfindingNode> path = new List<PathfindingNode>();

            var node = target;
            while(node != null && node != start)
            {
                path.Add(node);
                node = node.Parent;
            }
            path.Reverse();
            return path;
        }
    }
}
